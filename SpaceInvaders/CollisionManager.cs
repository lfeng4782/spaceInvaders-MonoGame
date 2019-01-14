
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
    /// a class of collisionManager
    /// </summary>
    public class CollisionManager : GameComponent
    {       
        private List<Bolt> shipBoltList;
        private List<Bolt> alienBoltList;
        private List<Alien> alienList;       
        private Game game;
        private SpaceShip ship;
        private Rocket rocket;
        private static int score = 0;

        public static int Score { get => score; set => score = value; }

        /// <summary>
        /// CollisionManager constructor
        /// </summary>
        /// <param name="game">this game</param>
        /// <param name="shipBoltList">shipBoltList</param>
        /// <param name="alienBoltList">alienBoltList</param>
        /// <param name="alienList">alienList</param>
        /// <param name="ship">ship</param>
        /// <param name="rocket">rocket</param>
        public CollisionManager(Game game,          
            List<Bolt> shipBoltList,
            List<Bolt> alienBoltList,
            List<Alien> alienList,
            SpaceShip ship,
            Rocket rocket) : base(game)
        {                    
            this.shipBoltList = shipBoltList;
            this.alienBoltList = alienBoltList;
            this.alienList = alienList;
            this.ship = ship;
            this.rocket = rocket;
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// When the left button of mouse is clicked, invoke an event.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {
            foreach (Bolt bolt in shipBoltList)
            {
                Rectangle boltRect = bolt.getBounds();
                foreach (Alien alien in alienList)
                {
                    Rectangle alienRect = alien.getBounds();
                    if (alienRect.Intersects(boltRect))
                    {
                        alien.Explode();
                        score += alien.Points;
                        alien.Removable = true;
                        bolt.Removable = true;
                        break;
                    }
                }

                if (rocket != null)
                {
                    Rectangle roketRect = rocket.getBounds();

                    if (!rocket.IsInExplosion && roketRect.Intersects(boltRect))
                    {
                        rocket.Explode();
                        score += rocket.Points;
                        bolt.Removable = true;
                        break;
                    }
                }

            }
            foreach (Bolt bolt in alienBoltList)
            {
                Rectangle boltRect = bolt.getBounds();
                Rectangle shipRect = ship.getBounds();

                if (!ship.IsInExplosion && shipRect.Intersects(boltRect))
                {
                    ship.Explode();
                    bolt.Removable = true;
                    break;
                }
            }

            base.Update(gameTime);
        }
    }
}
