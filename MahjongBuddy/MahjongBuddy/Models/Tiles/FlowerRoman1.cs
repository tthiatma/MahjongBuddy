using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class FlowerRoman1 : Tile
    {
        public FlowerRoman1()
        {
            Type = TileType.Flower;
            Status = TileStatus.Unrevealed;
            Value = TileValue.FlowerRomanOne;
            Owner = "board";
            Name = "FlowerRomanOne";
            Image = "/Content/images/tiles/64px/flower/flower1a.png";
        }
    }
}