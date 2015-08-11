using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            CommandResultDictionary.Add(CommandResult.SomethingWentWrong, "Something went wrong!");
            CommandResultDictionary.Add(CommandResult.InvalidWin, "Can't win with this");
            

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
                    var tileToThrow = game.Board.Tiles.Where(t => t.Id == tiles.First()).FirstOrDefault();
                    if (tileToThrow != null)
                    {
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
                int totalPoints = 0;
                var playerTiles = game.Board.Tiles
                    .Where(t => 
                        t.Owner == player.ConnectionId 
                        && (t.Status == TileStatus.UserActive || t.Status == TileStatus.JustPicked));

                var tilesToTestForwin = playerTiles.ToList();
                bool isPossibleToGetWeirdWinningSet = false;

                if (playerTiles.Count() == 14)
                {
                    isPossibleToGetWeirdWinningSet = true;
                }
                else if (playerTiles.Count() == 13)
                {
                    isPossibleToGetWeirdWinningSet = true;
                    tilesToTestForwin.Add(game.LastTile);
                }
                else 
                {
                    isPossibleToGetWeirdWinningSet = false;                
                }

                if (isPossibleToGetWeirdWinningSet)
                {
                    //check 13 wonder
                    var thirteenWonderTiles = TileBuilder.BuildThirteenWonder();
                    List<Tile> tempTilesToCheck13Wonders = new List<Tile>();
                    bool foundEyesFor13Wonders = false;
                    bool is13Wonders = true;
                    foreach (var t in thirteenWonderTiles)
                    {
                        var dTile = tilesToTestForwin.Where(dt => dt.Type == t.Type && dt.Value == t.Value);

                        if (dTile != null)
                        {
                            if (foundEyesFor13Wonders)
                            {
                                if (dTile.Count() != 1)
                                {
                                    is13Wonders = false;
                                    break;
                                }
                            }
                            else
                            {
                                if (dTile.Count() == 2)
                                {
                                    foundEyesFor13Wonders = true;
                                }
                                else if (dTile.Count() > 2)
                                {
                                    is13Wonders = false;
                                    break;
                                }
                            }

                            foreach (var tt in dTile)
                            {
                                tempTilesToCheck13Wonders.Add(tt);
                            }
                        }
                        else
                        {
                            is13Wonders = false;
                            break;
                        }
                    }
                    if (is13Wonders && tempTilesToCheck13Wonders.Count() == 14)
                    {
                        //it's 13 wonder set!
                        totalPoints += 10;
                        return CommandResult.PlayerWin;
                    }
                    else
                    {
                        //check al pairs      
                        bool isAllPair = true;
                        foreach (var t in tilesToTestForwin)
                        {
                            var pairTiles = tilesToTestForwin.Where(tt => tt.Type == t.Type && tt.Value == t.Value);
                            if (pairTiles != null)
                            {
                                if (pairTiles.Count() == 1 || pairTiles.Count() == 3)
                                {
                                    isAllPair = false;
                                }
                            }
                        }
                        if (isAllPair)
                        {
                            //TODO need to check flower
                            var playerFlowerTiles = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId && t.Type == TileType.Flower);

                            if (playerFlowerTiles != null)
                            {
                                totalPoints += PointCalculator.CalculateFlower(game, playerFlowerTiles, player);
                            }
                            totalPoints += 7;
                            return CommandResult.PlayerWin;
                        }
                    }
                }
 
                var winningSet = BuildWinningTiles(tilesToTestForwin, player.TileSets);
                if (winningSet != null)
                {
                    var winningTypes = PointCalculator.GetWinningType(game, playerTiles, player);
                    int tempPts = 0;

                    if (winningTypes.Count() > 0)
                    {
                        foreach (var item in winningTypes)
                        {
                            tempPts += game.PointSystem[item];
                        }
                    }

                    totalPoints = PointCalculator.GetTotalPoints(game, playerTiles, player);

                    if (tempPts >= 3)
                    {
                        //TODO = add the winning types to the game record
                        return CommandResult.PlayerWin;
                    }
                    else
                    {
                        return CommandResult.PlayerWinFailed;
                    }
                }
                else
                {
                    return CommandResult.InvalidWin;
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

        public void SetGameNextWind(Game game)
        {
            if (game.CurrentWind == WindDirection.East)
            {
                game.CurrentWind = WindDirection.South;
            }
            else if (game.CurrentWind == WindDirection.South)
            {
                game.CurrentWind = WindDirection.West;
            }
            else if (game.CurrentWind == WindDirection.West)
            {
                game.CurrentWind = WindDirection.North;
            }
            else
            {
                game.CurrentWind = WindDirection.East;
            }
        }
        
        public void SetPlayerWinds(Game game)
        {
            var diceMovedCount = game.DiceMovedCount;
            if (diceMovedCount == 1 || diceMovedCount == 5 || diceMovedCount == 9 || diceMovedCount == 13)
            {
                game.Player1.Wind = WindDirection.East;
                game.Player2.Wind = WindDirection.South;
                game.Player3.Wind = WindDirection.West;
                game.Player4.Wind = WindDirection.North;
            }
            else if (diceMovedCount == 2 || diceMovedCount == 6 || diceMovedCount == 10 || diceMovedCount == 14)
            {
                game.Player1.Wind = WindDirection.North;
                game.Player2.Wind = WindDirection.East;
                game.Player3.Wind = WindDirection.South;
                game.Player4.Wind = WindDirection.West;
            }
            else if (diceMovedCount == 3 || diceMovedCount == 7 || diceMovedCount == 11 || diceMovedCount == 15)
            {
                game.Player1.Wind = WindDirection.West;
                game.Player2.Wind = WindDirection.North;
                game.Player3.Wind = WindDirection.East;
                game.Player4.Wind = WindDirection.South;
            }
            else if (diceMovedCount == 4 || diceMovedCount == 8 || diceMovedCount == 12 || diceMovedCount == 16)
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

        public WinningTileSet BuildWinningTiles(IEnumerable<Tile> playerTiles, List<TileSet> tilesets) 
        {
            WinningTileSet ret = new WinningTileSet();
            bool tileSetIsLegit = false;
            var tiles = playerTiles.Where(t => t.Type != TileType.Flower);

            if (tilesets != null && tilesets.Count > 0)
            {
                for (int i = 0; i < tilesets.Count(); i++)
                {
                    ret.Sets[i] = tilesets[i];
                }            
            }

            //get list of possible eyes
            List<IEnumerable<Tile>> eyeCollection = new List<IEnumerable<Tile>>();
            foreach (var t in tiles)
            {
                var sameTiles = tiles.Where(ti => ti.Value == t.Value && ti.Type == t.Type);
                if (sameTiles != null && sameTiles.Count() > 1)
                {
                    eyeCollection.Add(sameTiles.Take(2));                            
                }            
            }

            //take out the eyes from the tiles and try to build winning collection
            foreach (var eyes in eyeCollection)
            {
                //remove possible eyes from tiles
                var tilesWithoutEyes = tiles.ToList();
                foreach (var t in eyes) 
                {
                    tilesWithoutEyes.Remove(t);
                }

                TileSet firstSetWinningTileSet;
                if (ret.Sets[0] != null)
                {
                    firstSetWinningTileSet = ret.Sets[0];
                }
                else 
                {
                    firstSetWinningTileSet = GetOneWinningTileSet(tilesWithoutEyes);
                }
                var tilesWithoutFirstSet = tilesWithoutEyes.ToList();

                if (firstSetWinningTileSet.Tiles != null && firstSetWinningTileSet.Tiles.Count() == 3)
                {
                    foreach (var t in firstSetWinningTileSet.Tiles)
                    {
                        tilesWithoutFirstSet.Remove(t);
                    }                
                }

                TileSet secondSetWinningTileSet;
                if (ret.Sets[1] != null)
                {
                    secondSetWinningTileSet = ret.Sets[1];
                }
                else
                {
                    secondSetWinningTileSet = GetOneWinningTileSet(tilesWithoutFirstSet);
                }

                var tilesWithoutSecondSet = tilesWithoutFirstSet.ToList();

                if (secondSetWinningTileSet.Tiles != null && secondSetWinningTileSet.Tiles.Count() == 3)
                {
                    foreach (var t in secondSetWinningTileSet.Tiles)
                    {
                        tilesWithoutSecondSet.Remove(t);
                    }                
                }

                TileSet thirdSetWinningTileSet;
                if (ret.Sets[2] != null)
                {
                    thirdSetWinningTileSet = ret.Sets[2];
                }
                else
                {
                    thirdSetWinningTileSet = GetOneWinningTileSet(tilesWithoutSecondSet);
                }

                var tilesWithoutThirdSet = tilesWithoutSecondSet.ToList();

                if (thirdSetWinningTileSet.Tiles != null && thirdSetWinningTileSet.Tiles.Count() == 3)
                {
                    foreach (var t in thirdSetWinningTileSet.Tiles)
                    {
                        tilesWithoutThirdSet.Remove(t);
                    }
                }

                TileSet fourthSetWinningTileSet;
                if (ret.Sets[3] != null)
                {
                    fourthSetWinningTileSet = ret.Sets[3];
                }
                else
                {
                    fourthSetWinningTileSet = GetOneWinningTileSet(tilesWithoutThirdSet);
                }


                if (fourthSetWinningTileSet.Tiles != null && fourthSetWinningTileSet.Tiles.Count() == 3)
                {
                    tileSetIsLegit = true;
                }

                if (tileSetIsLegit)
                {
                    ret.Eye.Tiles = eyes;
                    ret.Sets[0] = firstSetWinningTileSet;
                    ret.Sets[1] = secondSetWinningTileSet;
                    ret.Sets[2] = thirdSetWinningTileSet;
                    ret.Sets[3] = fourthSetWinningTileSet;
                    break;
                }
            }
            if (!tileSetIsLegit)
            {
                ret = null;
            }

            return ret;
        }

        private TileSet GetOneWinningTileSet(IEnumerable<Tile> tiles)
        {
            TileSet ret = new TileSet();

            //try get straight tile
            foreach (var t in tiles)
            {
                var temp = FindStraightTiles(t, tiles);
                if (temp != null && temp.Count() == 3)
                {
                    ret.Tiles = temp;
                    ret.TileType = TileSetType.Chow;
                    ret.isRevealed = false;
                    return ret;
                }
            }

            //if no straight tile, try get pong tile
            foreach (var t in tiles)
            {
                var temp = FindPongTiles(t, tiles);
                if (temp != null && temp.Count() == 3)
                {
                    ret.Tiles = temp;
                    ret.TileType = TileSetType.Pong;
                    ret.isRevealed = false;
                    return ret;
                }
            }
            
            return ret;
        }

        private void AddTilesToPlayerOpenTileSet(Game game, IEnumerable<Tile> tiles, string playerConnectionId, TileSetType type)
        {
            var player = GetPlayerByConnectionId(game, playerConnectionId);
            var temp = new TileSet()
            {
                Tiles = tiles,
                TileType = type,
                isRevealed = true
            };

            player.TileSets.Add(temp);
        }

        private List<Tile> FindPongTiles(Tile theTile, IEnumerable<Tile> tiles)
        {
            var ret = new List<Tile>();
            var sameTiles = tiles.Where(t => t.Type == theTile.Type && t.Value == theTile.Value);
            if (sameTiles != null && sameTiles.Count() == 3)
            {
                foreach (var t in sameTiles)
                {
                    ret.Add(t);
                }
            }

            return ret;
        }

        private List<Tile> FindStraightTiles(Tile theTile, IEnumerable<Tile> tiles)
        {
            var ret = new List<Tile>();

            var sameTypeTiles = tiles.Where(t => t.Type == theTile.Type);
            foreach (var t in sameTypeTiles)
            {
                if (sameTypeTiles.Any(ti => ti.Value == (t.Value - 2)) && sameTypeTiles.Any(ti => ti.Value == (t.Value - 1)))
                {
                    ret.Add(t);
                    ret.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value - 2)).First());
                    ret.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value - 1)).First());
                    break;
                }

                if (sameTypeTiles.Any(ti => ti.Value == (t.Value - 1)) && sameTypeTiles.Any(ti => ti.Value == (t.Value + 1)))
                {
                    ret.Add(t);
                    ret.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value - 1)).First());
                    ret.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value + 1)).First());
                    break;
                }

                if (sameTypeTiles.Any(ti => ti.Value == (t.Value + 1)) && sameTypeTiles.Any(ti => ti.Value == (t.Value + 2)))
                {
                    ret.Add(t);
                    ret.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value + 1)).First());
                    ret.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value + 2)).First());
                    break;
                }
            }
            return ret;
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