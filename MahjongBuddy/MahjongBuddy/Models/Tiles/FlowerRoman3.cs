using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class FlowerRoman3 : Tile
    {
        public FlowerRoman3()
        {
            Type = TileType.Flower;
            Status = TileStatus.Unrevealed;
            Value = TileValue.FlowerRomanThree;
            Owner = "board";
            Name = "FlowerRomanThree";
            Image = "/Content/images/tiles/64px/flower/flower3c.png";
        }
    }
}