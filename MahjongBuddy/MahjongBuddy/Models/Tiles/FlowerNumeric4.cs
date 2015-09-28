using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class FlowerNumeric4 : Tile
    {
        public FlowerNumeric4()
        {
            Type = TileType.Flower;
            Status = TileStatus.Unrevealed;
            Value = TileValue.FlowerNumericFour;
            Owner = "board";
            Name = "FlowerNumericFour";
            Image = "/Content/images/tiles/64px/flower/flower4.png";
            ImageSmall = "/Content/images/tiles/50px/flower/flower4.png";
        }
    }
}