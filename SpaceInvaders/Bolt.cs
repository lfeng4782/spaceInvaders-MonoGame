
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceInvaders
{
    /// <summary>
    ///  a class of bolt
    /// </summary>
    public class Bolt : DrawableGameComponent
    {
        // declaring variables
        private SpriteBatch spriteBatch;
        private Texture2D tex;
        private Vector2 position;
        private Vector2 speed;
        private bool removable;

        public bool Removable { get => removable; set => removable = value; }
        /// <summary>
        /// constructor of Bolt
        /// </summary>
        /// <param name="game">this game</param>
        /// <param name="spriteBatch">spriteBatch</param>
        /// <param name="position">bolt postion</param>
        /// <param name="dimension">bolt dimension</param>
        /// <param name="color">bolt color</param>
        /// <param name="speed">bolt speed</param>
        public Bolt(Game game,
            SpriteBatch spriteBatch,           
            Vector2 position,
            Vector2 dimension,
            Color color,
            Vector2 speed) : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.tex = CreateTexture(spriteBatch.GraphicsDevice, (int)dimension.X, (int)dimension.Y, pixel => color);
            this.position = position;            
            this.speed = speed;
            removable = false;
        }
        /// <summary>
        /// method to create texture
        /// </summary>
        /// <param name="device">this device</param>
        /// <param name="width">tex width</param>
        /// <param name="height">height</param>
        /// <param name="paint">paint</param>
        /// <returns>tex</returns>
        private Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);

            return texture;
        }
        /// <summary>
        /// This is called when draw bolt.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(tex, position, Color.White);
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
            position += speed;
            if (position.Y < 0 || position.Y > Shared.stage.Y)
            {
                removable = true;
            }
            base.Update(gameTime);
        }

        public Rectangle getBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, tex.Width, tex.Height);
        }
    }
}
