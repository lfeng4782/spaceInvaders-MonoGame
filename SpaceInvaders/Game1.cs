
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace SpaceInvaders
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        // declares all scenes here
        private StartScene startScene;
        private ActionScene actionScene;
        private HelpScene helpScene;
        private ScoreScene scoreScene;
        private AboutScene aboutScene;

        // scene declaration ends
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Shared.stage = new Vector2(graphics.PreferredBackBufferWidth,
               graphics.PreferredBackBufferHeight);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // instantiate all scenes here
            startScene = new StartScene(this, spriteBatch);
            this.Components.Add(startScene);

            actionScene = null;
            //actionScene = new ActionScene(this, spriteBatch);
            //this.Components.Add(actionScene);

            helpScene = new HelpScene(this, spriteBatch);
            this.Components.Add(helpScene);

            scoreScene = new ScoreScene(this, spriteBatch);
            this.Components.Add(scoreScene);

            aboutScene = new AboutScene(this, spriteBatch);
            this.Components.Add(aboutScene);
            // instantiation ends

            //make only startscene active
            startScene.show();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // TODO: Add your update logic here
            KeyboardState ks = Keyboard.GetState();
            StartScene.GameMenueEnum selectedIndex;

            if (startScene.Enabled)
            {
                selectedIndex = (StartScene.GameMenueEnum)startScene.Menu.SelectedIndex;
                if (ks.IsKeyDown(Keys.Enter))
                {
                    startScene.hide();
                    switch (selectedIndex)
                    {
                        case StartScene.GameMenueEnum.PlayGame:
                            if (actionScene != null)
                            {
                                this.Components.Remove(actionScene);
                            }
                            actionScene = new ActionScene(this, spriteBatch, 1);
                            this.Components.Add(actionScene);
                            actionScene.show();
                            break;
                        case StartScene.GameMenueEnum.Help:
                            helpScene.show();
                            break;
                        case StartScene.GameMenueEnum.HighScores:
                            scoreScene.show();
                            break;
                        case StartScene.GameMenueEnum.About:
                            aboutScene.show();
                            break;
                        case StartScene.GameMenueEnum.Quit:
                            Exit();
                            break;
                        default:
                            break;
                    }
                }
            }

            if ((actionScene != null && actionScene.Enabled) || helpScene.Enabled || aboutScene.Enabled || scoreScene.Enabled)
            {
                if (ks.IsKeyDown(Keys.Escape))
                {
                    hideAllScenes();
                    startScene.show();
                }
            }

            if (actionScene != null && actionScene.Enabled)
            {
                if (actionScene.NewRecord != null)
                {
                    scoreScene.AddRecord(actionScene.NewRecord);
                    actionScene.NewRecord = null;
                    actionScene.hide();
                    scoreScene.show();
                }
                if (actionScene.IsPassed)
                {
                    this.Components.Remove(actionScene);
                    actionScene = new ActionScene(this, spriteBatch, 2);
                    this.Components.Add(actionScene);
                    actionScene.show();
                }
            }
            base.Update(gameTime);
        }
        /// <summary>
        /// method to hide all scenes
        /// </summary>
        private void hideAllScenes()
        {
            foreach (GameScene item in Components)
            {
                item.hide();
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
