using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MahjongBuddy.Models;
using System.Linq;
using System.Collections.Generic;

namespace MahjongBuddy.Tests
{
    [TestClass]
    public class PointCountUnitTest
    {
        Game game = new Game();
        PointCalculator pc = new PointCalculator();
        Player player = new Player("test", "test", "test");
        GameLogic gl = new GameLogic();

        [TestMethod]
        public void TestFlower()
        {
            var tiles = TileBuilder.BuildAllStraightTilesWithOneFlower();
            game.CurrentWind = WindDirection.East;
            player.Wind = WindDirection.East;
            player.ConnectionId = "p1";

            int points = pc.CalculateFlower(game, tiles, player);
            Assert.AreEqual(1, points);
        }
        
        [TestMethod]
        public void TestWin()
        {
            game.Board = new Board();
            game.Board.CreateTiles();
            game.Records = new List<Record>();
            game.CurrentWind = WindDirection.East;
            game.PointSystem = new Dictionary<WinningType, int>();
            gl.PopulatePoint(game);
            player.Wind = WindDirection.East;
            player.ConnectionId = "p1";

            game.Player1 = new Player("test","test","test");
            game.Player2 = new Player("test2","test2","test2");
            game.Player3 = new Player("test3","test3","test3");
            game.Player4 = new Player("test4", "test4", "test4");

            var dTiles = game.Board.Tiles.Where(
                t => t.Id == 1
                || t.Id == 2
                || t.Id == 3
                || t.Id == 4
                || t.Id == 5
                || t.Id == 6
                || t.Id == 7
                || t.Id == 8
                || t.Id == 9
                || t.Id == 10
                || t.Id == 11
                || t.Id == 12
                || t.Id == 13
                || t.Id == 47
                || t.Id == 137
                || t.Id == 141
                );

            foreach (var t in dTiles)
            {
                t.Owner = "p1";
                t.Status = TileStatus.UserActive;

                if (t.Id == 1)
                {
                    t.Status = TileStatus.JustPicked;
                }

                if (t.Id == 10 || t.Id == 11 || t.Id == 12)
                {
                    t.Status = TileStatus.BoardGraveyard;                
                }
                if (t.Id == 137 || t.Id == 141)
                {
                    t.Status = TileStatus.UserGraveyard;
                }
            }

            List<Tile> temp = new List<Tile>();
            temp.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserGraveyard, Value = TileValue.One, Owner = "p1", Name = "1RoundOne", Image = "/Content/images/tiles/64px/pin/pin1.png" });
            temp.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserGraveyard, Value = TileValue.Two, Owner = "p1", Name = "1RoundTwo", Image = "/Content/images/tiles/64px/pin/pin2.png" });
            temp.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserGraveyard, Value = TileValue.Three, Owner = "p1", Name = "1RoundThree", Image = "/Content/images/tiles/64px/pin/pin3.png" });

            var tempTs = new TileSet()
            {
                Tiles = temp,
                TileSetType = TileSetType.Chow,
                TileType = TileType.Round,
                isRevealed = true
            };
            player.TileSets.Add(tempTs);
            
            var cs = gl.DoWin(game, player);

            Assert.AreEqual(CommandResult.PlayerWin, cs);
        }

        [TestMethod]
        public void TestWin13Wonders()
        {
            game.Board = new Board();
            game.Records = new List<Record>();
            game.Board.CreateTiles();
            game.CurrentWind = WindDirection.East;
            game.PointSystem = new Dictionary<WinningType, int>();
            gl.PopulatePoint(game);
            player.Wind = WindDirection.East;
            player.ConnectionId = "p1";

            var dTiles = game.Board.Tiles.Where(
                t => t.Id == 1
                || t.Id == 9
                || t.Id == 10
                || t.Id == 18
                || t.Id == 19
                || t.Id == 27
                || t.Id == 28
                || t.Id == 29
                || t.Id == 30
                || t.Id == 31
                || t.Id == 32
                || t.Id == 33
                || t.Id == 34
                || t.Id == 35
                );

            foreach (var t in dTiles)
            {
                t.Owner = "p1";
                t.Status = TileStatus.UserActive;
            }

            var cs = gl.DoWin(game, player);

            Assert.AreEqual(CommandResult.PlayerWin, cs);
        }
        
        [TestMethod]
        public void TestWinAllPairs()
        {
            game.Board = new Board();
            game.Records = new List<Record>();
            game.Board.CreateTiles();
            game.CurrentWind = WindDirection.East;
            game.PointSystem = new Dictionary<WinningType, int>();
            gl.PopulatePoint(game);
            player.Wind = WindDirection.East;
            player.ConnectionId = "p1";

            var dTiles = game.Board.Tiles.Where(
                t => t.Id == 1
                || t.Id == 35
                || t.Id == 10
                || t.Id == 44
                || t.Id == 19
                || t.Id == 53
                || t.Id == 28
                || t.Id == 62
                || t.Id == 31
                || t.Id == 65
                || t.Id == 2
                || t.Id == 36
                || t.Id == 69
                || t.Id == 103
                );

            foreach (var t in dTiles)
            {
                t.Owner = "p1";
                t.Status = TileStatus.UserActive;
            }

            var cs = gl.DoWin(game, player);

            Assert.AreEqual(CommandResult.PlayerWin, cs);
        }

        [TestMethod]
        public void TestMixPureHand()
        {
            GameLogic gl = new GameLogic();
            var tiles = TileBuilder.BuildMixPureHand();
            var ws = gl.BuildWinningTiles(tiles, null);
            var wt = pc.GetWinningType(game, ws, player);
            Assert.IsTrue(wt.Contains(WinningType.MixPureHand));
        }

        [TestMethod]
        public void TestPureHand()
        {
            GameLogic gl = new GameLogic();
            var tiles = TileBuilder.BuildPureHand();
            var ws = gl.BuildWinningTiles(tiles, null);
            var wt = pc.GetWinningType(game, ws, player);
            Assert.IsTrue(wt.Contains(WinningType.PureHand));
        }

        //[TestMethod]
        //public void TestSevenPairs()
        //{
        //    GameLogic gl = new GameLogic();
        //    var tiles = TileBuilder.BuildSevenPairs();
        //    var ws = gl.BuildWinningTiles(tiles, null);
        //    var wt = pc.GetWinningType(game, ws, player);
        //    Assert.IsTrue(wt.Contains(WinningType.SevenPairs));
        //}
    }
}
