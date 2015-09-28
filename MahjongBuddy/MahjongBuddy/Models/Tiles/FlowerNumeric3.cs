using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class FlowerNumeric3 : Tile
    {
        public FlowerNumeric3()
        {
            Type = TileType.Flower;
            Status = TileStatus.Unrevealed;
            Value = TileValue.FlowerNumericThree;
            Owner = "board";
            Name = "FlowerNumericThree";
            Image = "/Content/images/tiles/64px/flower/flower3.png";
            ImageSmall = "/Content/images/tiles/50px/flower/flower3.png";
        }
    }
}