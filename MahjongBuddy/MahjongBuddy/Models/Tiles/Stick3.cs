using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Stick3 : Tile
    {
        public Stick3()
        {
            Type = TileType.Stick;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Three;
            Owner = "board";
            Name = "StickThree";
            Image = "/Content/images/tiles/64px/bamboo/bamboo3.png";
            ImageSmall = "/Content/images/tiles/50px/bamboo/bamboo3.png";
        }
    }
}