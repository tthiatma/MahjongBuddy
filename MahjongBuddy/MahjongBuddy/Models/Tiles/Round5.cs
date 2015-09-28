using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Round5 : Tile
    {
        public Round5()
        {
            Type = TileType.Round;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Five;
            Owner = "board";
            Name = "RoundFive";
            Image = "/Content/images/tiles/64px/pin/pin5.png";
            ImageSmall = "/Content/images/tiles/50px/pin/pin5.png";
        }
    }
}