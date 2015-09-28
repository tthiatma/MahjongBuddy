using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Money9 : Tile
    {
        public Money9()
        {
            Type = TileType.Money;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Nine;
            Owner = "board";
            Name = "MoneyNine";
            Image = "/Content/images/tiles/64px/man/man9.png";
            ImageSmall = "/Content/images/tiles/50px/man/man9.png";
        }
    }
}