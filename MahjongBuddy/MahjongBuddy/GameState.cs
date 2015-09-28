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
using log4net;


namespace MahjongBuddy
{
    public class GameState
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GameHub));

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
            try
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

                gl.AssignPlayersTileIndex(game, game.Player1);
                gl.AssignPlayersTileIndex(game, game.Player2);
                gl.AssignPlayersTileIndex(game, game.Player3);
                gl.AssignPlayersTileIndex(game, game.Player4);

                if (game.GameSetting.SkipInitialFlowerSwapping)
                {
                    gl.RecycleInitialFlower(game);
                }
                game.TilesLeft = game.Board.Tiles.Where(t => t.Owner == "board").Count();
            }
            catch (Exception ex)
            {
                logger.Error("error on StartNextGame", ex);
            }
        }

        public Game CreateGame(ActivePlayer player1, ActivePlayer player2, ActivePlayer player3, ActivePlayer player4, string groupName)
        {
            var game = new Game(player1, player2, player3, player4);            
            InitGameProperties(player1, game);
            InitPlayerProperties(player1, player2, player3, player4, game, groupName);
            _games[groupName] = game;            

            return game;
        }

        private void InitGameProperties(Player starterPlayer, Game game)
        {
            game.Board.Tiles.Shuffle();
            DistributeTiles(game);
            //DistributeTilesForWin(game);
            //DistributeTilesForNoWinner(game);
            //DistributeTilesForChow(game);
            //DistributeTilesForPong(game);
            //DistributeTilesForKong(game);
            //DistributeTilesForSelfKong(game);
            //DistributeTilesForSelfKongToPong(game);
            //DistributeTilesForWinWaitingForEyeWithMixPureHand(game);
            //DistributeTilesForWinWaitingForEyeWithPureHand(game);
            //DistributeTilesForWinWaitingForEyeWithMixPureHandWithDragon(game);
            if (game.GameSetting.SkipInitialFlowerSwapping)
            {
                gl.RecycleInitialFlower(game);
            }

            game.TilesLeft = game.Board.Tiles.Where(t => t.Owner == "board").Count();            
            gl.PopulatePoint(game);
            
            gl.AssignPlayersTileIndex(game, game.Player1);
            gl.AssignPlayersTileIndex(game, game.Player2);
            gl.AssignPlayersTileIndex(game, game.Player3);
            gl.AssignPlayersTileIndex(game, game.Player4);
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

        private void SetOtherPlayer(ActivePlayer currentPlayer, ActivePlayer rightPlayer, ActivePlayer topPlayer, ActivePlayer leftPlayer)
        {
            currentPlayer.RightPlayer = new OtherPlayer(rightPlayer);
            currentPlayer.RightPlayer.ConnectionId = rightPlayer.ConnectionId;
            currentPlayer.RightPlayer.ActivePlayer = rightPlayer;
            currentPlayer.RightPlayer.CurrentPoint= rightPlayer.CurrentPoint;

            currentPlayer.TopPlayer = new OtherPlayer(topPlayer);
            currentPlayer.TopPlayer.ConnectionId = topPlayer.ConnectionId;
            currentPlayer.TopPlayer.ActivePlayer = topPlayer;
            currentPlayer.TopPlayer.CurrentPoint = topPlayer.CurrentPoint;

            currentPlayer.LeftPlayer = new OtherPlayer(leftPlayer);
            currentPlayer.LeftPlayer.ConnectionId = leftPlayer.ConnectionId;
            currentPlayer.LeftPlayer.ActivePlayer = leftPlayer;
            currentPlayer.LeftPlayer.CurrentPoint = leftPlayer.CurrentPoint;
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

            //reset player tiles
            game.Player1.TileSets = new List<TileSet>();
            game.Player1.ActiveTiles = new List<Tile>();
            game.Player1.GraveYardTiles = new List<Tile>();
            game.Player1.RightPlayer.GraveYardTiles = new List<Tile>();
            game.Player1.TopPlayer.GraveYardTiles = new List<Tile>();
            game.Player1.LeftPlayer.GraveYardTiles = new List<Tile>();

            game.Player2.TileSets = new List<TileSet>();
            game.Player2.ActiveTiles = new List<Tile>();
            game.Player2.GraveYardTiles = new List<Tile>();
            game.Player2.RightPlayer.GraveYardTiles = new List<Tile>();
            game.Player2.TopPlayer.GraveYardTiles = new List<Tile>();
            game.Player2.LeftPlayer.GraveYardTiles = new List<Tile>();

            game.Player3.TileSets = new List<TileSet>();
            game.Player3.ActiveTiles = new List<Tile>();
            game.Player3.GraveYardTiles = new List<Tile>();
            game.Player3.RightPlayer.GraveYardTiles = new List<Tile>();
            game.Player3.TopPlayer.GraveYardTiles = new List<Tile>();
            game.Player3.LeftPlayer.GraveYardTiles = new List<Tile>();

            game.Player4.TileSets = new List<TileSet>();
            game.Player4.ActiveTiles = new List<Tile>();
            game.Player4.GraveYardTiles = new List<Tile>();
            game.Player4.RightPlayer.GraveYardTiles = new List<Tile>();
            game.Player4.TopPlayer.GraveYardTiles = new List<Tile>();
            game.Player4.LeftPlayer.GraveYardTiles = new List<Tile>();
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
            }

            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 27; i < 40; i++)
            {
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 40; i < 53; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);
            }
        }
        //all of this belong to test section
        //TODO : move this to test
        private void DistributeTilesForWin(Game game)
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

            for (var i = 0; i < 13; i++)
            {
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p1.ActiveTiles.Add(tiles[i]);
            }
            tiles[46].Owner = p1.ConnectionId;
            tiles[46].Status = TileStatus.JustPicked;
            p1.ActiveTiles.Add(tiles[46]);

            //tiles[136].Owner = p1.ConnectionId;
            //tiles[136].Status = TileStatus.UserGraveyard;
            //p1.GraveYardTiles.Add(tiles[136]);

            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 27; i < 40; i++)
            {
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 53; i < 66; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);
            }
        }

        private void DistributeTilesForWinWaitingForEyeWithMixPureHand(Game game)
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

            for (var i = 0; i < 9; i++)
            {
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserGraveyard;
                p1.GraveYardTiles.Add(tiles[i]);
            }
            List<Tile> tempTile1 = new List<Tile>();
            tempTile1.Add(tiles[0]);
            tempTile1.Add(tiles[1]);
            tempTile1.Add(tiles[2]);
            TileSet newbie1 = new TileSet() { isRevealed = true, Tiles = tempTile1, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie1);

            List<Tile> tempTile2 = new List<Tile>();
            tempTile2.Add(tiles[3]);
            tempTile2.Add(tiles[4]);
            tempTile2.Add(tiles[5]);
            TileSet newbie2 = new TileSet() { isRevealed = true, Tiles = tempTile2, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie2);

            List<Tile> tempTile3 = new List<Tile>();
            tempTile3.Add(tiles[6]);
            tempTile3.Add(tiles[7]);
            tempTile3.Add(tiles[8]);
            TileSet newbie3 = new TileSet() { isRevealed = true, Tiles = tempTile3, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie3);

            for (var i = 34; i < 37; i++)
            {
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserGraveyard;
                p1.GraveYardTiles.Add(tiles[i]);
            }
            List<Tile> tempTile4 = new List<Tile>();
            tempTile4.Add(tiles[34]);
            tempTile4.Add(tiles[35]);
            tempTile4.Add(tiles[36]);
            TileSet newbie4 = new TileSet() { isRevealed = true, Tiles = tempTile4, TileSetType = TileSetType.Chow, TileType = TileType.Round };
            p1.TileSets.Add(newbie4);

            tiles[66].Owner = p1.ConnectionId;
            tiles[66].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[66]);

            tiles[121].Owner = p1.ConnectionId;
            tiles[121].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[121]);

            tiles[136].Owner = p1.ConnectionId;
            tiles[136].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[136]);
            
            tiles[140].Owner = p1.ConnectionId;
            tiles[140].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[140]);

            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 27; i < 40; i++)
            {
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 53; i < 65; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);
            }
            tiles[80].Owner = p4.ConnectionId;
            tiles[80].Status = TileStatus.UserActive;
            p4.ActiveTiles.Add(tiles[80]);
        }
        
        private void DistributeTilesForWinWaitingForEyeWithPureHand(Game game)
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

            for (var i = 0; i < 9; i++)
            {
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserGraveyard;
                p1.GraveYardTiles.Add(tiles[i]);
            }
            List<Tile> tempTile1 = new List<Tile>();
            tempTile1.Add(tiles[0]);
            tempTile1.Add(tiles[1]);
            tempTile1.Add(tiles[2]);
            TileSet newbie1 = new TileSet() { isRevealed = true, Tiles = tempTile1, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie1);

            List<Tile> tempTile2 = new List<Tile>();
            tempTile2.Add(tiles[3]);
            tempTile2.Add(tiles[4]);
            tempTile2.Add(tiles[5]);
            TileSet newbie2 = new TileSet() { isRevealed = true, Tiles = tempTile2, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie2);

            List<Tile> tempTile3 = new List<Tile>();
            tempTile3.Add(tiles[6]);
            tempTile3.Add(tiles[7]);
            tempTile3.Add(tiles[8]);
            TileSet newbie3 = new TileSet() { isRevealed = true, Tiles = tempTile3, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie3);

            for (var i = 34; i < 37; i++)
            {
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserGraveyard;
                p1.GraveYardTiles.Add(tiles[i]);
            }
            List<Tile> tempTile4 = new List<Tile>();
            tempTile4.Add(tiles[34]);
            tempTile4.Add(tiles[35]);
            tempTile4.Add(tiles[36]);
            TileSet newbie4 = new TileSet() { isRevealed = true, Tiles = tempTile4, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie4);

            tiles[70].Owner = p1.ConnectionId;
            tiles[70].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[70]);

            tiles[121].Owner = p1.ConnectionId;
            tiles[121].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[121]);

            tiles[136].Owner = p1.ConnectionId;
            tiles[136].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[136]);

            tiles[140].Owner = p1.ConnectionId;
            tiles[140].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[140]);

            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 27; i < 40; i++)
            {
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 53; i < 65; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);
            }
            tiles[80].Owner = p4.ConnectionId;
            tiles[80].Status = TileStatus.UserActive;
            p4.ActiveTiles.Add(tiles[80]);
        }

        private void DistributeTilesForWinWaitingForEye(Game game)
        {
            List<Tile> tiles = game.Board.Tiles;
            Player p1, p2, p3, p4;
            p1 = game.Player1;
            p2 = game.Player2;
            p3 = game.Player3;
            p4 = game.Player4;

            for (var i = 0; i < 12; i++)
            {
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserGraveyard;
            }
            List<Tile> tempTile1 = new List<Tile>();
            tempTile1.Add(tiles[0]);
            tempTile1.Add(tiles[1]);
            tempTile1.Add(tiles[2]);
            TileSet newbie1 = new TileSet() { isRevealed = true, Tiles = tempTile1, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie1);

            List<Tile> tempTile2 = new List<Tile>();
            tempTile2.Add(tiles[3]);
            tempTile2.Add(tiles[4]);
            tempTile2.Add(tiles[5]);
            TileSet newbie2 = new TileSet() { isRevealed = true, Tiles = tempTile2, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie2);

            List<Tile> tempTile3 = new List<Tile>();
            tempTile3.Add(tiles[6]);
            tempTile3.Add(tiles[7]);
            tempTile3.Add(tiles[8]);
            TileSet newbie3 = new TileSet() { isRevealed = true, Tiles = tempTile3, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie3);

            List<Tile> tempTile4 = new List<Tile>();
            tempTile4.Add(tiles[9]);
            tempTile4.Add(tiles[10]);
            tempTile4.Add(tiles[11]);
            TileSet newbie4 = new TileSet() { isRevealed = true, Tiles = tempTile4, TileSetType = TileSetType.Chow, TileType = TileType.Round };
            p1.TileSets.Add(newbie4);

            tiles[12].Owner = p1.ConnectionId;
            tiles[12].Status = TileStatus.UserActive;

            tiles[66].Owner = p1.ConnectionId;
            tiles[66].Status = TileStatus.UserActive;
            tiles[136].Owner = p1.ConnectionId;
            tiles[136].Status = TileStatus.UserGraveyard;
            tiles[140].Owner = p1.ConnectionId;
            tiles[140].Status = TileStatus.UserGraveyard;

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
            for (var i = 53; i < 65; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
            tiles[80].Owner = p4.ConnectionId;
            tiles[80].Status = TileStatus.UserActive;
        }

        private void DistributeTilesForKong(Game game)
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

            for (var i = 0; i < 13; i++)
            {
                if (i == 3)
                {
                    continue;
                }
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p1.ActiveTiles.Add(tiles[i]);

            }
            tiles[38].Owner = p1.ConnectionId;
            tiles[38].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[38]);

            tiles[106].Owner = p1.ConnectionId;
            tiles[106].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[106]);


            for (var i = 14; i < 26; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);
            }
            tiles[72].Owner = p2.ConnectionId;
            tiles[72].Status = TileStatus.UserActive;
            p2.ActiveTiles.Add(tiles[72]);

            for (var i = 27; i < 41; i++)
            {
                if (i == 38)
                {
                    continue;
                }
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 53; i < 66; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);
            }
        }
        
        private void DistributeTilesForSelfKong(Game game)
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
                if (i == 3 || i == 5)
                {
                    continue;
                }
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p1.ActiveTiles.Add(tiles[i]);

            }
            tiles[38].Owner = p1.ConnectionId;
            tiles[38].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[38]);

            tiles[72].Owner = p1.ConnectionId;
            tiles[72].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[72]);

            tiles[106].Owner = p1.ConnectionId;
            tiles[106].Status = TileStatus.JustPicked;
            p1.ActiveTiles.Add(tiles[106]);


            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 27; i < 41; i++)
            {
                if (i == 38)
                {
                    continue;
                }
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 53; i < 66; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);
            }
        }

        private void DistributeTilesForSelfKongToPong(Game game)
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
                if (i == 2 || i == 3 || i == 4 || i == 5)
                {
                    continue;
                }
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p1.ActiveTiles.Add(tiles[i]);
            }

            List<Tile> tempTile1 = new List<Tile>();
            tempTile1.Add(tiles[4]);
            tempTile1.Add(tiles[38]);
            tempTile1.Add(tiles[72]);
            TileSet newbie1 = new TileSet() { isRevealed = true, Tiles = tempTile1, TileSetType = TileSetType.Pong, TileType = TileType.Money };
            p1.TileSets.Add(newbie1);

            tiles[4].Owner = p1.ConnectionId;
            tiles[4].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[4]);

            tiles[38].Owner = p1.ConnectionId;
            tiles[38].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[38]);

            tiles[72].Owner = p1.ConnectionId;
            tiles[72].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[72]);

            tiles[0].Status = TileStatus.JustPicked;

            tiles[106].Owner = p1.ConnectionId;
            tiles[106].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[106]);

            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 27; i < 41; i++)
            {
                if (i == 38)
                {
                    continue;
                }
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 53; i < 66; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);
            }
        }

        private void DistributeTilesForPong(Game game)
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
                if (i == 3 || i == 5)
                {
                    continue;
                }
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p1.ActiveTiles.Add(tiles[i]);
            }
            tiles[38].Owner = p1.ConnectionId;
            tiles[38].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[38]);

            tiles[74].Owner = p1.ConnectionId;
            tiles[74].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[74]);

            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 27; i < 40; i++)
            {
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 53; i < 66; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);
            }
        }

        private void DistributeTilesForChow(Game game)
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

            for (var i = 0; i < 17; i++)
            {
                if (i == 3 || i == 5 || i == 6)
                {
                    continue;
                }
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p1.ActiveTiles.Add(tiles[i]);
            }

            for (var i = 17; i < 30; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);

            }
            for (var i = 30; i < 43; i++)
            {
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);

            }
            for (var i = 43; i < 56; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);
            }
        }

        private void DistributeTilesForNoWinner(Game game)
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
            }
            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 27; i < 40; i++)
            {
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);
            }
            //tiles[114].Owner = p3.ConnectionId;
            //tiles[114].Status = TileStatus.UserActive;
            //p3.ActiveTiles.Add(tiles[114]);

            for (var i = 40; i < 53; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);
            }

            for (var i = 53; i < 143; i++)
            {
                if (i == 53) continue;
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.BoardGraveyard;
            }
        }

        private void DistributeTilesForWinWaitingForEyeWithMixPureHandWithDragon(Game game)
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

            for (var i = 0; i < 6; i++)
            {
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserGraveyard;
                p1.GraveYardTiles.Add(tiles[i]);
            }
            List<Tile> tempTile1 = new List<Tile>();
            tempTile1.Add(tiles[0]);
            tempTile1.Add(tiles[1]);
            tempTile1.Add(tiles[2]);
            TileSet newbie1 = new TileSet() { isRevealed = true, Tiles = tempTile1, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie1);

            List<Tile> tempTile2 = new List<Tile>();
            tempTile2.Add(tiles[3]);
            tempTile2.Add(tiles[4]);
            tempTile2.Add(tiles[5]);
            TileSet newbie2 = new TileSet() { isRevealed = true, Tiles = tempTile2, TileSetType = TileSetType.Chow, TileType = TileType.Money };
            p1.TileSets.Add(newbie2);

            tiles[27].Owner = p1.ConnectionId;
            tiles[27].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[27]);

            tiles[61].Owner = p1.ConnectionId;
            tiles[61].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[61]);

            tiles[95].Owner = p1.ConnectionId;
            tiles[95].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[95]);

            List<Tile> tempTile3 = new List<Tile>();
            tempTile3.Add(tiles[27]);
            tempTile3.Add(tiles[61]);
            tempTile3.Add(tiles[95]);
            TileSet newbie3 = new TileSet() { isRevealed = true, Tiles = tempTile3, TileSetType = TileSetType.Pong, TileType = TileType.Dragon };
            p1.TileSets.Add(newbie3);

            tiles[28].Owner = p1.ConnectionId;
            tiles[28].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[28]);

            tiles[62].Owner = p1.ConnectionId;
            tiles[62].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[62]);

            tiles[96].Owner = p1.ConnectionId;
            tiles[96].Status = TileStatus.UserGraveyard;
            p1.GraveYardTiles.Add(tiles[96]);

            List<Tile> tempTile4 = new List<Tile>();
            tempTile4.Add(tiles[28]);
            tempTile4.Add(tiles[62]);
            tempTile4.Add(tiles[96]);
            TileSet newbie4 = new TileSet() { isRevealed = true, Tiles = tempTile4, TileSetType = TileSetType.Pong, TileType = TileType.Dragon };
            p1.TileSets.Add(newbie4);

            tiles[121].Owner = p1.ConnectionId;
            tiles[121].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[121]);

            tiles[66].Owner = p1.ConnectionId;
            tiles[66].Status = TileStatus.UserActive;
            p1.ActiveTiles.Add(tiles[66]);

            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p2.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 27; i < 42; i++)
            {
                if (i == 27 || i == 28) continue;
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p3.ActiveTiles.Add(tiles[i]);
            }
            for (var i = 53; i < 68; i++)
            {
                if (i == 61 || i == 62 || i == 66) continue;
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
                p4.ActiveTiles.Add(tiles[i]);
            }
            tiles[80].Owner = p4.ConnectionId;
            tiles[80].Status = TileStatus.UserActive;
            p4.ActiveTiles.Add(tiles[80]);
        }
    }
}