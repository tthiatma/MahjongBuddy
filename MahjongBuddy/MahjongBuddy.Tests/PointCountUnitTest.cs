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
            game.CurrentWind = WindDirection.East;
            player.Wind = WindDirection.East;
            player.ConnectionId = "p1";

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
                );

            foreach (var t in dTiles)
            {
                t.Owner = "p1";
                t.Status = TileStatus.UserActive;

                if (t.Id == 10 || t.Id == 11 || t.Id == 12)
                {
                    t.Status = TileStatus.BoardGraveyard;                
                }
            }

            List<Tile> temp = new List<Tile>();
            temp.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserGraveyard, Value = TileValue.One, Owner = "p1", Name = "1RoundOne", Image = "/Content/images/tiles/64px/pin/pin1.png" });
            temp.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserGraveyard, Value = TileValue.Two, Owner = "p1", Name = "1RoundTwo", Image = "/Content/images/tiles/64px/pin/pin2.png" });
            temp.Add(new Tile() { Type = TileType.Round, Status = TileStatus.UserGraveyard, Value = TileValue.Three, Owner = "p1", Name = "1RoundThree", Image = "/Content/images/tiles/64px/pin/pin3.png" });

            var tempTs = new TileSet()
            {
                Tiles = temp,
                TileType = TileSetType.Chow
            };
            player.TileSets.Add(tempTs);
            
            var cs = gl.DoWin(game, player);

            Assert.AreEqual(CommandResult.PlayerWin, cs);
        }
 
    }
}
