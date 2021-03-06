﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class WinningTileSet
    {
        public TileSet Eye { get; set; }
        public TileSet[] Sets { get; set; }
        public List<Tile> Flowers { get; set; }
        public List<HandWorth> Hands { get; set; }
        public Dictionary<String, int> WinningTypes { get; set; }
        public int TotalPoints { get; set; }

        public WinningTileSet() 
        {
            Sets = new TileSet[4];
            Flowers = new List<Tile>();
            Eye = new TileSet();
            Eye.TileSetType = TileSetType.Eye;
            Hands = new List<HandWorth>();
            WinningTypes = new Dictionary<String, int>();
        }
    }
}