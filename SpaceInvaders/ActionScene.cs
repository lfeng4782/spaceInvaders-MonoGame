
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace SpaceInvaders
{
    /// <summary>
    /// a class of ActionScene
    /// </summary>
    public class ActionScene : GameScene
    {
        // declaring variables and constants
        private const int MAX_NUM_SHIP_LIVES = 3;
        private const int MAX_ALIEN_FIRE_DELAY_LEVEL1 = 5;
        private const int MAX_ALIEN_FIRE_DELAY_LEVEL2 = 4;
        private const int ALIEN_NUM_OF_COL = 4;
        private const int ALIEN_NUM_OF_ROW = 3;
        private const int ALIEN_H_SPEED = 10;
        private const int ALIEN_V_SPEED = 40;

        private const int ALIEN_INIT_TOP = 50;
        private const int ALIEN_H_GAP = 20;
        private const int ALIEN_V_GAP = 10;

        private const int SHIP_BOLT_SPEED_V = -8;
        private const int ALIEN_BOLT_SPEED_V = 8;
        private const int ROCKET_BOLT_SPEED_V = 10;

        private Random rand;
        private int alienFireCounter = 0;
        private int alienFireDelay = MAX_ALIEN_FIRE_DELAY_LEVEL1;

        private Game game;
        private SpriteBatch spriteBatch;
        private Texture2D tex;


        private Texture2D defenceLineTex;
        private SpriteFont scoreFont;

        private bool isPassed = false;
        private bool isGameOver = false;

        private int level = 0;
        private SpaceShip ship;
        private Alien[,] alienArray;
        CollisionManager cm;
        private KeyBoardInput keyBoardInput;
        private int inputFlagCounter;
        private string name = "";
        private Rocket rocket;
        private ScoreRecord newRecord = null;
        public ScoreRecord NewRecord { get => newRecord; set => newRecord = value; }
        public int Level { get => level; set => level = value; }
        public bool IsPassed { get; set; }

        private List<Bolt> newBoltList = new List<Bolt>();
        private List<Bolt> shipBoltList = new List<Bolt>();
        private List<Bolt> alienBoltList = new List<Bolt>();
        private List<Alien> alienList = new List<Alien>();
        private List<Alien> alienFireList = new List<Alien>();
        /// <summary>
        /// ActionScene constructor
        /// </summary>
        /// <param name="game">this game</param>
        /// <param name="spriteBatch">spriteBatch</param> /// <summary>        
        /// <param name="level">game level</param>
        public ActionScene(Game game,
            SpriteBatch spriteBatch,
            int level) : base(game)
        {            
            this.spriteBatch = spriteBatch;
            this.level = level;
            IsPassed = isPassed = false;

            Texture2D shipTex = game.Content.Load<Texture2D>("Images/ship-strip");
            SoundEffect shipPewSound = game.Content.Load<SoundEffect>("Sounds/shipFire");
            SoundEffect shipBlastSound = game.Content.Load<SoundEffect>("Sounds/blast1");
            tex= tex = game.Content.Load<Texture2D>("Images/background");
            Vector2 shipInitPos = new Vector2(Shared.stage.X / 2 - shipTex.Width / 3 / 2,
                Shared.stage.Y - shipTex.Height / 2);
            Vector2 shipSpeed = new Vector2(5, 0);
            ship = new SpaceShip(game, spriteBatch, shipTex, shipInitPos, shipSpeed,
                shipPewSound, shipBlastSound);
            ship.FireBolt += ShipFireBolt_Proc;
            ship.Lives = MAX_NUM_SHIP_LIVES;
            this.Components.Add(ship);

            rocket = null;
            Texture2D rocketTex;
            Vector2 alienInitPos;
            if (level == 2)
            {
                rocketTex = game.Content.Load<Texture2D>("Images/rocket");
                Texture2D rocketExplosionTex = game.Content.Load<Texture2D>("Images/explosion");
                SoundEffect rocketPewSound = game.Content.Load<SoundEffect>("Sounds/AlienFire");
                SoundEffect rocketBlastSound = game.Content.Load<SoundEffect>("Sounds/RocketExp");
                Vector2 rocketPosition = new Vector2(-Shared.stage.X / 2, ALIEN_INIT_TOP);
                rocket = new Rocket(game, spriteBatch, rocketTex, rocketExplosionTex, rocketPosition, rocketPewSound,
                    rocketBlastSound, 800);
                rocket.Scale = 0.2f;
                rocket.FireBolt += RocketFireBolt_Proc;
                this.Components.Add(rocket);

                alienFireDelay = MAX_ALIEN_FIRE_DELAY_LEVEL2;
                alienInitPos = new Vector2(ALIEN_H_GAP,
                    ALIEN_INIT_TOP + rocketTex.Height * rocket.Scale * 2);
            }
            else
            {
                alienFireDelay = MAX_ALIEN_FIRE_DELAY_LEVEL1;
                alienInitPos = new Vector2(ALIEN_H_GAP,
                    ALIEN_INIT_TOP);
            }
            CreateAliens(game, alienInitPos);

            cm = new CollisionManager(game, shipBoltList, alienBoltList, alienList, ship, rocket);
            if (level == 1)
            {
                CollisionManager.Score = 0;
            }

            this.Components.Add(cm);

            Song song = game.Content.Load<Song>("Sounds/chimes");
            MediaPlayer.IsRepeating = true;            

            defenceLineTex = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Int32[] pixel = { 0xFFFFFF }; // White. 0xFF is Red, 0xFF0000 is Blue
            defenceLineTex.SetData<Int32>(pixel, 0, defenceLineTex.Width * defenceLineTex.Height);

            scoreFont = game.Content.Load<SpriteFont>("Fonts/scorefont");

            rand = new Random();
            keyBoardInput = new KeyBoardInput();
        }
        /// <summary>
        /// ShipFireBolt event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShipFireBolt_Proc(object sender, System.EventArgs e)
        {
            SpaceShip ship = (SpaceShip)sender;
            Vector2 speed = new Vector2(0, SHIP_BOLT_SPEED_V);
            Vector2 boltDimension = new Vector2(4, 12);
            Vector2 boltPosition = ship.BoltPosition + new Vector2(-boltDimension.X / 2, -boltDimension.Y);
            Bolt bolt = new Bolt(game, spriteBatch, boltPosition, boltDimension, Color.White, speed);
            //this.Components.Add(bolt);
            newBoltList.Add(bolt);
            shipBoltList.Add(bolt);
        }
        /// <summary>
        /// RocketFireBolt event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RocketFireBolt_Proc(object sender, System.EventArgs e)
        {
            Rocket rocket = (Rocket)sender;
            Vector2 speed = new Vector2(0, ROCKET_BOLT_SPEED_V);
            Vector2 boltDimension = new Vector2(8, 12);
            Vector2 boltPosition = rocket.BoltPosition + new Vector2(-boltDimension.X / 2, 0);
            Bolt bolt = new Bolt(game, spriteBatch, rocket.BoltPosition, new Vector2(8, 12), Color.Red, speed);
            newBoltList.Add(bolt);
            alienBoltList.Add(bolt);
        }
        /// <summary>
        /// mothed to create aliens
        /// </summary>
        /// <param name="game">this game</param>
        /// <param name="alienInitPos">alien initial position</param>
        public void CreateAliens(Game game, Vector2 alienInitPos)
        {
            Vector2 hSep;
            Vector2 vSep;

            Alien.TotalNum = ALIEN_NUM_OF_ROW * ALIEN_NUM_OF_COL;
            Alien.UpdatedNum = 0;

            Texture2D alienATex = game.Content.Load<Texture2D>("Images/alien-strip3");
            Texture2D alienBTex = game.Content.Load<Texture2D>("Images/alien-strip2");
            Texture2D alienCTex = game.Content.Load<Texture2D>("Images/alien-strip1");
            SoundEffect alienPewSound = game.Content.Load<SoundEffect>("Sounds/AlienFire");
            SoundEffect alienPopSound = game.Content.Load<SoundEffect>("Sounds/pop2");

            hSep = new Vector2(ALIEN_H_GAP + alienATex.Width / 2, 0);
            vSep = new Vector2(0, ALIEN_V_GAP + alienATex.Height / 3);

            Alien.Speed = new Vector2(ALIEN_H_SPEED, ALIEN_V_SPEED);

            alienArray = new Alien[ALIEN_NUM_OF_ROW, ALIEN_NUM_OF_COL];
            int rowIdx = 0;

            // Create a row of Alien A
            for (int i = 0; i < ALIEN_NUM_OF_COL; i++)
            {
                alienArray[rowIdx, i] = new Alien(game, spriteBatch, alienATex, alienInitPos + hSep * i,
                    alienPewSound, alienPopSound, 400);
                alienArray[rowIdx, i].Row = rowIdx;
                alienArray[rowIdx, i].Col = i;
                alienArray[rowIdx, i].ExplosionDone += AlienExplosionDone_Proc;
                this.Components.Add(alienArray[rowIdx, i]);
                alienList.Add(alienArray[rowIdx, i]);
            }
            rowIdx++;

            // Create (ALIEN_NUM_OF_ROW-1)/2 row(s) of Alien B
            for (; rowIdx < (ALIEN_NUM_OF_ROW + 1) / 2; rowIdx++)
            {
                alienInitPos += vSep;
                for (int i = 0; i < ALIEN_NUM_OF_COL; i++)
                {
                    alienArray[rowIdx, i] = new Alien(game, spriteBatch, alienBTex, alienInitPos + hSep * i,
                        alienPewSound, alienPopSound, 200);
                    alienArray[rowIdx, i].Row = rowIdx;
                    alienArray[rowIdx, i].Col = i;
                    alienArray[rowIdx, i].ExplosionDone += AlienExplosionDone_Proc;
                    this.Components.Add(alienArray[rowIdx, i]);
                    alienList.Add(alienArray[rowIdx, i]);
                }
            }

            // Create row(s) of Alien C
            for (; rowIdx < ALIEN_NUM_OF_ROW; rowIdx++)
            {
                alienInitPos += vSep;
                for (int i = 0; i < ALIEN_NUM_OF_COL; i++)
                {
                    alienArray[rowIdx, i] = new Alien(game, spriteBatch, alienCTex, alienInitPos + hSep * i,
                        alienPewSound, alienPopSound, 100);
                    alienArray[rowIdx, i].Row = rowIdx;
                    alienArray[rowIdx, i].Col = i;
                    alienArray[rowIdx, i].ExplosionDone += AlienExplosionDone_Proc;
                    this.Components.Add(alienArray[rowIdx, i]);
                    alienList.Add(alienArray[rowIdx, i]);

                }
            }

            // Add the row at the bottom to the firelist
            for (int i = 0; i < ALIEN_NUM_OF_COL; i++)
            {
                alienFireList.Add(alienArray[ALIEN_NUM_OF_ROW - 1, i]);
            }
        }

        /// <summary>
        ///  AlienExplosionDone event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlienExplosionDone_Proc(object sender, System.EventArgs e)
        {
            Alien alien = (Alien)sender;
            if (alienFireList.Contains(alien))
            {
                int row = alien.Row;
                int col = alien.Col;
                alienFireList.Remove(alien);
                while (--row >= 0)
                {
                    Alien alienNew = GetAlien(row, col);
                    if (alienNew != null)
                    {
                        alienFireList.Add(alienNew);
                        break;
                    }
                }
            }
            if (alienList.Count == 0)
            {
                alienFireDelay--;
                if (level == 1)
                {
                    if ((alienFireDelay <= MAX_ALIEN_FIRE_DELAY_LEVEL2) && !isGameOver)
                    {
                        isPassed = true;
                        foreach (Bolt bolt in alienBoltList)
                        {
                            bolt.Removable = true;
                            bolt.Enabled = false;
                            bolt.Visible = false;
                        }
                    }
                }
                else
                {
                    if (alienFireDelay < 0)
                    {
                        alienFireDelay = 0;
                    }
                }

                if (!(isPassed || isGameOver))
                {
                    Alien.TotalNum = ALIEN_NUM_OF_ROW * ALIEN_NUM_OF_COL;
                    Alien.UpdatedNum = 0;
                    Alien.Speed = new Vector2(ALIEN_H_SPEED, ALIEN_H_SPEED); ;
                    for (int i = 0; i < ALIEN_NUM_OF_ROW; i++)
                    {
                        for (int j = 0; j < ALIEN_NUM_OF_COL; j++)
                        {
                            alienArray[i, j].Reset();
                            alienList.Add(alienArray[i, j]);
                        }
                    }

                    for (int j = 0; j < ALIEN_NUM_OF_COL; j++)
                    {
                        alienFireList.Add(alienArray[ALIEN_NUM_OF_ROW - 1, j]);
                    }
                    alienFireCounter = 0;
                }

            }
        }
        /// <summary>
        /// get alien mothed
        /// </summary>
        /// <param name="row">alien row</param>
        /// <param name="col">alien column</param>
        /// <returns>alien</returns>
        private Alien GetAlien(int row, int col)
        {
            if ((row >= 0) && (row < ALIEN_NUM_OF_ROW) && (col >= 0) && (col < ALIEN_NUM_OF_COL) &&
                (alienArray[row, col].Removable == false))
            {
                return alienArray[row, col];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// This is called when draw ActionScene.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(tex, Vector2.Zero, Color.White);
            spriteBatch.Draw(defenceLineTex, new Rectangle(0, (int)(Shared.stage.Y - 64), (int)Shared.stage.X, 1), Color.White);

            string scoreString = "Score: " + CollisionManager.Score.ToString();
            Vector2 position = new Vector2(5, 10);
            spriteBatch.DrawString(scoreFont, scoreString, position, Color.Yellow);

            string levelString = "Level - " + level.ToString();
            Vector2 stringDimensions = scoreFont.MeasureString(levelString);
            position = new Vector2((Shared.stage.X - stringDimensions.X) / 2, 10);
            spriteBatch.DrawString(scoreFont, levelString, position, Color.Yellow);

            string livesString = "Lives: " + ship.Lives.ToString();
            stringDimensions = scoreFont.MeasureString(livesString);
            position = new Vector2(Shared.stage.X - stringDimensions.X, 10);
            spriteBatch.DrawString(scoreFont, livesString, position, Color.Yellow);

            if (isPassed)
            {
                DisplayPass();
            }
            else if (isGameOver)
            {
                DisplayGameOver();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        /// <summary>
        /// method to display the screen of game over
        /// </summary>
        private void DisplayGameOver()
        {
            string gameOverString = "Game Over!\n" +
                "Enter Your Name:";

            Vector2 stringDimensions = scoreFont.MeasureString(gameOverString);
            Vector2 position = new Vector2((Shared.stage.X - stringDimensions.X) / 2,
                (Shared.stage.Y - stringDimensions.Y) / 2);
            spriteBatch.DrawString(scoreFont, gameOverString, position, Color.Yellow);

            String nameString = name;
            if ((inputFlagCounter & 0x1f) > 0x10)
            {
                nameString += "|";
            }
            inputFlagCounter++;
            inputFlagCounter &= 0xff;
            position = new Vector2((int)((Shared.stage.X - stringDimensions.X) / 2),
                (int)((Shared.stage.Y - stringDimensions.Y) / 2 + stringDimensions.Y));
            spriteBatch.DrawString(scoreFont, nameString, position, Color.Yellow);
        }
        /// <summary>
        /// method to display the screen of game passed
        /// </summary>
        private void DisplayPass()
        {
            string passString = "Congratulations! You passed level" + level.ToString() + "\n" +
                "Press <N> to continue next level";

            Vector2 stringDimensions = scoreFont.MeasureString(passString);
            Vector2 position = new Vector2((Shared.stage.X - stringDimensions.X) / 2,
                (Shared.stage.Y - stringDimensions.Y) / 2);
            spriteBatch.DrawString(scoreFont, passString, position, Color.Yellow);
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// When the left button of mouse is clicked, invoke an event.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {
            if (Alien.AllMovedAStep())
            {
                // alien fires bolts
                if ((alienFireList.Count > 0) && !isGameOver)
                {
                    if (++alienFireCounter >= alienFireDelay)
                    {
                        int posToFire = rand.Next(alienFireList.Count);
                        Bolt bolt = alienFireList[posToFire].FireBolt(new Vector2(0, ALIEN_BOLT_SPEED_V));
                        alienBoltList.Add(bolt);
                        this.Components.Add(bolt);
                        alienFireCounter = 0;
                    }
                }
            }

            if (isGameOver)
            {
                // if game is over, get player name from keyboard
                bool isInputDone = keyBoardInput.CheckInputState();
                name = keyBoardInput.Text;
                if (isInputDone)
                {
                    newRecord = new ScoreRecord(name.Trim(), CollisionManager.Score);
                    keyBoardInput.Text = "";
                    name = "";
                }
            }
            else
            {
                // if ship's lives becomes 0 or aliens cross the defense line, game over
                if ((ship.Lives == 0 || Alien.CrossedLine) && !IsPassed)
                {
                    isGameOver = true;
                    foreach (Alien alien in alienList)
                    {
                        alien.Enabled = false;
                        alien.Visible = false;
                    }
                    foreach (Bolt bolt in alienBoltList)
                    {
                        bolt.Removable = true;
                        bolt.Enabled = false;
                        bolt.Visible = false;
                    }
                    foreach (Bolt bolt in shipBoltList)
                    {
                        bolt.Removable = true;
                        bolt.Enabled = false;
                        bolt.Visible = false;
                    }
                    if (rocket != null)
                    {
                        rocket.Enabled = false;
                        rocket.Visible = false;
                    }
                }
            }

            if (isPassed)
            {
                KeyboardState ks = Keyboard.GetState();
                if (ks.IsKeyDown(Keys.N))
                {
                    IsPassed = true;
                }
            }

            UpdateAllLists();
            base.Update(gameTime);
        }
        /// <summary>
        /// UpdateallLists method
        /// </summary>
        private void UpdateAllLists()
        {
            foreach (Bolt bolt in newBoltList)
            {
               this.Components.Add(bolt);
            }
            newBoltList.Clear();

            foreach (Bolt bolt in shipBoltList)
            {
                if (bolt.Removable)
                {
                    this.Components.Remove(bolt);
                }
            }
            foreach (Bolt bolt in alienBoltList)
            {
                if (bolt.Removable)
                {
                    this.Components.Remove(bolt);
                }
            }
            shipBoltList.RemoveAll(bolt => bolt.Removable == true);
            alienBoltList.RemoveAll(bolt => bolt.Removable == true);
            alienList.RemoveAll(alien => alien.Removable == true);
        }
    }
}
