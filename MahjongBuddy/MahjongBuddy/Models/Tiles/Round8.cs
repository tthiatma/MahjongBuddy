using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Round8 : Tile
    {
        public Round8()
        {
            Type = TileType.Round;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Eight;
            Owner = "board";
            Name = "RoundEight";
            Image = "/Content/images/tiles/64px/pin/pin8.png";
        }
    }
}