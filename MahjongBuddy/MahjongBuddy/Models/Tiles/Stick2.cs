using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Stick2 : Tile
    {
        public Stick2()
        {
            Type = TileType.Stick;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Two;
            Owner = "board";
            Name = "StickTwo";
            Image = "/Content/images/tiles/64px/bamboo/bamboo2.png";
        }
    }
}