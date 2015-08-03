using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Round3 : Tile
    {
        public Round3()
        {
            Type = TileType.Round;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Three;
            Owner = "board";
            Name = "RoundThree";
            Image = "/Content/images/tiles/64px/pin/pin3.png";
        }
    }
}