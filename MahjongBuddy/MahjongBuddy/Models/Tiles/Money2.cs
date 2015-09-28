using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Money2 : Tile
    {
        public Money2()
        {
            Type = TileType.Money;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Two;
            Owner = "board";
            Name = "MoneyTwo";
            Image = "/Content/images/tiles/64px/man/man2.png";
            ImageSmall = "/Content/images/tiles/50px/man/man2.png";
        }
    }
}