using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MahjongBuddy.Extensions;

namespace MahjongBuddy.Models
{
    public class Game
    {
        public ActivePlayer Player1 { get; set; }
        public ActivePlayer Player2 { get; set; }
        public ActivePlayer Player3 { get; set; }
        public ActivePlayer Player4 { get; set; }
        public List<Record> Records { get; set; }
        public Dictionary<WinningType, int> PointSystem { get; set; }
        public Board Board { get; set; }
        public Tile LastTile { get; set; }
        public int TilesLeft { get; set; }
        public string PlayerTurn { get; set; }
        public string DiceRoller { get; set; }
        public int DiceMovedCount { get; set; }
        public int Count { get; set; }
        public int TileCounter { get; set; }
        public bool HaltMove { get; set; }
        public WindDirection CurrentWind { get; set; }
        public GameSetting GameSetting { get; set; }

        public Game()
        {
            GameInit();

        }
        public Game(ActivePlayer p1, ActivePlayer p2, ActivePlayer p3, ActivePlayer p4)
        {
            GameInit();
            Player1 = p1;
            Player2 = p2;
            Player3 = p3;
            Player4 = p4;

            PlayerTurn = p1.ConnectionId;
            DiceRoller = p1.ConnectionId;
            DiceMovedCount = 1;
            TileCounter = 0;
            CurrentWind = WindDirection.East;
            GameSetting.SkipInitialFlowerSwapping = true;

            Board.CreateTiles();
        }

        private void GameInit()
        {
            HaltMove = true;
            Board = new Board();
            Records = new List<Record>();
            GameSetting = new GameSetting();
            PointSystem = new Dictionary<WinningType, int>();
        }
    }
}