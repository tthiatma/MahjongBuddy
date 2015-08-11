using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class TileSet
    {
        public IEnumerable<Tile> Tiles { get; set; }
        public TileSetType TileType { get; set; }
        public bool isRevealed { get; set; }
    }

    public enum TileSetType
    { 
        Chow,
        Pong,
        Kong,
        Eye
    }
}