using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MahjongBuddy.Models;
using System.Threading.Tasks;
using MahjongBuddy.Extensions;
using System.Collections;
namespace MahjongBuddy
{
    public class GameHub : Hub
    {
        public override Task OnConnected()
        {
            var playerCount = GameState.Instance.Players.Count();
            Clients.All.updatePlayerCount(playerCount);
            return base.OnConnected();
        }

        private GameLogic _gameLogic;

        public GameLogic GameLogic {
            get 
            {
                if (_gameLogic == null)
                {
                    _gameLogic = new GameLogic();
                }
                return _gameLogic;
            }
        }

        public void ResetGame()
        {
            var game = GameState.Instance.FindGameByGroupName("mjbuddy");
            if (game != null)
            {
                GameState.Instance.ResetGame(game);
                var playerCount = GameState.Instance.Players.Count();
                Clients.All.updatePlayerCount(playerCount);
            }
        }
        
        public async void Join(string userName, string groupName)
        {
            var player = GameState.Instance.GetPlayer(userName);
            if (player != null)
            {
                Clients.Caller.playerExists();
            }


            player = GameState.Instance.CreatePlayer(userName, groupName);
            player.ConnectionId = Context.ConnectionId;
            Clients.Caller.name = player.Name;
            Clients.Caller.hash = player.Hash;
            Clients.Caller.id = player.Id;

            await Groups.Add(Context.ConnectionId, groupName);
            var playerCount = GameState.Instance.Players.Count();
            Clients.All.updatePlayerCount(playerCount);
            Clients.OthersInGroup(groupName).notifyUserInGroup(userName + " joined.");                        

            StartGame(player);
        }

        public void StartNextGame(string group)
        {
            var game = GameState.Instance.FindGameByGroupName(group);
            if (game != null)
            {
                //if there's winner from game jz now
                if (game.Records != null && game.Records.Last().NoWinner == false)
                {
                    if (game.Records != null && game.Records.Last().Winner != null)
                    {
                        if (game.DiceRoller.ConnectionId != game.Records.Last().Winner.ConnectionId)
                        {
                            game.DiceMovedCount++;
                            GameLogic.SetNextGamePlayerToStart(game);
                            GameLogic.SetGameNextWind(game);
                            GameLogic.SetPlayerWinds(game);
                        }
                    }
                }

                GameState.Instance.ResetForNextGame(game);
                DistributeTiles(game);
                
                if (game.GameSetting.SkipInitialFlowerSwapping)
                {
                    GameLogic.RecycleInitialFlower(game);
                }
                game.TilesLeft = game.Board.Tiles.Where(t => t.Owner == "board").Count();
                Clients.Group(group).startNextGame(game);
            }
        }

        private bool StartGame(Player player)
        {
            if (player != null)
            {
                Player player1;
                Player player2;
                Player player3;
                Player player4;

                var pepInGroup = GameState.Instance.Players.Where(p => p.Value.Group == player.Group && p.Value.Id != player.Id);

                if (pepInGroup.Count() == 3)
                {
                    Clients.Group(player.Group).gameStarted();
                    var arrayOfPep = pepInGroup.ToArray();
                    Random random = new Random();
                    //int randomPlayerToStart = 1;

                    int randomPlayerToStart = random.Next(1, 4);
                    
                    switch (randomPlayerToStart)
                    { 
                        case 1:
                            player1 = player;
                            player2 = arrayOfPep[0].Value;
                            player3 = arrayOfPep[1].Value;
                            player4 = arrayOfPep[2].Value;
                            break;
                        case 2:
                            player1 = arrayOfPep[0].Value;
                            player2 = arrayOfPep[1].Value;
                            player3 = arrayOfPep[2].Value;
                            player4 = player;
                            break;
                        case 3:
                            player1 = arrayOfPep[1].Value;
                            player2 = arrayOfPep[2].Value;
                            player3 = player;
                            player4 = arrayOfPep[0].Value;
                            break;

                        case 4:
                            player1 = arrayOfPep[2].Value;
                            player2 = player;
                            player3 = arrayOfPep[0].Value;
                            player4 = arrayOfPep[1].Value;
                            break;

                        default:
                            player1 = player;
                            player2 = arrayOfPep[0].Value;
                            player3 = arrayOfPep[1].Value;
                            player4 = arrayOfPep[2].Value;
                            break;
                    }

                    var game = GameState.Instance.FindGameByGroupName(player.Group);

                    if (game != null)
                    {
                        Clients.Group(player.Group).startGame(game);
                        return true;
                    }

                    game = GameState.Instance.CreateGame(player1, player2, player3, player4, player.Group);
                    game.PlayerTurn = player1;
                    game.DiceRoller = player1;
                    game.DiceMovedCount = 1;
                    game.TileCounter = 0;
                    game.CurrentWind = WindDirection.East;

                    game.Board.Tiles.Shuffle();
                    //DistributeTilesForNoWinner(game);
                    DistributeTiles(game);

                    game.GameSetting.SkipInitialFlowerSwapping = true;

                    if (game.GameSetting.SkipInitialFlowerSwapping)
                    {
                        GameLogic.RecycleInitialFlower(game);
                    }
                    game.TilesLeft = game.Board.Tiles.Where(t => t.Owner == "board").Count();
                    GameLogic.SetPlayerWinds(game);

                    Clients.Group(player.Group).startGame(game);

                    //DistributeTilesForWin(game.Board.Tiles, player, player2, player3, player4);
                    //DistributeTilesForChow(game.Board.Tiles, player, player2, player3, player4);
                    //DistributeTilesForPong(game.Board.Tiles, player, player2, player3, player4);
                    //DistributeTilesForKong(game.Board.Tiles, player, player2, player3, player4);
                    return true;
                }
                else
                {
                    var remainingPep = 3 - pepInGroup.Count();
                    Clients.Caller.waitingList(string.Format("need {0} more player(s)", remainingPep));
                    return false;
                }
            }
            return false;
        }

        public void PlayerMove(string group, string command, IEnumerable<int> tiles)
        {
            CommandResult cr = CommandResult.ValidCommand;
            bool switchTurn = false;
            string invalidMessage = "shit went wrong...";
            var game = GameState.Instance.FindGameByGroupName(group);
            var userName = Clients.Caller.name;
            var player = GameState.Instance.GetPlayer(userName);

            switch (command)
            {
                //TODO 
                //case "flower":
                //    cr = CommandFlower(game);
                //    break;

                //TODO
                //case "noflower":
                //    cr = CommandNoFlower(game);
                //    switchTurn = true;
                //    break;

                case "pick":                    
                    cr = GameLogic.DoPickNewTile(game, player);;
                    break;

                case "throw":
                    cr = GameLogic.DoThrowTile(game, tiles, player);
                    switchTurn = true;
                   break;

                case "chow":
                    cr = GameLogic.DoChow(game, tiles, player);
                    break;

                case "pong":
                    cr = GameLogic.DoPong(game, tiles, player);
                    break;

                case "kong":
                    cr = GameLogic.DoKong(game, tiles, player);
                    break;

                case "win":
                    cr = GameLogic.DoWin(game, player);
                    break;

                case "giveup":
                    cr = CommandResult.NobodyWin;
                    break;
            }
            invalidMessage = GameLogic.CommandResultDictionary[cr];
            if (cr == CommandResult.ValidCommand)
            {
                game.TilesLeft = game.Board.Tiles.Where(t => t.Owner == "board").Count();

                if (switchTurn)
                {
                    GameLogic.SetNextPlayerTurn(game);                
                }
                Clients.Group(group).updateGame(game);           
            }
            else if (cr == CommandResult.PlayerWin)
            {
                Clients.Group(group).showWinner(game);
            }
            else if (cr == CommandResult.NobodyWin)
            {
                Record rec = new Record();
                rec.NoWinner = true;
                game.Records.Add(rec);
                Clients.Group(group).showNoWinner(game);            
            }
            else
            {
                Clients.Caller.alertUser(invalidMessage);
            }
        }

        //all of this belong to test section
        //TODO : move this to test
        private void DistributeTilesForWin(Game game) 
        {
            List<Tile> tiles = game.Board.Tiles;
            Player p1, p2, p3, p4;
            p1 = game.Player1;
            p2 = game.Player2;
            p3 = game.Player3;
            p4 = game.Player4;

            for (var i = 0; i < 13; i++)
            {
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
            tiles[46].Owner = p1.ConnectionId;
            tiles[46].Status = TileStatus.UserActive;
            tiles[136].Owner = p1.ConnectionId;
            tiles[136].Status = TileStatus.UserGraveyard;

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
            for (var i = 53; i < 66; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
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
            TileSet newbie1 = new TileSet() { isRevealed = true, Tiles = tempTile1, TileSetType = TileSetType.Chow, TileType = TileType.Money};
            p1.TileSets.Add(newbie1);

            List<Tile> tempTile2 = new List<Tile>();
            tempTile2.Add(tiles[3]);
            tempTile2.Add(tiles[4]);
            tempTile2.Add(tiles[5]);
            TileSet newbie2 = new TileSet() { isRevealed = true, Tiles = tempTile2, TileSetType = TileSetType.Chow, TileType = TileType.Money};
            p1.TileSets.Add(newbie2);

            List<Tile> tempTile3 = new List<Tile>();
            tempTile3.Add(tiles[6]);
            tempTile3.Add(tiles[7]);
            tempTile3.Add(tiles[8]);
            TileSet newbie3 = new TileSet() { isRevealed = true, Tiles = tempTile3, TileSetType = TileSetType.Chow, TileType = TileType.Money};
            p1.TileSets.Add(newbie3);

            List<Tile> tempTile4 = new List<Tile>();
            tempTile4.Add(tiles[9]);
            tempTile4.Add(tiles[10]);
            tempTile4.Add(tiles[11]);
            TileSet newbie4 = new TileSet() { isRevealed = true, Tiles = tempTile4, TileSetType = TileSetType.Chow, TileType = TileType.Round};
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
            Player p1, p2, p3, p4;
            p1 = game.Player1;
            p2 = game.Player2;
            p3 = game.Player3;
            p4 = game.Player4;

            for (var i = 0; i < 14; i++)
            {
                if (i == 3 || i == 5)
                {
                    continue;
                }
                tiles[i].Owner = p1.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
            tiles[38].Owner = p1.ConnectionId;
            tiles[38].Status = TileStatus.UserActive;
            tiles[72].Owner = p1.ConnectionId;
            tiles[72].Status = TileStatus.UserActive;
            tiles[106].Owner = p1.ConnectionId;
            tiles[106].Status = TileStatus.JustPicked;

            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
            for (var i = 27; i < 41; i++)
            {
                if (i == 38)
                {
                    continue;
                }
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
            for (var i = 53; i < 66; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
        }

        private void DistributeTilesForPong(List<Tile> tiles, Player p1, Player p2, Player p3, Player p4)
        {
            for (var i = 0; i < 14; i++)
            {
                if (i == 3 || i == 5)
                {
                    continue;
                }
                tiles[i].Owner = p1.ConnectionId;
            }
            tiles[38].Owner = p1.ConnectionId;
            tiles[74].Owner = p1.ConnectionId;

            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
            }
            for (var i = 27; i < 40; i++)
            {
                tiles[i].Owner = p3.ConnectionId;
            }
            for (var i = 53; i < 66; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
            }
        }

        private void DistributeTilesForChow(List<Tile> tiles, Player p1, Player p2, Player p3, Player p4)
        {

            for (var i = 0; i < 17; i++)
            {
                if (i == 3 || i == 5 || i == 6)
                {
                    continue;
                }
                tiles[i].Owner = p1.ConnectionId;
            }

            for (var i = 14; i < 27; i++)
            {
                tiles[i].Owner = p2.ConnectionId;
            }
            for (var i = 27; i < 40; i++)
            {
                tiles[i].Owner = p3.ConnectionId;
            }
            for (var i = 40; i < 53; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
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

        private void DistributeTilesForNoWinner(Game game)
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
                if (i == 39)
                    continue;
                tiles[i].Owner = p3.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }
            tiles[114].Owner = p3.ConnectionId;
            tiles[114].Status = TileStatus.UserActive;

            for (var i = 40; i < 53; i++)
            {
                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.UserActive;
            }

            for (var i = 54; i < tiles.Count-1; i++)
            {
                if (i == 114)
                    continue;

                tiles[i].Owner = p4.ConnectionId;
                tiles[i].Status = TileStatus.BoardGraveyard;
            }
        }
    }
}