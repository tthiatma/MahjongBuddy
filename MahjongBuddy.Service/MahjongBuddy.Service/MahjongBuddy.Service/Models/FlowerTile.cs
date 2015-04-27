using MahjongBuddy.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Service.Models
{
    public class FlowerTile : ITile
    {
        public int TileTypeCount { get { return 2; } }
    }
    
    enum FlowerTileType
    {
        Roman,
        Numeric
    }
}