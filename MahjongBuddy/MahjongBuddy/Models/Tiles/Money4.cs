using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Money4 : Tile
    {
        public Money4()
        {
            Type = TileType.Money;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Four;
            Owner = "board";
            Name = "MoneyFour";
            Image = "/Content/images/tiles/64px/man/man4.png";
            ImageSmall = "/Content/images/tiles/50px/man/man4.png";
        }
    }
}