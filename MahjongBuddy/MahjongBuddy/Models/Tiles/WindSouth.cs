using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class WindSouth : Tile
    {
        public WindSouth()
        {
            Type = TileType.Wind;
            Status = TileStatus.Unrevealed;
            Value = TileValue.WindSouth;
            Owner = "board";
            Name = "WindSouth";
            Image = "/Content/images/tiles/64px/wind/wind-south.png";
            ImageSmall = "/Content/images/tiles/50px/wind/wind-south.png";
        }
    }
}