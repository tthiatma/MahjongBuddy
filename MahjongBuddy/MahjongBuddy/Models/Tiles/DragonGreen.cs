using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class DragonGreen : Tile
    {
        public DragonGreen()
        {
            Type = TileType.Dragon;
            Status = TileStatus.Unrevealed;
            Value = TileValue.DragonGreen;
            Owner = "board";
            Name = "DragonGreen";
            Image = "/Content/images/tiles/64px/dragon/dragon-green.png";
        }
    }
}