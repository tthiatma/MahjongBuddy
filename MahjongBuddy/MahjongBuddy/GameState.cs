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
            gl.PopulatePoint(game);
            game.Board.CreateTiles();

            var group = groupName;
            _games[group] = game;

            player1.IsPlaying = true;
            player1.CanDoNoFlower = true;
            player1.Group = group;

            player2.IsPlaying = true;
            player2.CanDoNoFlower = true;
            player2.Group = group;

            player3.IsPlaying = true;
            player3.CanDoNoFlower = true;
            player3.Group = group;
            
            player4.IsPlaying = true;
            player4.CanDoNoFlower = true;
            player4.Group = group;

            return game;
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
            game.TileCounter = 0;
            game.PlayerTurn = game.DiceRoller;
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