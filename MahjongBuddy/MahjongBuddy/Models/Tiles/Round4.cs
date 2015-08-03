using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Round4 : Tile
    {
        public Round4()
        {
            Type = TileType.Round;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Four;
            Owner = "board";
            Name = "RoundFour";
            Image = "/Content/images/tiles/64px/pin/pin4.png";
        }
    }
}