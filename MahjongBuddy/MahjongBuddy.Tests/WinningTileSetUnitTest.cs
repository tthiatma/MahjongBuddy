﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MahjongBuddy.Models;
using System.Collections.Generic;

namespace MahjongBuddy.Tests
{
    /// <summary>
    /// Test to create winningtileset
    /// winningtileset consist of 4 set of pong/chow + 1 eye
    /// </summary>
    [TestClass]
    public class WinningTileSetUnitTest
    {
        Game game = new Game();
        PointCalculator pc = new PointCalculator();
        Player player = new Player("test", "test", "test");
        GameLogic gl = new GameLogic();

        [TestMethod]
        public void TestAllPongWinningSet()
        {
            GameLogic gl = new GameLogic();
            var tiles = TileBuilder.BuildAllPongTiles();
            var wt = gl.BuildWinningTiles(tiles, null);
            Assert.IsNotNull(wt);
        }
        
        [TestMethod]
        public void TestNotAllPongWinningSet()
        {
            GameLogic gl = new GameLogic();
            var tiles = TileBuilder.BuildNotAllPongTiles();
            var wt = gl.BuildWinningTiles(tiles, null);
            Assert.IsNull(wt);
        }

        [TestMethod]
        public void TestAllStraightWinningSet()
        {
            GameLogic gl = new GameLogic();
            var tiles = TileBuilder.BuildAllStraightTiles();
            var wt = gl.BuildWinningTiles(tiles, null);
            Assert.IsNotNull(wt);
        }

        [TestMethod]
        public void TestNotAllStraightWinningSet()
        {
            GameLogic gl = new GameLogic();
            var tiles = TileBuilder.BuildNotAllStraightTiles();
            var wt = gl.BuildWinningTiles(tiles, null);
            Assert.IsNull(wt);
        }

    }
}
