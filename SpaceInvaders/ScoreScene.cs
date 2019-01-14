
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace SpaceInvaders
{
    /// <summary>
    /// a class of scorescene
    /// </summary>
    public class ScoreScene : GameScene
    {
        // delclaring variables
        private SpriteBatch spriteBatch;
        private Texture2D tex;
        private const int TOP_INIT = 10;
        private const int LEFT_INIT = 120;
        string scoreStr =
            "Name" + new string(' ', 12) + "Score\n";
        SpriteFont scoreFont;
        string scoreRecordStr = "";        
        Vector2 strDimensions;
        private List<ScoreRecord> highScoreList = new List<ScoreRecord>();
        /// <summary>
        /// ScoreScene constructor
        /// </summary>
        /// <param name="game">this game</param>
        /// <param name="spriteBatch">spriteBatch</param>
        public ScoreScene(Game game,
            SpriteBatch spriteBatch) : base(game)
        {
            this.spriteBatch = spriteBatch;
            scoreFont = game.Content.Load<SpriteFont>("Fonts/scorefont");
            tex = game.Content.Load<Texture2D>("Images/background");
            OpenRecord();
            strDimensions = scoreFont.MeasureString("1.    \n2.    ");
        }
        /// <summary>
        /// This is called when draw AboutScene.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(tex, Vector2.Zero, Color.White);
            spriteBatch.DrawString(scoreFont, scoreStr,
                new Vector2(LEFT_INIT, TOP_INIT), Color.Yellow);
            int counter = 0;
            scoreRecordStr = "";
            foreach (ScoreRecord scoreRecord in highScoreList)
            {
                counter++;
                scoreRecordStr += counter + "." +
                    new string(' ', 5 - counter.ToString().Length) +
                    scoreRecord.Name +
                    new string(' ', 16 - scoreRecord.Name.Length) +
                    scoreRecord.Score + "\n";
                if (counter >= 10)
                {
                    break;
                }
            }
            spriteBatch.DrawString(scoreFont, scoreRecordStr,
                new Vector2(LEFT_INIT - strDimensions.X, TOP_INIT + strDimensions.Y / 2), Color.White);
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
            base.Update(gameTime);
        }
        /// <summary>
        /// method to open records from file and store into a list
        /// </summary>
        private void OpenRecord()
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(".\\record.csv");
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] temp = line.Split(',');

                    highScoreList.Add(new ScoreRecord(temp[0], int.Parse(temp[1])));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);                
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
        /// <summary>
        /// method to add new record to list, then save to file
        /// </summary>
        /// <param name="newRecord"></param>
        public void AddRecord(ScoreRecord newRecord)
        {
            bool toSave = false;
            int index = 0;

            ScoreRecord record = highScoreList.Find(x => x.Name == newRecord.Name);

            if (record == null)
            {
                toSave = true;
            }
            else if (record.Score < newRecord.Score)
            {
                toSave = true;
                highScoreList.Remove(record);
            }
            //record = new ScoreRecord(name, score);
            if (toSave)
            {
                foreach (ScoreRecord scoreRecord in highScoreList)
                {
                    if (newRecord.Score > scoreRecord.Score)
                    {
                        break;
                    }
                    index++;
                }
                highScoreList.Insert(index, newRecord);

                StreamWriter writer = new StreamWriter(".\\record.csv");
                foreach (ScoreRecord scoreRecord in highScoreList)
                {
                    writer.WriteLine(scoreRecord.Name + "," + scoreRecord.Score);
                }

                writer.Close();
            }
        }
    }
}
