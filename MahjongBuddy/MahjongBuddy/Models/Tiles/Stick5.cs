using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Stick5 : Tile
    {
        public Stick5()
        {
            Type = TileType.Stick;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Five;
            Owner = "board";
            Name = "StickFive";
            Image = "/Content/images/tiles/64px/bamboo/bamboo5.png";
            ImageSmall = "/Content/images/tiles/50px/bamboo/bamboo5.png";
        }
    }
}