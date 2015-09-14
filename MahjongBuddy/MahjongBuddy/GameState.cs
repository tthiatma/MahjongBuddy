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

        private readonly ConcurrentDictionary<string, ActivePlayer> _players = new ConcurrentDictionary<string, ActivePlayer>(StringComparer.OrdinalIgnoreCase);

        private readonly ConcurrentDictionary<string, Game> _games = new ConcurrentDictionary<string, Game>(StringComparer.OrdinalIgnoreCase);

        private GameState(IHubContext context)
        {
            Clients = context.Clients;
            Groups = context.Groups;
        }

        public ConcurrentDictionary<string, ActivePlayer> Players
        {
            get { return _players; }
        }

        public static GameState Instance
        {
	        get{return _instance.Value;}
        }

        public IHubConnectionContext<dynamic> Clients { get; set; }

        public IGroupManager Groups { get; set; }

        public ActivePlayer CreatePlayer(string userName, string userGroup)
        {
            var player = new ActivePlayer(userName, userGroup, GetMD5Hash(userName));
            _players[userName] = player;
            return player;
        }

        public ActivePlayer GetPlayer(string userName)
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
                    if (game.DiceRoller != game.Records.Last().Winner.ConnectionId)
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

        public Game CreateGame(ActivePlayer player1, ActivePlayer player2, ActivePlayer player3, ActivePlayer player4, string groupName)
        {
            var game = new Game(player1, player2, player3, player4);

            InitPlayerProperties(player1, player2, player3, player4, game, groupName);
            InitGameProperties(player1, game);
            _games[groupName] = game;            

            return game;
        }

        private void InitGameProperties(Player starterPlayer, Game game)
        {
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

        private void InitPlayerProperties(ActivePlayer player1, ActivePlayer player2, ActivePlayer player3, ActivePlayer player4, Game game, string groupName) 
        {
            player1.IsPlaying = true;
            player1.CanDoNoFlower = true;
            player1.Group = groupName;

            SetOtherPlayer(player1, player2, player3, player4);

            player2.IsPlaying = true;
            player2.CanDoNoFlower = true;
            player2.Group = groupName;
            SetOtherPlayer(player2, player3, player4, player1);

            player3.IsPlaying = true;
            player3.CanDoNoFlower = true;
            player3.Group = groupName;
            SetOtherPlayer(player3, player4, player1, player2);

            player4.IsPlaying = true;
            player4.CanDoNoFlower = true;
            player4.Group = groupName;
            SetOtherPlayer(player4, player1, player2, player3);

            player1.RightPlayer.GraveYardTiles = player2.GraveYardTiles;
            player1.TopPlayer.GraveYardTiles = player3.GraveYardTiles;
            player1.LeftPlayer.GraveYardTiles = player4.GraveYardTiles;

            player2.RightPlayer.GraveYardTiles = player3.GraveYardTiles;
            player2.TopPlayer.GraveYardTiles = player4.GraveYardTiles;
            player2.LeftPlayer.GraveYardTiles = player1.GraveYardTiles;

            player3.RightPlayer.GraveYardTiles = player4.GraveYardTiles;
            player3.TopPlayer.GraveYardTiles = player1.GraveYardTiles;
            player3.LeftPlayer.GraveYardTiles = player2.GraveYardTiles;

            player4.RightPlayer.GraveYardTiles = player1.GraveYardTiles;
            player4.TopPlayer.GraveYardTiles = player2.GraveYardTiles;
            player4.LeftPlayer.GraveYardTiles = player3.GraveYardTiles;

            gl.SetPlayerWinds(game);
        }

        private void SetOtherPlayer(Player currentPlayer, Player rightPlayer, Player topPlayer, Player leftPlayer)
        {
            currentPlayer.RightPlayer = new OtherPlayer(rightPlayer);           
            currentPlayer.TopPlayer = new OtherPlayer(topPlayer);
            currentPlayer.LeftPlayer = new OtherPlayer(leftPlayer);
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
            ActivePlayer p1, p2, p3, p4;
            if (game.DiceRoller == game.Player1.ConnectionId)
            {
                p1 = game.Player1;
                p2 = game.Player2;
                p3 = game.Player3;
                p4 = game.Player4;
            }
            else if (game.DiceRoller == game.Player2.ConnectionId)
            {
                p1 = game.Player2;
                p2 = game.Player3;
                p3 = game.Player4;
                p4 = game.Player1;
            }
            else if (game.DiceRoller == game.Player3.ConnectionId)
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
                p1.ActiveTiles.Add(tiles[i]);

                p2.LeftPlayer.ActiveTilesCount++;
                p3.TopPlayer.ActiveTilesCount++;
                p4.RightPlayer.ActiveTilesCount++;
            }

            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);

                p3.LeftPlayer.ActiveTilesCount++;
                p4.TopPlayer.ActiveTilesCount++;
                p1.RightPlayer.ActiveTilesCount++;

            }
            for (var i = 27; i < 40; i++)
            {
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);

                p4.LeftPlayer.ActiveTilesCount++;
                p1.TopPlayer.ActiveTilesCount++;
                p2.RightPlayer.ActiveTilesCount++;
            }
            for (var i = 40; i < 53; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);

                p1.LeftPlayer.ActiveTilesCount++;
                p2.TopPlayer.ActiveTilesCount++;
                p3.RightPlayer.ActiveTilesCount++;
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

            ActivePlayer p1;
            _players.TryRemove(player1Name, out p1);

            ActivePlayer p2;
            _players.TryRemove(player2Name, out p2);

            ActivePlayer p3;
            _players.TryRemove(player3Name, out p3);

            ActivePlayer p4;
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