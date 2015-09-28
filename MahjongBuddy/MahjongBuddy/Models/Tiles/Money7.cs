using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Money7 : Tile
    {
        public Money7()
        {
            Type = TileType.Money;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Seven;
            Owner = "board";
            Name = "MoneySeven";
            Image = "/Content/images/tiles/64px/man/man7.png";
            ImageSmall = "/Content/images/tiles/50px/man/man7.png";
        }
    }
}