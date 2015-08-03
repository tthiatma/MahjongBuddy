using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Round9 : Tile
    {
        public Round9()
        {
            Type = TileType.Round;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Nine;
            Owner = "board";
            Name = "RoundNine";
            Image = "/Content/images/tiles/64px/pin/pin9.png";
        }
    }
}