using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Money5 : Tile
    {
        public Money5()
        {
            Type = TileType.Money;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Five;
            Owner = "board";
            Name = "MoneyFive";
            Image = "/Content/images/tiles/64px/man/man5.png";
        }
    }
}