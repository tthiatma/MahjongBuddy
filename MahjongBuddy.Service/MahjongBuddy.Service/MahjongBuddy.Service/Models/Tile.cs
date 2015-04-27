using MahjongBuddy.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Service.Models
{
    public class Tile
    {
        public int BaseTileCount {get{return 4;}}
        public ITile TileType { get; set; }
        public string TileImagePath { get; set; }
    }
}