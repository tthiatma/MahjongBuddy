using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MahjongBuddy.Models;
using System.Collections.Generic;

namespace MahjongBuddy.Tests
{
    [TestClass]
    public class WinningTileSetUnitTest
    {
        [TestMethod]
        public void TestAllPongWinningSet()
        {
            GameLogic gl = new GameLogic();
            var tiles = TileBuilder.BuildAllPongTiles();
            var wt = gl.BuildWinningTiles(tiles);
            Assert.IsNotNull(wt);
        }
        [TestMethod]
        public void TestNotAllPongWinningSet()
        {
            GameLogic gl = new GameLogic();
            var tiles = TileBuilder.BuildNotAllPongTiles();
            var wt = gl.BuildWinningTiles(tiles);
            Assert.IsNull(wt);
        }

        [TestMethod]
        public void TestAllStraightWinningSet()
        {
            GameLogic gl = new GameLogic();
            var tiles = TileBuilder.BuildAllStraightTiles();
            var wt = gl.BuildWinningTiles(tiles);
            Assert.IsNotNull(wt);
        }

        [TestMethod]
        public void TestNotAllStraightWinningSet()
        {
            GameLogic gl = new GameLogic();
            var tiles = TileBuilder.BuildNotAllStraightTiles();
            var wt = gl.BuildWinningTiles(tiles);
            Assert.IsNull(wt);
        }
    }
}
