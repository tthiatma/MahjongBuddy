using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class ActivePlayer : Player
    {
        public List<Tile> ActiveTiles { get; set; }

        public ActivePlayer(string name, string group, string hash)
            : base(name, group, hash)
        {
            ActiveTiles = new List<Tile>();
        }
    }
}