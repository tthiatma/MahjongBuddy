using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Money3 : Tile
    {
        public Money3()
        {
            Type = TileType.Money;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Three;
            Owner = "board";
            Name = "MoneyThree";
            Image = "/Content/images/tiles/64px/man/man3.png";
        }
    }
}