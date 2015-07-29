using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class WinningTileSet
    {
        public TileSet Eye { get; set; }
        public TileSet[] Sets { get; set; }

        public WinningTileSet() 
        {
            Sets = new TileSet[4];
            Eye = new TileSet();
            Eye.TileType = TileSetType.Eye;
        }
    }
}