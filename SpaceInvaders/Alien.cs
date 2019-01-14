
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace SpaceInvaders
{
    /// <summary>
    ///  a class of alien
    /// </summary>
    public class Alien : DrawableGameComponent
    {
        // declaring variables and constant
        private SpriteBatch spriteBatch;
        private Texture2D tex;
        private Vector2 position;
        private Vector2 originalPosition;
        private static Vector2 speed, nextSpeed;
        private SoundEffect pewSound, popSound;
        private static int totalNum = 0;
        private static int updatedNum = 0;
        private static string moveDirection = "Right";
        private Vector2 dimension;
        private List<Rectangle> frames;
        private int frameIndex = 0;
        private int movingDelay = 30;
        private int explosionDelay = 1;
        private int delayCounter;
        private bool isInExplosion = false;
        private Game game;
        private bool removable;
        private int points;
        private int row;
        private int col;
        private static bool changeDirection = false;
        private static bool crossedLine = false;
        private const int DEFENSELINE_HEIGHT = 64;
        private const int ALIEN_WIDTH = 36;
        private const int ALIEN_HEIGHT = 36;

        public bool Removable { get => removable; set => removable = value; }
        public static Vector2 Speed { get => speed; set => speed = value; }
        public static bool CrossedLine { get => crossedLine; set => crossedLine = value; }
        public static int UpdatedNum { get => updatedNum; set => updatedNum = value; }
        public static int TotalNum { get => totalNum; set => totalNum = value; }
        public int Row { get => row; set => row = value; }
        public int Col { get => col; set => col = value; }
        public int Points { get => points; set => points = value; }
        /// <summary>
        /// Alien constructor
        /// </summary>
        /// <param name="game">this game</param>
        /// <param name="spriteBatch">spriteBatch</param>
        /// <param name="tex">alien texture</param>
        /// <param name="position">alien position</param>
        /// <param name="pewSound">pewSound</param>
        /// <param name="popSound">popSound</param>
        /// <param name="points">points per alien</param>
        public Alien(Game game,
            SpriteBatch spriteBatch,
            Texture2D tex,
            Vector2 position,
            SoundEffect pewSound,
            SoundEffect popSound,
            int points) : base(game)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            this.tex = tex;
            this.position = position;
            this.originalPosition = position;
            this.Visible = true;
            this.removable = false;
            this.pewSound = pewSound;
            this.popSound = popSound;
            this.points = points;
            moveDirection = "Right";
            dimension = new Vector2(ALIEN_WIDTH, ALIEN_HEIGHT);

            createFrames();
        }
        /// <summary>
        /// mothed to reset the aliens
        /// </summary>
        public void Reset()
        {
            position = originalPosition;
            Visible = true;
            frameIndex = 0;
            isInExplosion = false;
            removable = false;
            delayCounter = 0;
            moveDirection = "Right";
        }
        /// <summary>
        /// method to create alien frames
        /// </summary>
        private void createFrames()
        {
            frames = new List<Rectangle>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int x = j * (int)dimension.X;
                    int y = i * (int)dimension.Y;
                    Rectangle r = new Rectangle(x, y, (int)dimension.X, (int)dimension.Y);
                    frames.Add(r);
                }
            }
        }

        public event EventHandler ExplosionDone;
        /// <summary>
        /// This is called when draw alien.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (frameIndex < 6)
            {                
                spriteBatch.Draw(tex, position, frames[frameIndex], Color.White);
            }
            else
            {
                Visible = false;
                ExplosionDone.Invoke(this, EventArgs.Empty);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// When the left button of mouse is clicked, invoke an event.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {
            delayCounter++;
            if (!isInExplosion)
            {
                if (delayCounter > movingDelay)
                {
                    frameIndex++;
                    delayCounter = 0;

                    frameIndex &= 1;
                    updatedNum++;

                    if (moveDirection == "Down")
                    {
                        position.Y += speed.Y;
                        changeDirection = true;
                        if (position.Y > Shared.stage.Y - DEFENSELINE_HEIGHT - ALIEN_HEIGHT)
                        {
                            crossedLine = true;
                        }
                    }
                    else
                    {
                        position.X += speed.X;
                        if ((moveDirection == "Right" && position.X + ALIEN_HEIGHT + 20 > Shared.stage.X) ||
                            (moveDirection == "Left" && position.X - 20 < 0))
                        {
                            nextSpeed.X = -1 * speed.X;
                            changeDirection = true;
                        }
                    }
                }
            }
            else
            {
                if (delayCounter > explosionDelay)
                {
                    frameIndex++;
                    delayCounter = 0;
                }
            }

            base.Update(gameTime);
        }
        /// <summary>
        /// method to get alien bounds
        /// </summary>
        /// <returns>rectangle</returns>
        public Rectangle getBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, tex.Width / 2, tex.Height / 3);
        }
        /// <summary>
        /// method used when alien is exploding
        /// </summary>
        public void Explode()
        {
            frameIndex = 1;
            isInExplosion = true;
            popSound.Play();
            totalNum--;
        }
        /// <summary>
        /// method of alien firing bolts
        /// </summary>
        /// <returns>bolt</returns>
        public Bolt FireBolt(Vector2 boltSpeed)
        {
            Vector2 boltDimension = new Vector2(4, 12);
            Vector2 boltInitPos = new Vector2((ALIEN_WIDTH - boltDimension.X)/ 2, ALIEN_HEIGHT);
            boltInitPos += position;           
            Bolt bolt = new Bolt(game, spriteBatch, boltInitPos, boltDimension, Color.Red, boltSpeed);
            pewSound.Play();
            return bolt;
        }
        /// <summary>
        /// aliens move as a team, used when change direction and fire bolts
        /// </summary>
        /// <returns>bool</returns>
        public static bool AllMovedAStep()
        {
            bool isTeamMoved = false;

            if ((UpdatedNum >= TotalNum) && (TotalNum > 0))
            {
                isTeamMoved = true;
                UpdatedNum = 0;
                if (changeDirection)
                {
                    if (moveDirection == "Right" || moveDirection == "Left")
                    {
                        moveDirection = "Down";
                    }
                    else
                    {
                        speed.X = nextSpeed.X;
                        if (speed.X > 0)
                        {
                            moveDirection = "Right";
                        }
                        else
                        {
                            moveDirection = "Left";
                        }

                    }
                    changeDirection = false;
                }
            }

            return isTeamMoved;
        }
    }
}
