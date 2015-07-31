using MahjongBuddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy
{
    public static class TileBuilder
    {
        public static List<Tile> BuildAllStraightTiles()
        {
            List<Tile> tiles = new List<Tile>();
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.One, Owner = "p1", Name = "1MoneyOne", Image = "/Content/images/tiles/64px/man/man1.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Two, Owner = "p1", Name = "1MoneyTwo", Image = "/Content/images/tiles/64px/man/man2.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Three, Owner = "p1", Name = "1MoneyThree", Image = "/Content/images/tiles/64px/man/man3.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Four, Owner = "p1", Name = "1MoneyFour", Image = "/Content/images/tiles/64px/man/man4.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Five, Owner = "p1", Name = "1MoneyFive", Image = "/Content/images/tiles/64px/man/man5.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Six, Owner = "p1", Name = "1MoneySix", Image = "/Content/images/tiles/64px/man/man6.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Seven, Owner = "p1", Name = "1MoneySeven", Image = "/Content/images/tiles/64px/man/man7.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Eight, Owner = "p1", Name = "1MoneyEight", Image = "/Content/images/tiles/64px/man/man8.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Nine, Owner = "p1", Name = "1MoneyNine", Image = "/Content/images/tiles/64px/man/man9.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.One, Owner = "p1", Name = "1RoundOne", Image = "/Content/images/tiles/64px/pin/pin1.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Two, Owner = "p1", Name = "1RoundTwo", Image = "/Content/images/tiles/64px/pin/pin2.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Three, Owner = "p1", Name = "1RoundThree", Image = "/Content/images/tiles/64px/pin/pin3.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Four, Owner = "p1", Name = "1RoundFour", Image = "/Content/images/tiles/64px/pin/pin4.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Four, Owner = "p1", Name = "2RoundFour", Image = "/Content/images/tiles/64px/pin/pin4.png" });
            return tiles;
        }

        public static List<Tile> BuildNotAllStraightTiles()
        {
            List<Tile> tiles = new List<Tile>();
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.One, Owner = "p1", Name = "1MoneyOne", Image = "/Content/images/tiles/64px/man/man1.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Two, Owner = "p1", Name = "1MoneyTwo", Image = "/Content/images/tiles/64px/man/man2.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Three, Owner = "p1", Name = "1MoneyThree", Image = "/Content/images/tiles/64px/man/man3.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Four, Owner = "p1", Name = "1MoneyFour", Image = "/Content/images/tiles/64px/man/man4.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Five, Owner = "p1", Name = "1MoneyFive", Image = "/Content/images/tiles/64px/man/man5.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Six, Owner = "p1", Name = "1MoneySix", Image = "/Content/images/tiles/64px/man/man6.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Seven, Owner = "p1", Name = "1MoneySeven", Image = "/Content/images/tiles/64px/man/man7.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Eight, Owner = "p1", Name = "1MoneyEight", Image = "/Content/images/tiles/64px/man/man8.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Nine, Owner = "p1", Name = "1MoneyNine", Image = "/Content/images/tiles/64px/man/man9.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.One, Owner = "p1", Name = "1RoundOne", Image = "/Content/images/tiles/64px/pin/pin1.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Two, Owner = "p1", Name = "1RoundTwo", Image = "/Content/images/tiles/64px/pin/pin2.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Three, Owner = "p1", Name = "1RoundThree", Image = "/Content/images/tiles/64px/pin/pin3.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Four, Owner = "p1", Name = "1RoundFour", Image = "/Content/images/tiles/64px/pin/pin4.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Five, Owner = "p1", Name = "1RoundFive", Image = "/Content/images/tiles/64px/pin/pin5.png" });
            return tiles;
        }

        public static List<Tile> BuildAllPongTiles()
        {
            List<Tile> tiles = new List<Tile>();
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.One, Owner = "p1", Name = "1MoneyOne", Image = "/Content/images/tiles/64px/man/man1.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.One, Owner = "p1", Name = "2MoneyOne", Image = "/Content/images/tiles/64px/man/man1.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.One, Owner = "p1", Name = "3MoneyOne", Image = "/Content/images/tiles/64px/man/man1.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Four, Owner = "p1", Name = "1MoneyFour", Image = "/Content/images/tiles/64px/man/man4.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Four, Owner = "p1", Name = "2MoneyFour", Image = "/Content/images/tiles/64px/man/man4.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Four, Owner = "p1", Name = "3MoneyFour", Image = "/Content/images/tiles/64px/man/man4.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Seven, Owner = "p1", Name = "1MoneySeven", Image = "/Content/images/tiles/64px/man/man7.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Seven, Owner = "p1", Name = "2MoneySeven", Image = "/Content/images/tiles/64px/man/man7.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Seven, Owner = "p1", Name = "3MoneySeven", Image = "/Content/images/tiles/64px/man/man7.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Two, Owner = "p1", Name = "1RoundTwo", Image = "/Content/images/tiles/64px/pin/pin2.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Two, Owner = "p1", Name = "2RoundTwo", Image = "/Content/images/tiles/64px/pin/pin2.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Two, Owner = "p1", Name = "3RoundTwo", Image = "/Content/images/tiles/64px/pin/pin2.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Five, Owner = "p1", Name = "1RoundFive", Image = "/Content/images/tiles/64px/pin/pin5.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Five, Owner = "p1", Name = "2RoundFive", Image = "/Content/images/tiles/64px/pin/pin5.png" });
            return tiles;
        }

        public static List<Tile> BuildNotAllPongTiles()
        {
            List<Tile> tiles = new List<Tile>();
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.One, Owner = "p1", Name = "1MoneyOne", Image = "/Content/images/tiles/64px/man/man1.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.One, Owner = "p1", Name = "2MoneyOne", Image = "/Content/images/tiles/64px/man/man1.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.One, Owner = "p1", Name = "3MoneyOne", Image = "/Content/images/tiles/64px/man/man1.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Four, Owner = "p1", Name = "1MoneyFour", Image = "/Content/images/tiles/64px/man/man4.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Four, Owner = "p1", Name = "2MoneyFour", Image = "/Content/images/tiles/64px/man/man4.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Four, Owner = "p1", Name = "3MoneyFour", Image = "/Content/images/tiles/64px/man/man4.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Seven, Owner = "p1", Name = "1MoneySeven", Image = "/Content/images/tiles/64px/man/man7.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Seven, Owner = "p1", Name = "2MoneySeven", Image = "/Content/images/tiles/64px/man/man7.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Seven, Owner = "p1", Name = "3MoneySeven", Image = "/Content/images/tiles/64px/man/man7.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Two, Owner = "p1", Name = "1RoundTwo", Image = "/Content/images/tiles/64px/pin/pin2.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Two, Owner = "p1", Name = "2RoundTwo", Image = "/Content/images/tiles/64px/pin/pin2.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Two, Owner = "p1", Name = "3RoundTwo", Image = "/Content/images/tiles/64px/pin/pin2.png" });
            tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserActive, Value = TileValue.Five, Owner = "p1", Name = "1RoundFive", Image = "/Content/images/tiles/64px/pin/pin5.png" });
            tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.UserActive, Value = TileValue.Seven, Owner = "p1", Name = "4MoneySeven", Image = "/Content/images/tiles/64px/pin/man7.png" });
            return tiles;
        }
    }
}