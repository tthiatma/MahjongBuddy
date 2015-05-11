using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class WinningTileSet
    {
        public List<OpenTileSet> OpenedTiles { get; set; }
        public IEnumerable<Tile> ClosedTiles { get; set; }
        public WinningTileSet() 
        {
            OpenedTiles = new List<OpenTileSet>();
        }
    }
}