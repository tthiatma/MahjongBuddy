using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Round1 : Tile
    {
        public Round1()
        {
            Type = TileType.Round;
            Status = TileStatus.Unrevealed;
            Value = TileValue.One;
            Owner = "board";
            Name = "RoundOne";
            Image = "/Content/images/tiles/64px/pin/pin1.png";
            ImageSmall = "/Content/images/tiles/50px/pin/pin1.png";
        }
    }
}