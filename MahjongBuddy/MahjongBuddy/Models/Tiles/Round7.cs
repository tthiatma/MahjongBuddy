using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Round7 : Tile
    {
        public Round7()
        {
            Type = TileType.Round;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Seven;
            Owner = "board";
            Name = "RoundSeven";
            Image = "/Content/images/tiles/64px/pin/pin7.png";
            ImageSmall = "/Content/images/tiles/50px/pin/pin7.png";
        }
    }
}