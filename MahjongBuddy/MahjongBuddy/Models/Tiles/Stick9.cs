using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Stick9 : Tile
    {
        public Stick9()
        {
            Type = TileType.Stick;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Nine;
            Owner = "board";
            Name = "StickNine";
            Image = "/Content/images/tiles/64px/bamboo/bamboo9.png";
            ImageSmall = "/Content/images/tiles/50px/bamboo/bamboo9.png";
        }
    }
}