using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class FlowerRoman4 : Tile
    {
        public FlowerRoman4()
        {
            Type = TileType.Flower;
            Status = TileStatus.Unrevealed;
            Value = TileValue.FlowerRomanFour;
            Owner = "board";
            Name = "FlowerRomanFour";
            Image = "/Content/images/tiles/64px/flower/flower4d.png";
        }
    }
}