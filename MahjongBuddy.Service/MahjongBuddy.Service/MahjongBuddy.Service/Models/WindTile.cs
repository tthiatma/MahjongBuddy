using MahjongBuddy.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Service.Models
{
    public class WindTile : ITile
    {
        public int TileTypeCount { get { return 4; } }
    }
    enum WindTileType
    {
        East,
        South,
        West,
        North
    }
}