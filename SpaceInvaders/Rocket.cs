
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
    /// a class of rocket
    /// </summary>
    public class Rocket : DrawableGameComponent
    {
        // declaring variables
        private const float angularVelocity = (float)(Math.PI / 96);
        private const float rotationRadius = 1.0f;
        private const int speedH = 4;
        private const int FireDelay = 40;
        private const int ExplosionDelay = 3;

        private SpriteBatch spriteBatch;
        private Texture2D tex;
        private Texture2D explosionTex;
        private Vector2 position;
        private Vector2 originalPosition;
        private Vector2 speed, originalSpeed;
        private SoundEffect pewSound, blastSound;
        private float scale = 0.2f;
        private int points;
        private float rotation = 0;
        private int delayCounter;

        private Vector2 dimension;
        private List<Rectangle> frames;
        private int frameIndex = -1;
        private bool isInExplosion = false;
        private enum MoveState
        {
            Right,
            Right_Rotate,
            Left,
            Left_Rotate
        }
        private MoveState moveState;

        public Vector2 Position { get => position; set => position = value; }
        public Vector2 BoltPosition { get; set; }
        public float Scale { get => scale; set => scale = value; }
        public int Points { get => points; set => points = value; }
        public bool IsInExplosion { get => isInExplosion; set => isInExplosion = value; }
        /// <summary>
        /// Rocket constructor
        /// </summary>
        /// <param name="game">this game</param>
        /// <param name="spriteBatch">spriteBatch</param>
        /// <param name="tex">tex</param>
        /// <param name="explosionTex">explosionTex</param>
        /// <param name="position">position</param>
        /// <param name="pewSound">pewSound</param>
        /// <param name="blastSound">blastSound</param>
        /// <param name="points">points</param>
        public Rocket(Game game,
            SpriteBatch spriteBatch,
            Texture2D tex,
            Texture2D explosionTex,
            Vector2 position,
            SoundEffect pewSound,
            SoundEffect blastSound,
            int points) : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.tex = tex;
            this.explosionTex = explosionTex;
            this.position = position;
            this.originalPosition = position;
            this.speed = new Vector2(speedH, 0);
            this.originalSpeed = this.speed;
            this.pewSound = pewSound;
            this.blastSound = blastSound;
            this.points = points;
            this.moveState = MoveState.Right;

            dimension = new Vector2(64, 64);
            createFrames();
        }
        /// <summary>
        /// method to reset rocket
        /// </summary>
        public void Reset()
        {
            position = originalPosition;
            speed = originalSpeed;
            frameIndex = -1;
            delayCounter = 0;
            rotation = 0;
            isInExplosion = false;
            moveState = MoveState.Right;
        }
        /// <summary>
        /// method to create rocket frames
        /// </summary>
        private void createFrames()
        {
            frames = new List<Rectangle>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int x = j * (int)dimension.X;
                    int y = i * (int)dimension.Y;
                    Rectangle r = new Rectangle(x, y, (int)dimension.X, (int)dimension.Y);
                    frames.Add(r);
                }
            }
        }
        /// <summary>
        /// This is called when draw AboutScene.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (frameIndex >= 0)
            {
                spriteBatch.Draw(explosionTex, position - new Vector2(tex.Width / 2, tex.Height / 2) * scale, frames[frameIndex], Color.White);
            }
            else
            {
                spriteBatch.Draw(tex, position, null, Color.White, rotation,
                    new Vector2(tex.Width / 2, tex.Height / 2), scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public event EventHandler FireBolt;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// When the left button of mouse is clicked, invoke an event.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {
            delayCounter++;
            if (isInExplosion)
            {
                if (delayCounter > ExplosionDelay)
                {
                    frameIndex++;
                    if (frameIndex > 24)
                    {
                        Reset();
                    }
                    delayCounter = 0;
                }
            }
            else
            {
                position += speed;
                switch (moveState)
                {
                    case MoveState.Right:
                        CheckAndFire();
                        if (position.X + tex.Width * scale + 20 > Shared.stage.X)
                        {
                            moveState = MoveState.Right_Rotate;
                        }
                        break;

                    case MoveState.Right_Rotate:
                        rotation += angularVelocity;
                        speed.X = (float)(rotationRadius * Math.Cos(rotation));
                        speed.Y = (float)(rotationRadius * Math.Sin(rotation));
                        if (rotation >= Math.PI)
                        {
                            moveState = MoveState.Left;
                            rotation = (float)Math.PI;
                            speed.X = -originalSpeed.X;
                            speed.Y = 0;
                        }
                        break;
                    case MoveState.Left:
                        CheckAndFire();
                        if (position.X - tex.Width * scale - 20 < 0)
                        {
                            moveState = MoveState.Left_Rotate;
                        }
                        break;
                    case MoveState.Left_Rotate:
                        rotation += angularVelocity;
                        speed.X = (float)(rotationRadius * Math.Cos(rotation/* + angle*/));
                        speed.Y = (float)(rotationRadius * Math.Sin(rotation/* + angle*/));
                        if (rotation >= Math.PI * 2)
                        {
                            moveState = MoveState.Right;
                            rotation = 0;
                            position.Y = originalPosition.Y;
                            speed.X = originalSpeed.X;
                            speed.Y = 0;
                        }
                        break;
                    default:
                        break;
                }
            }

            base.Update(gameTime);
        }
        /// <summary>
        /// method to check and fire bolts
        /// </summary>
        private void CheckAndFire()
        {
            if ((delayCounter > FireDelay) && (position.X > 0))
            {
                pewSound.Play();
                BoltPosition = position + new Vector2(0, tex.Height * scale / 4);
                FireBolt.Invoke(this, EventArgs.Empty);
                delayCounter = 0;
            }
        }
        /// <summary>
        /// method to get rocket bounds
        /// </summary>
        /// <returns>rectangle</returns>
        public Rectangle getBounds()
        {
            return new Rectangle((int)(position.X - tex.Width * scale / 2), (int)(position.Y - tex.Height * scale / 2),
                (int)(tex.Width * scale), (int)(tex.Height * scale));
        }
        /// <summary>
        /// rocket explode
        /// </summary>
        public void Explode()
        {
            frameIndex = 0;
            isInExplosion = true;
            blastSound.Play();
        }
    }
}
