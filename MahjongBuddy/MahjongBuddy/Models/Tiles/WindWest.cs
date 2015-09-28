using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class WindWest : Tile
    {
        public WindWest()
        {
            Type = TileType.Wind;
            Status = TileStatus.Unrevealed;
            Value = TileValue.WindWest;
            Owner = "board";
            Name = "WindWest";
            Image = "/Content/images/tiles/64px/wind/wind-west.png";
            ImageSmall = "/Content/images/tiles/50px/wind/wind-west.png";
        }
    }
}