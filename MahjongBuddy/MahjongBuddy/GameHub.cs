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
    [Authorize]
    public class GameHub : Hub
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(GameHub));
        private Dealer _dealer;
        private GameLogic _gameLogic;        

        public override Task OnConnected()
        {
            var playerCount = GameState.Instance.Players.Count();
            Clients.All.updatePlayerCount(playerCount);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var player = GameState.Instance.GetPlayer(userName);
            if (player != null)
            {
                lock(player.ConnectionIds)
                {
                    player.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));

                    //if (!player.ConnectionIds.Any())
                    //{
                    //    ActivePlayer removedPlayer;
                    //    GameState.Instance.Players.TryRemove(userName, out removedPlayer);

                    //    //TODO: create this in client side
                    //    Clients.Others.userDisconnected(userName);
                    //}
                }
            }

            return base.OnDisconnected(stopCalled);
        }

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
            _logger.Debug("ResetGame Called");
            
            var game = GameState.Instance.FindGameByGroupName("mjbuddy");
            if (game != null)
            {
                GameState.Instance.ResetGame(game);
                var playerCount = GameState.Instance.Players.Count();
                Clients.All.updatePlayerCount(playerCount);
            }
        }

        public void ReconnectToGame(string groupName)
        {
            _logger.Debug("ReconnectToGame Called to group " + groupName);
            string userName = Context.User.Identity.Name;
            var player = GameState.Instance.GetPlayer(userName);
            var game = GameState.Instance.FindGameByGroupName(groupName);
            var pepInGroup = GameState.Instance.Players.Where(p => p.Value.Group == player.Group).Count();
            if (game != null)
            {
                if (pepInGroup == 4)
                {
                    Clients.Group(groupName).gameStarted();
                }
                else
                {
                    //TODO: show all connected players
                }
            }
        }

        public async void Join(string groupName)
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            _logger.Debug(string.Format("Player joined. Name: {0} to Group: {1}", userName, groupName));

            var player = GameState.Instance.GetPlayer(userName);
            if (player != null)
            {
                lock(player.ConnectionIds)
                {
                    player.ConnectionIds.Add(connectionId);
                }
                //TODO: need to retrieve user game status and put them back in the game
            }
            else 
            {
                player = GameState.Instance.CreatePlayer(userName, groupName);
                player.ConnectionId = Context.ConnectionId;
                lock(player.ConnectionIds)
                {
                    player.ConnectionIds.Add(connectionId);
                }
                Clients.Caller.name = player.Name;
                Clients.Caller.hash = player.Hash;
                Clients.Caller.id = player.Id;

                await Groups.Add(Context.ConnectionId, groupName);
                var playerCount = GameState.Instance.Players.Count();
                Clients.All.updatePlayerCount(playerCount);
                Clients.All.isPlayerPlaying(player.IsPlaying);
                Clients.OthersInGroup(groupName).notifyUserInGroup(userName + " joined.");

                StartGame(player);            
            }
        }

        public void StartNextGame(string group)
        {
            _logger.Debug("StartNextGame is called");
            var game = GameState.Instance.FindGameByGroupName(group);
            if (game != null)
            {
                _logger.Debug("trying to start next game");
                GameState.Instance.StartNextGame(game);
                UpdateClient(game);
                Clients.Group(group).startNextGame();
            }
            else
            {
                _logger.Error("trying to start next game but game is null");
            }
        }

        private bool StartGame(ActivePlayer player)
        {
            _logger.Debug("StartGame called");
            if (player != null)
            {
                ActivePlayer seat1, seat2, seat3, seat4;
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
                            seat1 = player;
                            seat2 = arrayOfPep[0].Value;
                            seat3 = arrayOfPep[1].Value;
                            seat4 = arrayOfPep[2].Value;
                            break;
                        case 2:
                            seat1 = arrayOfPep[0].Value;
                            seat2 = arrayOfPep[1].Value;
                            seat3 = arrayOfPep[2].Value;
                            seat4 = player;
                            break;
                        case 3:
                            seat1 = arrayOfPep[1].Value;
                            seat2 = arrayOfPep[2].Value;
                            seat3 = player;
                            seat4 = arrayOfPep[0].Value;
                            break;

                        case 4:
                            seat1 = arrayOfPep[2].Value;
                            seat2 = player;
                            seat3 = arrayOfPep[0].Value;
                            seat4 = arrayOfPep[1].Value;
                            break;

                        default:
                            seat1 = player;
                            seat2 = arrayOfPep[0].Value;
                            seat3 = arrayOfPep[1].Value;
                            seat4 = arrayOfPep[2].Value;
                            break;
                    }

                    var game = GameState.Instance.FindGameByGroupName(player.Group);

                    if (game != null)
                    {
                        Clients.Group(player.Group).startGame(game);
                        return true;
                    }
                    else
                    {
                        _logger.Error("trying to find game by group name but failed");
                    }

                    game = GameState.Instance.CreateGame(seat1, seat2, seat3, seat4, player.Group);

                    UpdateClient(game);

                    return true;
                }
                else
                {
                    var remainingPep = 3 - pepInGroup.Count();
                    Clients.Caller.waitingList(string.Format("need {0} more player(s)", remainingPep));
                    return false;
                }
            }
            else
            {
                _logger.Error("Trying to start game but player is null");
            }
            return false;
        }

        public void PlayerMove(string group, string command, IEnumerable<int> tiles)
        {
            _logger.Debug(string.Format("PlayerMove Called. Group: {0}, Command: {1}", group, command));
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
                _logger.Debug("cr is " + cr);

                if (GameLogic.CommandResultDictionary.ContainsKey(cr))
                {
                    invalidMessage = GameLogic.CommandResultDictionary[cr];
                }
                else
                {
                    _logger.Error("unable to get message for this command " + cr);
                    throw new Exception("unable to find invalid message for this command result");
                }
                
                if (cr == CommandResult.ValidCommand)
                {
                    ValidCommand(game, group, switchTurn, player);
                }
                else if (cr == CommandResult.ValidChow || cr == CommandResult.ValidPong || cr == CommandResult.ValidKong)
                { 
                    Clients.Group(group).removeBoardTiles();
                    ValidCommand(game, group, switchTurn, player);
                }
                else if (cr == CommandResult.ValidSelfKong)
                {
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
                _logger.Error(ex);
                Clients.Caller.alertUser("something went wrong: " + ex);
            }            
        }

        /// <summary>
        /// This is an endpoint when player toggle their tile sorting
        /// </summary>
        /// <param name="group"></param>
        /// <param name="autoSort"></param>
        public void SetPlayerSortTile(string group, bool autoSort)
        {
            _logger.Debug("SetPlayerSortTile Called");
            var game = GameState.Instance.FindGameByGroupName(group);
            var userName = Clients.Caller.name;
            var player = GameState.Instance.GetPlayer(userName);
            if (game != null && userName != null && player != null)
            {
                player.IsTileAutoSort = autoSort;
                GameLogic.AssignPlayersTileIndex(game, player);
                UpdateCurrentPlayer(game, player);
            }
            else
            {
                _logger.Error("something went wrong when trying to sort player tile");
            }
        }

        private void ValidCommand(Game game, string group, bool switchTurn, ActivePlayer player)
        {
            game.TilesLeft = game.Board.Tiles.Where(t => t.Owner == "board").Count();

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
            UpdateClient(game);
        }

        private void UpdateCurrentPlayer(Game game, ActivePlayer player)
        {
            if (_dealer == null) { _dealer = new Dealer(game); };
            Clients.Client(player.ConnectionId).updateCurrentPlayer(player, _dealer);
        }

        /// <summary>
        /// Update client side with information that players in need to know
        /// </summary>
        /// <param name="game"></param>
        private void UpdateClient(Game game)
        {
            if (_dealer == null) { _dealer = new Dealer(game); };
            Clients.Client(game.Player1.ConnectionId).updateCurrentPlayer(game.Player1, _dealer);
            Clients.Client(game.Player2.ConnectionId).updateCurrentPlayer(game.Player2, _dealer);
            Clients.Client(game.Player3.ConnectionId).updateCurrentPlayer(game.Player3, _dealer);
            Clients.Client(game.Player4.ConnectionId).updateCurrentPlayer(game.Player4, _dealer);
        }
    }
}