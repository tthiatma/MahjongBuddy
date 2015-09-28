using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Round6 : Tile
    {
        public Round6()
        {
            Type = TileType.Round;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Six;
            Owner = "board";
            Name = "RoundSix";
            Image = "/Content/images/tiles/64px/pin/pin6.png";
            ImageSmall = "/Content/images/tiles/50px/pin/pin6.png";
        }
    }
}