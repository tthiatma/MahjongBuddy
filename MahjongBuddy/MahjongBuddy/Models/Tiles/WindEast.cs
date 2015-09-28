using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class WindEast : Tile
    {
        public WindEast()
        {
            Type = TileType.Wind;
            Status = TileStatus.Unrevealed;
            Value = TileValue.WindEast;
            Owner = "board";
            Name = "WindEast";
            Image = "/Content/images/tiles/64px/wind/wind-east.png";
            ImageSmall = "/Content/images/tiles/50px/wind/wind-east.png";
        }
    }
}