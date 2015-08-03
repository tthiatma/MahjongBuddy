using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Money8 : Tile
    {
        public Money8()
        {
            Type = TileType.Money;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Eight;
            Owner = "board";
            Name = "MoneyEight";
            Image = "/Content/images/tiles/64px/man/man8.png";
        }
    }
}