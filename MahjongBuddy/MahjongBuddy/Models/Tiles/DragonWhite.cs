using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class DragonWhite: Tile
    {
        public DragonWhite()
        {
            Type = TileType.Dragon;
            Status = TileStatus.Unrevealed;
            Value = TileValue.DragonWhite;
            Owner = "board";
            Name = "DragonWhite";
            Image = "/Content/images/tiles/64px/dragon/dragon-white.png";
        }
    }
}