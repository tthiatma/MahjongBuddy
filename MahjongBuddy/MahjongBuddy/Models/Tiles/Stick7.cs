using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Stick7 : Tile
    {
        public Stick7()
        {
            Type = TileType.Stick;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Seven;
            Owner = "board";
            Name = "StickSeven";
            Image = "/Content/images/tiles/64px/bamboo/bamboo7.png";
            ImageSmall = "/Content/images/tiles/50px/bamboo/bamboo7.png";
        }
    }
}