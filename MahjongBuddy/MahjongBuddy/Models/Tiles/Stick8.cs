using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Stick8 : Tile
    {
        public Stick8()
        {
            Type = TileType.Stick;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Eight;
            Owner = "board";
            Name = "StickEight";
            Image = "/Content/images/tiles/64px/bamboo/bamboo8.png";
            ImageSmall = "/Content/images/tiles/50px/bamboo/bamboo8.png";
        }
    }
}