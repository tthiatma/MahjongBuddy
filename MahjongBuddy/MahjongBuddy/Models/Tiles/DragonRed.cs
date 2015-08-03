using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class DragonRed : Tile
    {
        public DragonRed()
        {
            Type = TileType.Dragon;
            Status = TileStatus.Unrevealed;
            Value = TileValue.DragonRed;
            Owner = "board";
            Name = "DragonRed";
            Image = "/Content/images/tiles/64px/dragon/dragon-chun.png";
        }
    }
}