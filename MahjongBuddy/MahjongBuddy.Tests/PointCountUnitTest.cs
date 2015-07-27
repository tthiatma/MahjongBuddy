using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MahjongBuddy.Models;
using System.Collections.Generic;

namespace MahjongBuddy.Tests
{
    [TestClass]
    public class PointCountUnitTest
    {
        [TestMethod]
        public void TestFlower()
        {
            Game game = new Game();
            Player player = new Player("test", "test", "test");
            List<Tile> tiles = new List<Tile>();
            PointCalculator pc = new PointCalculator();

            game.CurrentWind = WindDirection.East;

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
            tiles.Add(new Tile() { Type = TileType.Flower, Status = TileStatus.UserGraveyard, Value = TileValue.FlowerNumericOne, Owner = "p1", Name = "137FlowerNumeric", Image = "/Content/images/tiles/64px/flower/flower1.png" });

            player.Wind = WindDirection.East;
            player.ConnectionId = "p1";

            int points = pc.CalculateFlower(game, tiles, player);

            Assert.AreEqual(1, points);
        }
    }
}
