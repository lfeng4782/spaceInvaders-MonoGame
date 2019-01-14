
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    /// <summary>
    /// a class of ScoreRecord
    /// </summary>
    public class ScoreRecord
    {
        // declaring variables
        private string name;
        private int score;

        public String Name { get => name; set => name = value; }
        public int Score { get => score; set => score = value; }
        /// <summary>
        /// ScoreRecord constructor
        /// </summary>
        /// <param name="name">name </param>
        /// <param name="score">score</param>
        public ScoreRecord(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }
}
