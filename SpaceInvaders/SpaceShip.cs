
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
    public class SpaceShip : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D tex;
        private Vector2 position, originalPosition;
        private Vector2 speed;
        private SoundEffect pewSound;
        private SoundEffect blastSound;
        private int lives;
        private Vector2 dimension;
        private List<Rectangle> frames;
        private int delayCounter;
        private int frameIndex = 0;
        private bool isInExplosion = false;

        public Vector2 Position { get => position; set => position = value; }
        public Vector2 BoltPosition { get; set; }
        public int Lives { get => lives; set => lives = value; }
        public bool IsInExplosion { get => isInExplosion; set => isInExplosion = value; }
        public SpaceShip(Game game,
            SpriteBatch spriteBatch,
            Texture2D tex,
            Vector2 position,
            Vector2 speed,
            SoundEffect pewSound,
            SoundEffect blastSound) : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.tex = tex;
            this.position = position;
            this.originalPosition = position;
            this.speed = speed;
            this.pewSound = pewSound;
            this.blastSound = blastSound;
            dimension = new Vector2(44, 44);
            createFrames();
        }

        private void createFrames()
        {
            frames = new List<Rectangle>();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int x = j * (int)dimension.X;
                    int y = i * (int)dimension.Y;
                    Rectangle r = new Rectangle(x, y, (int)dimension.X, (int)dimension.Y);
                    frames.Add(r);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (frameIndex < 6)
            {
                spriteBatch.Draw(tex, position, frames[frameIndex], Color.White);
            }
            else
            {
                if (lives > 0)
                {
                    lives--;
                    frameIndex = 0;
                    if (lives == 0)
                    {
                        Visible = false;
                    }
                    else
                    {
                        isInExplosion = false;
                        position = originalPosition;
                    }
                }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public event EventHandler FireBolt;
        public override void Update(GameTime gameTime)
        {
            if (isInExplosion)
            {
                frameIndex++;
            }
            else
            {
                KeyboardState ks = Keyboard.GetState();
                if (ks.IsKeyDown(Keys.Left))
                {
                    position -= speed;

                    if (position.X < 0)
                    {
                        position.X = 0;
                    }
                }
                if (ks.IsKeyDown(Keys.Right))
                {
                    position += speed;
                    if (position.X + dimension.X > Shared.stage.X)
                    {
                        position.X = Shared.stage.X - dimension.X;
                    }

                }
                if (ks.IsKeyDown(Keys.Space))
                {
                    if (delayCounter == 0)
                    {
                        pewSound.Play();
                        BoltPosition = position + new Vector2(dimension.X / 2, 0);
                        FireBolt.Invoke(this, EventArgs.Empty);
                        delayCounter = 30;
                    }
                    delayCounter--;
                }
                if (ks.IsKeyUp(Keys.Space))
                {
                    delayCounter = 0;
                }
            }

            base.Update(gameTime);
        }

        public Rectangle getBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, tex.Width / 3, tex.Height / 2);
        }

        public void Explode()
        {
            frameIndex = 1;
            isInExplosion = true;
            blastSound.Play();
        }
    }
}
