using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class FlowerNumeric1 : Tile
    {
        public FlowerNumeric1()
        {
            Type = TileType.Flower;
            Status = TileStatus.Unrevealed;
            Value = TileValue.FlowerNumericOne;
            Owner = "board";
            Name = "FlowerNumericOne";
            Image = "/Content/images/tiles/64px/flower/flower1.png";
        }
    }
}