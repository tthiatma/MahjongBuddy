using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class WindNorth: Tile
    {
        public WindNorth()
        {
            Type = TileType.Wind;
            Status = TileStatus.Unrevealed;
            Value = TileValue.WindNorth;
            Owner = "board";
            Name = "WindNorth";
            Image = "/Content/images/tiles/64px/wind/wind-north.png";
        }
    }
}