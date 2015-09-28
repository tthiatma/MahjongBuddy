using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Stick1 : Tile
    {
        public Stick1()
        {
            Type = TileType.Stick;
            Status = TileStatus.Unrevealed;
            Value = TileValue.One;
            Owner = "board";
            Name = "StickOne";
            Image = "/Content/images/tiles/64px/bamboo/bamboo1.png";
            ImageSmall = "/Content/images/tiles/50px/bamboo/bamboo1.png";
        }
    }
}