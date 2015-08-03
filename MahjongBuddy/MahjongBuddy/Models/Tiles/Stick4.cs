using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Stick4 : Tile
    {
        public Stick4()
        {
            Type = TileType.Stick;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Four;
            Owner = "board";
            Name = "StickFour";
            Image = "/Content/images/tiles/64px/bamboo/bamboo4.png";
        }
    }
}