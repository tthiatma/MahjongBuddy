using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MahjongBuddy.Models;
using System.Threading.Tasks;
using MahjongBuddy.Extensions;
using System.Collections;
using log4net;
namespace MahjongBuddy
{
    public class GameHub : Hub
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GameHub));

        public override Task OnConnected()
        {
            var playerCount = GameState.Instance.Players.Count();
            Clients.All.updatePlayerCount(playerCount);
            return base.OnConnected();
        }

        private Dealer _dealer;

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
                GameState.Instance.StartNextGame(game);
                UpdateClient(game, group);
                Clients.Group(group).startNextGame();                
            }
        }

        private bool StartGame(ActivePlayer player)
        {
            if (player != null)
            {
                ActivePlayer player1;
                ActivePlayer player2;
                ActivePlayer player3;
                ActivePlayer player4;

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

                    UpdateClient(game, player.Group);

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
            try
            {
                CommandResult cr = CommandResult.ValidCommand;
                bool switchTurn = false;
                string invalidMessage = "shit went wrong...";
                var game = GameState.Instance.FindGameByGroupName(group);
                var userName = Clients.Caller.name;
                ActivePlayer player = GameState.Instance.GetPlayer(userName);

                switch (command)
                {
                    //TODO this is to simulate player to manually switching flower tile
                    //case "flower":
                    //    cr = CommandFlower(game);
                    //    break;

                    //TODO this is to simulate player to say no flower tile
                    //case "noflower":
                    //    cr = CommandNoFlower(game);
                    //    switchTurn = true;
                    //    break;

                    case "pick":
                        cr = GameLogic.DoPickNewTile(game, player);
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
                if (GameLogic.CommandResultDictionary.ContainsKey(cr))
                {
                    invalidMessage = GameLogic.CommandResultDictionary[cr];
                }
                else
                {
                    throw new Exception("unable to find invalid message for this command result");
                }
                
                if (cr == CommandResult.ValidCommand)
                {
                    ValidCommand(game, group, switchTurn, player);
                }
                else if (cr == CommandResult.ValidChow || cr == CommandResult.ValidPong || cr == CommandResult.ValidKong)
                { 
                    //TODO what to do for validSelfKong
                    Clients.Group(group).removeBoardTiles();
                    ValidCommand(game, group, switchTurn, player);
                }
                else if (cr == CommandResult.ValidThrow)
                {
                    Clients.Group(group).addBoardTiles(game.LastTile);
                    ValidCommand(game, group, switchTurn, player);
                }
                else if (cr == CommandResult.ValidPick)
                {
                    UpdateCurrentPlayer(game, player);
                }
                else if (cr == CommandResult.PlayerWin)
                {
                    Clients.Group(group).showWinner(game);
                }

                //TODO fix logic when tile left is 0, to be in synch when user throw last tile
                else if (cr == CommandResult.NobodyWin)
                {
                    ValidCommand(game, group, switchTurn, player);
                }
                else
                {
                    Clients.Caller.alertUser(invalidMessage);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            
        }

        public void SetPlayerSortTile(string group, bool autoSort)
        {
            var game = GameState.Instance.FindGameByGroupName(group);
            var userName = Clients.Caller.name;
            var player = GameState.Instance.GetPlayer(userName);
            player.IsTileAutoSort = autoSort;
            if (autoSort)
            {
                GameLogic.AssignPlayersTileIndex(game, player);
                UpdateCurrentPlayer(game, player);
            }
        }

        private void ValidCommand(Game game, string group, bool switchTurn, ActivePlayer player)
        {
            game.TilesLeft = game.Board.Tiles.Where(t => t.Owner == "board").Count();

            //TODO fix logic when tile left is 0, to be in synch when user throw last tile
            if (game.TilesLeft == 0)
            {
                Record rec = new Record();
                rec.NoWinner = true;
                game.Records.Add(rec);
                Clients.Group(group).showNoWinner(game);
            }
            if (switchTurn)
            {
                GameLogic.SetNextPlayerTurn(game);
            }
            UpdateClient(game, player.Group);
        }

        private void UpdateCurrentPlayer(Game game, ActivePlayer player)
        {
            if (_dealer == null) { _dealer = new Dealer(game); };
            Clients.Client(player.ConnectionId).updateCurrentPlayer(player, _dealer);
        }

        /// <summary>
        /// Update all players in the group with all starting information
        /// </summary>
        /// <param name="game"></param>
        /// <param name="player"></param>
        private void UpdateClient(Game game, string group)
        {
            if (_dealer == null) { _dealer = new Dealer(game); };
            Clients.Client(game.Player1.ConnectionId).updateCurrentPlayer(game.Player1, _dealer);
            Clients.Client(game.Player2.ConnectionId).updateCurrentPlayer(game.Player2, _dealer);
            Clients.Client(game.Player3.ConnectionId).updateCurrentPlayer(game.Player3, _dealer);
            Clients.Client(game.Player4.ConnectionId).updateCurrentPlayer(game.Player4, _dealer);
        }
        
       
    }
}