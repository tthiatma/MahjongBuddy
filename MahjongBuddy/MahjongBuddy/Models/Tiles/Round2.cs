using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Round2 : Tile
    {
        public Round2()
        {
            Type = TileType.Round;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Two;
            Owner = "board";
            Name = "RoundTwo";
            Image = "/Content/images/tiles/64px/pin/pin2.png";        
        }
    }
}