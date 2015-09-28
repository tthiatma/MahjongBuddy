using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Stick6 : Tile
    {
        public Stick6()
        {
            Type = TileType.Stick;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Six;
            Owner = "board";
            Name = "StickSix";
            Image = "/Content/images/tiles/64px/bamboo/bamboo6.png";
            ImageSmall = "/Content/images/tiles/50px/bamboo/bamboo6.png";
        }
    }
}