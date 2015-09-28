using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models.Tiles
{
    public class Money6 : Tile
    {
        public Money6()
        {
            Type = TileType.Money;
            Status = TileStatus.Unrevealed;
            Value = TileValue.Six;
            Owner = "board";
            Name = "MoneySix";
            Image = "/Content/images/tiles/64px/man/man6.png";
            ImageSmall = "/Content/images/tiles/50px/man/man6.png";
        }
    }
}