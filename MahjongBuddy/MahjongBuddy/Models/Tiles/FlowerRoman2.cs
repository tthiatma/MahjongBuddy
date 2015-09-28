using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class FlowerRoman2 : Tile
    {
        public FlowerRoman2()
        {
            Type = TileType.Flower;
            Status = TileStatus.Unrevealed;
            Value = TileValue.FlowerRomanTwo;
            Owner = "board";
            Name = "FlowerRomanTwo";
            Image = "/Content/images/tiles/64px/flower/flower2b.png";
            ImageSmall = "/Content/images/tiles/50px/flower/flower2b.png";
        }
    }
}