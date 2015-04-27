using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class Game
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player Player3 { get; set; }
        public Player Player4 { get; set; }

        public Board Board { get; set; }
        public Tile LastTile { get; set; }
        public string WhosTurn { get; set; }
        public int GameCount { get; set; }

        public WindDirection CurrentWind { get; set; }
        public string WhoStart { get; set; }
    }
}