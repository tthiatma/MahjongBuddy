using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class FlowerNumeric2 : Tile
    {
        public FlowerNumeric2()
        {
            Type = TileType.Flower;
            Status = TileStatus.Unrevealed;
            Value = TileValue.FlowerNumericTwo;
            Owner = "board";
            Name = "FlowerNumericTwo";
            Image = "/Content/images/tiles/64px/flower/flower2.png";
        }
    }
}