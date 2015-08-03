using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Money1 : Tile
    {
        public Money1()
        {
            Type = TileType.Money;
            Status = TileStatus.Unrevealed;
            Value = TileValue.One;
            Owner = "board";
            Name = "MoneyOne";
            Image = "/Content/images/tiles/64px/man/man1.png";
        }
    }
}