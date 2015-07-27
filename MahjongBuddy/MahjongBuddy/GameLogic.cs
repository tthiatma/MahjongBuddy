using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;
using MahjongBuddy.Models;

namespace MahjongBuddy
{
    public class GameLogic
    {
        public Dictionary<CommandResult, string> CommandResultDictionary { get; set; }

        public PointCalculator PointCalculator { get; set; }

        public GameLogic() {

            CommandResultDictionary = new Dictionary<CommandResult, string>();
            CommandResultDictionary.Add(CommandResult.ValidCommand, "");
            CommandResultDictionary.Add(CommandResult.InvalidPong, "Nothing to pong!");
            CommandResultDictionary.Add(CommandResult.InvalidKong, "Nothing to kong!");
            CommandResultDictionary.Add(CommandResult.InvalidChow, "Nothing to chow!");
            CommandResultDictionary.Add(CommandResult.InvalidPick, "It's not your turn yet");
            CommandResultDictionary.Add(CommandResult.InvalidThrow, "Select one tile to throw");
            CommandResultDictionary.Add(CommandResult.InvalidChowTileType, "Please select a valid tile");
            CommandResultDictionary.Add(CommandResult.InvalidChowNeedTwoTiles, "Select 2 tiles to chow");
            CommandResultDictionary.Add(CommandResult.InvalidPlayer, "Lost information about player");
            CommandResultDictionary.Add(CommandResult.PlayerWin, "You won!!!!");
            CommandResultDictionary.Add(CommandResult.PlayerWinFailed, "No penalty this time");

            PointCalculator = new PointCalculator();
        }

        public CommandResult DoPong(Game game, IEnumerable<int> tiles, Player player)
        {
            if (player != null)
            {
                var activePlayerTiles = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId && t.Status == TileStatus.UserActive);
                var thrownTile = game.LastTile;
                if (thrownTile != null && thrownTile.Owner != player.ConnectionId)
                {
                    var matchedTileTypeAndValue = activePlayerTiles.Where(t => t.Type == thrownTile.Type && t.Value == thrownTile.Value);
                    if (matchedTileTypeAndValue.Count() >= 2)
                    {
                        var playerTilesPong = matchedTileTypeAndValue.Take(2).ToList();
                        playerTilesPong.Add(thrownTile);
                        CommandTileToPlayerGraveyard(game, playerTilesPong, player.ConnectionId);
                        List<Tile> actualTiles = new List<Tile>();
                        foreach (var t in tiles)
                        {
                            actualTiles.Add(game.Board.Tiles.Where(ti => ti.Id == t).FirstOrDefault());
                        }
                        AddTilesToPlayerOpenTileSet(game, actualTiles, player.ConnectionId, TileSetType.Pong);
                        game.WhosTurn = player.ConnectionId;
                        player.CanPickTile = false;
                        player.CanOnlyThrowTile = true;

                        return CommandResult.ValidCommand;
                    }
                    else
                    {
                        return CommandResult.InvalidPong;
                    }
                }
                else
                {
                    return CommandResult.InvalidPong;
                }
            }
            else
            {
                return CommandResult.InvalidPlayer;
            }
        }

        public CommandResult DoChow(Game game, IEnumerable<int> tiles, Player player) 
        {
            if (player != null)
            {
                if (tiles == null || tiles.Count() < 2 || tiles.Count() > 2)
                {
                    return CommandResult.InvalidChowNeedTwoTiles;
                }
                else
                {
                    var playerTiles = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId && t.Status == TileStatus.UserActive);
                    var thrownTile = game.LastTile;

                    if (thrownTile.Type == TileType.Money || thrownTile.Type == TileType.Round || thrownTile.Type == TileType.Stick)
                    {
                        var matchedTileType = playerTiles.Where(t => t.Type == thrownTile.Type);
                        if (matchedTileType.Count() >= 2)
                        {
                            List<Tile> possiblePair = new List<Tile>();
                            foreach (var t in tiles)
                            {
                                var tempTile = game.Board.Tiles.Where(tt => tt.Id == t).First();
                                if (tempTile != null)
                                {
                                    possiblePair.Add(tempTile);
                                }
                            }
                            possiblePair.Add(thrownTile);

                            var sortedList = possiblePair.OrderBy(o => o.Value).ToArray();

                            //check if its straight
                            if (sortedList[0].Value + 1 == sortedList[1].Value && sortedList[1].Value + 1 == sortedList[2].Value)
                            {
                                CommandTileToPlayerGraveyard(game, sortedList, player.ConnectionId);
                                AddTilesToPlayerOpenTileSet(game, sortedList, player.ConnectionId, TileSetType.Chow);
                                game.WhosTurn = player.ConnectionId;
                                player.CanPickTile = false;
                                player.CanOnlyThrowTile = true;

                                return CommandResult.ValidCommand;
                            }
                            else
                            {
                                return CommandResult.InvalidChow;
                            }
                        }
                        else
                        {
                            return CommandResult.InvalidChow;
                        }
                    }
                    else
                    {
                        return CommandResult.InvalidChowTileType;
                    }
                }
            }
            else 
            {
                return CommandResult.InvalidPlayer;
            }
        }

        public CommandResult DoPickNewTile(Game game, Player player) 
        {
            if (player != null)
            {
                for (var i = 0; i < 8; i++)
                {
                    var newTileForPlayer = game.Board.Tiles.Where(t => t.Owner == "board").FirstOrDefault();

                    if (newTileForPlayer != null)
                    {
                        if (newTileForPlayer.Type == TileType.Flower)
                        {
                            List<Tile> ft = new List<Tile>();
                            ft.Add(newTileForPlayer);
                            CommandTileToPlayerGraveyard(game, ft, player.ConnectionId, replaceTile: false);
                        }
                        else
                        {
                            newTileForPlayer.Owner = player.ConnectionId;
                            newTileForPlayer.Status = TileStatus.JustPicked;
                            break;
                        }
                    }
                }
                SetPlayerCanPickTile(game, player.ConnectionId, false);

                return CommandResult.ValidCommand;
            }
            else 
            {
                return CommandResult.InvalidPlayer;
            }
        }

        public CommandResult DoThrowTile(Game game, IEnumerable<int> tiles, Player player) 
        {
            if (player != null)
            {
                if (tiles.Count() > 0 && tiles.Count() < 2)
                {
                    var tileToThrow = game.Board.Tiles.Where(t => t.Id == tiles.First()).First();
                    tileToThrow.Status = TileStatus.BoardGraveyard;
                    game.LastTile = tileToThrow;

                    var justPickedTile = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId && t.Status == TileStatus.JustPicked).FirstOrDefault();
                    if (justPickedTile != null)
                    {
                        justPickedTile.Status = TileStatus.UserActive;
                    }
                    return CommandResult.ValidCommand;
                }
                else
                {
                    return CommandResult.InvalidThrow;
                }
            }
            else
            {
                return CommandResult.InvalidPlayer;
            }
        }

        public CommandResult DoKong(Game game, IEnumerable<int> tiles, Player player) 
        {
            if (player != null)
            {
                var activePlayerTiles = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId && t.Status == TileStatus.UserActive);
                var thrownTile = game.LastTile;
                if (thrownTile != null)
                {
                    var matchedTileTypeAndValue = activePlayerTiles.Where(t => t.Type == thrownTile.Type && t.Value == thrownTile.Value);
                    if (matchedTileTypeAndValue.Count() == 3)
                    {
                        var playerTilesKong = matchedTileTypeAndValue.Take(3).ToList();
                        playerTilesKong.Add(thrownTile);
                        CommandTileToPlayerGraveyard(game, playerTilesKong, player.ConnectionId);

                        List<Tile> actualTiles = new List<Tile>();
                        foreach (var t in tiles)
                        {
                            actualTiles.Add(game.Board.Tiles.Where(ti => ti.Id == t).FirstOrDefault());
                        }
                        AddTilesToPlayerOpenTileSet(game, actualTiles, player.ConnectionId, TileSetType.Kong);

                        game.WhosTurn = player.ConnectionId;
                        var pp = GetPlayerByConnectionId(game, player.ConnectionId);
                        pp.CanPickTile = false;
                        pp.CanOnlyThrowTile = true;

                        return CommandResult.ValidCommand;
                    }
                    else
                    {
                        return CommandResult.InvalidKong;
                    }
                }
                else
                {
                    return CommandResult.InvalidKong;
                }
            }
            else 
            {
                return CommandResult.InvalidPlayer;
            }
        }

        public CommandResult DoWin(Game game, Player player)
        {
            if (player != null)
            {
                var playerTiles = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId);

                var totalPoints = PointCalculator.GetTotalPoints(game, playerTiles, player);

                if (totalPoints >= 3)
                {
                    return CommandResult.PlayerWin;
                }
                else
                {
                    return CommandResult.PlayerWinFailed;
                }
            }
            else
            {
                return CommandResult.InvalidPlayer;
            }
        }

        public void CommandTileToPlayerGraveyard(Game game, IEnumerable<Tile> tiles, string playerConnectionId, bool replaceTile = false)
        {
            foreach (var f in tiles)
            {
                var tileToGraveyard = game.Board.Tiles.Where(t => t.Id == f.Id).First();
                tileToGraveyard.Status = TileStatus.UserGraveyard;
                tileToGraveyard.Owner = playerConnectionId;
                f.OpenTileCounter = game.TileCounter;
                game.TileCounter++;

                if (replaceTile)
                {
                    var newTileForPlayer = game.Board.Tiles.Where(t => t.Owner == "board").First();
                    newTileForPlayer.Owner = playerConnectionId;
                    newTileForPlayer.Status = TileStatus.UserActive;
                }
            }
        }

        public void RecycleInitialFlower(Game game)
        {
            var p1 = game.Player1;
            var p2 = game.Player2;
            var p3 = game.Player3;
            var p4 = game.Player4;

            for (var i = 0; i < 8; i++)
            {
                var playerTilesFlower = game.Board.Tiles.Where(t => t.Owner == p1.ConnectionId && t.Type == TileType.Flower && t.Status == TileStatus.UserActive);

                if (playerTilesFlower.Count() > 0)
                {
                    CommandTileToPlayerGraveyard(game, playerTilesFlower, p1.ConnectionId, replaceTile: true);
                }
            }

            for (var i = 0; i < 8; i++)
            {
                var playerTilesFlower = game.Board.Tiles.Where(t => t.Owner == p2.ConnectionId && t.Type == TileType.Flower && t.Status == TileStatus.UserActive);

                if (playerTilesFlower.Count() > 0)
                {
                    CommandTileToPlayerGraveyard(game, playerTilesFlower, p2.ConnectionId, replaceTile: true);
                }
            }

            for (var i = 0; i < 8; i++)
            {
                var playerTilesFlower = game.Board.Tiles.Where(t => t.Owner == p3.ConnectionId && t.Type == TileType.Flower && t.Status == TileStatus.UserActive);

                if (playerTilesFlower.Count() > 0)
                {
                    CommandTileToPlayerGraveyard(game, playerTilesFlower, p3.ConnectionId, replaceTile: true);
                }
            }

            for (var i = 0; i < 8; i++)
            {
                var playerTilesFlower = game.Board.Tiles.Where(t => t.Owner == p4.ConnectionId && t.Type == TileType.Flower && t.Status == TileStatus.UserActive);

                if (playerTilesFlower.Count() > 0)
                {
                    CommandTileToPlayerGraveyard(game, playerTilesFlower, p4.ConnectionId, replaceTile: true);
                }
            }
        }

        public void SetPlayerWinds(Game game)
        {
            var gameNo = game.GameCount;
            if (gameNo == 1 || gameNo == 5 || gameNo == 9 || gameNo == 13)
            {
                game.Player1.Wind = WindDirection.East;
                game.Player2.Wind = WindDirection.South;
                game.Player3.Wind = WindDirection.West;
                game.Player4.Wind = WindDirection.North;
            }
            else if (gameNo == 2 || gameNo == 6 || gameNo == 10 || gameNo == 14)
            {
                game.Player1.Wind = WindDirection.North;
                game.Player2.Wind = WindDirection.East;
                game.Player3.Wind = WindDirection.South;
                game.Player4.Wind = WindDirection.West;
            }
            else if (gameNo == 3 || gameNo == 7 || gameNo == 11 || gameNo == 15)
            {
                game.Player1.Wind = WindDirection.West;
                game.Player2.Wind = WindDirection.North;
                game.Player3.Wind = WindDirection.East;
                game.Player4.Wind = WindDirection.South;
            }
            else if (gameNo == 4 || gameNo == 8 || gameNo == 12 || gameNo == 16)
            {
                game.Player1.Wind = WindDirection.South;
                game.Player2.Wind = WindDirection.West;
                game.Player3.Wind = WindDirection.North;
                game.Player4.Wind = WindDirection.East;
            }
        }

        public void SetNextPLayerTurn(Game game)
        {
            if (game != null)
            {
                if (game.WhosTurn == game.Player1.ConnectionId)
                {
                    game.WhosTurn = game.Player2.ConnectionId;
                    game.Player2.CanPickTile = true;
                }
                else if (game.WhosTurn == game.Player2.ConnectionId)
                {
                    game.WhosTurn = game.Player3.ConnectionId;
                    game.Player3.CanPickTile = true;
                }
                else if (game.WhosTurn == game.Player3.ConnectionId)
                {
                    game.WhosTurn = game.Player4.ConnectionId;
                    game.Player4.CanPickTile = true;
                }
                else if (game.WhosTurn == game.Player4.ConnectionId)
                {
                    game.WhosTurn = game.Player1.ConnectionId;
                    game.Player1.CanPickTile = true;
                }
            }
        }

        private void AddTilesToPlayerOpenTileSet(Game game, IEnumerable<Tile> tiles, string playerConnectionId, TileSetType type)
        {
            var player = GetPlayerByConnectionId(game, playerConnectionId);
            var temp = new OpenTileSet()
            {
                Tiles = tiles,
                TileType = type
            };

            player.WinningTileSet.OpenedTiles.Add(temp);
        }

        private Player GetPlayerByConnectionId(Game game, string connectionId)
        {
            if (game.Player1.ConnectionId == connectionId)
            {
                return game.Player1;
            }
            else if (game.Player2.ConnectionId == connectionId)
            {
                return game.Player2;
            }
            else if (game.Player3.ConnectionId == connectionId)
            {
                return game.Player3;
            }
            else if (game.Player4.ConnectionId == connectionId)
            {
                return game.Player4;
            }
            else
            {
                return null;
            }
        }

        private void SetPlayerCanPickTile(Game game, string conId, bool flag)
        {
            if (game.Player1.ConnectionId == conId)
            {
                game.Player1.CanPickTile = flag;
            }
            else if (game.Player2.ConnectionId == conId)
            {
                game.Player2.CanPickTile = flag;
            }
            else if (game.Player3.ConnectionId == conId)
            {
                game.Player3.CanPickTile = flag;
            }
            else if (game.Player4.ConnectionId == conId)
            {
                game.Player4.CanPickTile = flag;
            }
        }
    }
}