using MahjongBuddy.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Security.Cryptography;
using System.Text;
using MahjongBuddy.Extensions;


namespace MahjongBuddy
{
    public class GameState
    {
        GameLogic gl = new GameLogic();

        private readonly static Lazy<GameState> _instance = 
            new Lazy<GameState>(()=> 
                new GameState(
                    GlobalHost.ConnectionManager.GetHubContext<GameHub>()                
                )
            );

        private readonly ConcurrentDictionary<string, Player> _players = new ConcurrentDictionary<string, Player>(StringComparer.OrdinalIgnoreCase);

        private readonly ConcurrentDictionary<string, Game> _games = new ConcurrentDictionary<string, Game>(StringComparer.OrdinalIgnoreCase);

        public ConcurrentDictionary<string, Player> Players
        {
            get { return _players; }
        }

        private GameState(IHubContext context)
        {
            Clients = context.Clients;
            Groups = context.Groups;
        }

        public static GameState Instance
        {
	        get{return _instance.Value;}
        }

        public IHubConnectionContext<dynamic> Clients { get; set; }

        public IGroupManager Groups { get; set; }

        public Player CreatePlayer(string userName, string userGroup)
        {
            var player = new Player(userName, userGroup, GetMD5Hash(userName));
            _players[userName] = player;
            return player;
        }

        public Player GetPlayer(string userName)
        {
            return _players.Values.FirstOrDefault(u => u.Name == userName);
        }

        public void StartNextGame(Game game)
        {
            //if there's winner from game jz now
            if (game.Records != null && game.Records.Last().NoWinner == false)
            {
                if (game.Records != null && game.Records.Last().Winner != null)
                {
                    if (game.DiceRoller.ConnectionId != game.Records.Last().Winner.ConnectionId)
                    {
                        game.DiceMovedCount++;
                        gl.SetNextGamePlayerToStart(game);
                        gl.SetGameNextWind(game);
                        gl.SetPlayerWinds(game);
                    }
                }
            }

            GameState.Instance.ResetForNextGame(game);
            DistributeTiles(game);

            if (game.GameSetting.SkipInitialFlowerSwapping)
            {
                gl.RecycleInitialFlower(game);
            }
            game.TilesLeft = game.Board.Tiles.Where(t => t.Owner == "board").Count();
        }

        public Game CreateGame(Player player1, Player player2, Player player3, Player player4, string groupName)
        {
            var game = new Game()
            {
                Player1 = player1,
                Player2 = player2,
                Player3 = player3,
                Player4 = player4,
                Board = new Board(),
                Records = new List<Record>(),
                GameSetting = new GameSetting(),
                PointSystem = new Dictionary<WinningType,int>()
            };

            InitPlayerProperties(player1, player2, player3, player4, game, groupName);
            InitGameProperties(player1, game);
            gl.SetPlayerWinds(game);

            _games[groupName] = game;
            
            return game;
        }

        private void InitGameProperties(Player starterPlayer, Game game)
        {
            game.PlayerTurn = starterPlayer;
            game.DiceRoller = starterPlayer;
            game.DiceMovedCount = 1;
            game.TileCounter = 0;
            game.CurrentWind = WindDirection.East;
            game.GameSetting.SkipInitialFlowerSwapping = true;
            
            //tiles section
            game.Board.CreateTiles();
            game.Board.Tiles.Shuffle();            
            DistributeTiles(game);
            if (game.GameSetting.SkipInitialFlowerSwapping)
            {
                gl.RecycleInitialFlower(game);
            }

            game.TilesLeft = game.Board.Tiles.Where(t => t.Owner == "board").Count();
            
            gl.PopulatePoint(game);
            
            AssignAllPlayersTileIndex(game);
        }

        private void InitPlayerProperties(Player player1, Player player2, Player player3, Player player4, Game game, string groupName) 
        {

            player1.IsPlaying = true;
            player1.CanDoNoFlower = true;
            player1.Group = groupName;

            player2.IsPlaying = true;
            player2.CanDoNoFlower = true;
            player2.Group = groupName;

            player3.IsPlaying = true;
            player3.CanDoNoFlower = true;
            player3.Group = groupName;

            player4.IsPlaying = true;
            player4.CanDoNoFlower = true;
            player4.Group = groupName;
        }

        private void AssignAllPlayersTileIndex(Game game)
        {
            List<Tile> tiles = game.Board.Tiles;

            var player1Tiles = tiles.Where(t => t.Owner == game.Player1.ConnectionId && t.Status == TileStatus.UserActive).OrderBy(t => t.Type).ThenBy(t => t.Value).ToArray();
            var player2Tiles = tiles.Where(t => t.Owner == game.Player2.ConnectionId && t.Status == TileStatus.UserActive).OrderBy(t => t.Type).ThenBy(t => t.Value).ToArray();
            var player3Tiles = tiles.Where(t => t.Owner == game.Player3.ConnectionId && t.Status == TileStatus.UserActive).OrderBy(t => t.Type).ThenBy(t => t.Value).ToArray();
            var player4Tiles = tiles.Where(t => t.Owner == game.Player4.ConnectionId && t.Status == TileStatus.UserActive).OrderBy(t => t.Type).ThenBy(t => t.Value).ToArray();

            for (int i = 0; i < player1Tiles.Count(); i++)
            {
                player1Tiles[i].ActiveTileIndex = i;
            }

            for (int i = 0; i < player2Tiles.Count(); i++)
            {
                player2Tiles[i].ActiveTileIndex = i;
            }

            for (int i = 0; i < player3Tiles.Count(); i++)
            {
                player3Tiles[i].ActiveTileIndex = i;
            }

            for (int i = 0; i < player4Tiles.Count(); i++)
            {
                player4Tiles[i].ActiveTileIndex = i;
            }
        }

        private void DistributeTiles(Game game)
        {
            List<Tile> tiles = game.Board.Tiles;
            Player p1, p2, p3, p4;
            if (game.DiceRoller.ConnectionId == game.Player1.ConnectionId)
            {
                p1 = game.Player1;
                p2 = game.Player2;
                p3 = game.Player3;
                p4 = game.Player4;
            }
            else if (game.DiceRoller.ConnectionId == game.Player2.ConnectionId)
            {
                p1 = game.Player2;
                p2 = game.Player3;
                p3 = game.Player4;
                p4 = game.Player1;
            }
            else if (game.DiceRoller.ConnectionId == game.Player3.ConnectionId)
            {
                p1 = game.Player3;
                p2 = game.Player4;
                p3 = game.Player1;
                p4 = game.Player2;
            }
            else
            {
                p1 = game.Player4;
                p2 = game.Player1;
                p3 = game.Player2;
                p4 = game.Player3;
            }

            for (var i = 0; i < 14; i++)
            {
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
            for (var i = 27; i < 40; i++)
            {
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
            for (var i = 40; i < 53; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
        }

        public Game FindGameByGroupName(string groupName)
        {
            Game game;
            _games.TryGetValue(groupName, out game);

            if (game != null)
            {
                return game;
            }
            return null;
        }

        public Game FindGame(Player player, Player opponent1, Player opponent2, Player opponent3)
        {
            opponent1 = null;
            if (player.Group == null)
            {
                return null;
            }

            Game game;
            _games.TryGetValue(player.Group, out game);

            if (game != null)
            {
                if (player.Id == game.Player1.Id)
                {
                    opponent1 = game.Player2;
                }

                opponent1 = game.Player1;
                return game;
            }
            return null;
        }

        public void ResetForNextGame(Game game)
        {
            game.Board.Tiles = new List<Tile>();
            game.Board.CreateTiles();
            game.Board.Tiles.Shuffle();
            game.LastTile = null;
            game.TileCounter = 0;            
            game.PlayerTurn = game.DiceRoller;
            game.TilesLeft = game.Board.Tiles.Where(t => t.Owner == "board").Count();
            game.HaltMove = false;

            //reset player tileset
            game.Player1.TileSets = new List<TileSet>();
            game.Player2.TileSets = new List<TileSet>();
            game.Player3.TileSets = new List<TileSet>();
            game.Player4.TileSets = new List<TileSet>();
        }

        public void ResetGame(Game game)
        {
            var groupName = game.Player1.Group;
            var player1Name = game.Player1.Name;
            var player2Name = game.Player2.Name;
            var player3Name = game.Player3.Name;
            var player4Name = game.Player4.Name;

            Groups.Remove(game.Player1.ConnectionId, groupName);
            Groups.Remove(game.Player2.ConnectionId, groupName);
            Groups.Remove(game.Player3.ConnectionId, groupName);
            Groups.Remove(game.Player4.ConnectionId, groupName);

            Player p1;
            _players.TryRemove(player1Name, out p1);

            Player p2;
            _players.TryRemove(player2Name, out p2);

            Player p3;
            _players.TryRemove(player3Name, out p3);

            Player p4;
            _players.TryRemove(player4Name, out p4);

            Game g;
            _games.TryRemove(groupName, out g);

        }

        private string GetMD5Hash(string userName)
        {
            return String.Join("", MD5.Create()
                                .ComputeHash(Encoding.Default.GetBytes(userName))
                                .Select(b => b.ToString("x2")));
        }

    }
}