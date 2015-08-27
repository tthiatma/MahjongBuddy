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
            CommandResultDictionary.Add(CommandResult.InvalidWin, "Can't win with this");
            CommandResultDictionary.Add(CommandResult.WinNotEnoughPoint, "Win doesn't meet minimum points");
            CommandResultDictionary.Add(CommandResult.SomethingWentWrong, "Something went wrong!");
            CommandResultDictionary.Add(CommandResult.InvalidPickWentWrong, "Can't pick more tile");
            CommandResultDictionary.Add(CommandResult.NobodyWin, "No one win...sad time");

            PointCalculator = new PointCalculator();
        }

        public CommandResult DoPong(Game game, IEnumerable<int> tiles, ActivePlayer player)
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
                        AddTilesToPlayerOpenTileSet(game, playerTilesPong, player.ConnectionId, TileSetType.Pong);
                        game.HaltMove = true;
                        game.PlayerTurn = player.ConnectionId;
                        player.CanPickTile = false;
                        player.CanThrowTile = true;

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
                                game.HaltMove = true;
                                game.PlayerTurn = player.ConnectionId;
                                player.CanPickTile = false;
                                player.CanThrowTile = true;

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

                //to know how many tiles a player can have, we need to check how many sets/tiles that they have/revealed
                //this section is to prevent player to have more tiles when they pick new tile.
                var playerTileSetCount = player.TileSets.Count();
                var playerActiveTilesCount = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId && t.Status == TileStatus.UserActive).Count();
                bool canPickNewTile = false;
                switch (playerTileSetCount)
                {
                    case 0:
                        if (playerActiveTilesCount == 13)
                            canPickNewTile = true;
                        break;
                    case 1:
                        if (playerActiveTilesCount == 10)
                            canPickNewTile = true;
                        break;

                    case 2:
                        if (playerActiveTilesCount == 7)
                            canPickNewTile = true;
                        break;

                    case 3:
                        if (playerActiveTilesCount == 4)
                            canPickNewTile = true;
                        break;

                    case 4:
                        if (playerActiveTilesCount == 1)
                            canPickNewTile = true;
                        break;
                }

                if (canPickNewTile)
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
                                game.HaltMove = true;
                                break;
                            }
                        }
                    }
                    player.CanPickTile = false;
                    player.CanThrowTile = true;
                   
                    return CommandResult.ValidCommand;
                }
                else
                {
                    return CommandResult.InvalidPickWentWrong;
                }
                
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
                        tileToThrow.OpenTileCounter = game.TileCounter;
                        game.TileCounter++;

                        game.LastTile = tileToThrow;

                        game.HaltMove = false;

                        var justPickedTile = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId && t.Status == TileStatus.JustPicked).FirstOrDefault();
                        if (justPickedTile != null)
                        {
                            justPickedTile.Status = TileStatus.UserActive;
                        }
                        
                        //check remaining tiles when throwing tile
                        var remainingTiles = game.Board.Tiles.Where(t => t.Owner == "board").Count();
                        if (remainingTiles > 0)
                        {
                            return CommandResult.ValidCommand;
                        }
                        else
                        {
                            return CommandResult.NobodyWin;
                        }
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
                //check if user just picked tile
                Tile tileToKong = null;
                var justpickedPlayerTile = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId && t.Status == TileStatus.JustPicked);

                if (justpickedPlayerTile != null && justpickedPlayerTile.Count() > 0)
                {
                    tileToKong = justpickedPlayerTile.FirstOrDefault();
                }
                else
                {
                    tileToKong = game.LastTile;
                }

                var activePlayerTiles = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId && t.Status == TileStatus.UserActive);
                if (tileToKong != null)
                {
                    var matchedTileTypeAndValue = activePlayerTiles.Where(t => t.Type == tileToKong.Type && t.Value == tileToKong.Value);
                    if (matchedTileTypeAndValue.Count() == 3)
                    {
                        var playerTilesKong = matchedTileTypeAndValue.Take(3).ToList();
                        playerTilesKong.Add(tileToKong);
                        
                        CommandTileToPlayerGraveyard(game, playerTilesKong, player.ConnectionId);
                        AddTilesToPlayerOpenTileSet(game, playerTilesKong, player.ConnectionId, TileSetType.Kong);
                        game.HaltMove = true;
                        game.PlayerTurn = player.ConnectionId;
                        var pp = GetPlayerByConnectionId(game, player.ConnectionId);
                        pp.CanPickTile = false;
                        pp.CanThrowTile = true;

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
                    if (game.LastTile != null)
                    {
                        tilesToTestForwin.Add(game.LastTile);
                    }
                }
                else 
                {
                    isPossibleToGetWeirdWinningSet = false;
                    if (game.LastTile != null)
                    {
                        tilesToTestForwin.Add(game.LastTile);                    
                    }
                }

                //weird winning set refers to 13wonders and 7pairs combo
                //weird winning set only works when play have not revealed any of their card
                if (isPossibleToGetWeirdWinningSet)
                {
                    //check 13 wonder
                    var tempTilesToCheck13Wonders = CheckFor13Wonder(tilesToTestForwin);
                    if (tempTilesToCheck13Wonders != null && tempTilesToCheck13Wonders.Count() == 14)
                    {
                        WinningTileSet weirdWinningSet = BuildWinningTilesForWeirdSet(game, player, tempTilesToCheck13Wonders.ToList());
                        CalculateWinning(game, player, weirdWinningSet);
                        AddWinningHand(game, weirdWinningSet, WinningType.ThirteenOrphans);
                        RecordWinning(game, player, weirdWinningSet);
                        return CommandResult.PlayerWin;
                    }
                    else
                    {
                        bool isAllPair = CheckForAllPair(tilesToTestForwin);
                        if (isAllPair)
                        {
                            WinningTileSet weirdWinningSet = BuildWinningTilesForWeirdSet(game, player, tilesToTestForwin.ToList());
                            CalculateWinning(game, player, weirdWinningSet);
                            AddWinningHand(game, weirdWinningSet, WinningType.SevenPairs);
                            RecordWinning(game, player, weirdWinningSet);
                            return CommandResult.PlayerWin;
                        }
                    }
                }

                //Below for logic non weird winning set
                var playerFlowers = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId && t.Type == TileType.Flower);
                if (playerFlowers != null)
                {
                    foreach (var t in playerFlowers)
                    {
                        tilesToTestForwin.Add(t);
                    }
                }

                var winningSet = BuildWinningTiles(tilesToTestForwin, player.TileSets);
                if (winningSet != null)
                {
                    CalculateWinning(game, player, winningSet);
                    int tempPts = GetTotalPointForHand(game, winningSet.Hands);
                    if (tempPts >= 3)
                    {
                        RecordWinning(game, player, winningSet);
                        return CommandResult.PlayerWin;
                    }
                    else
                    {
                        return CommandResult.WinNotEnoughPoint;
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
        
        private void RecordWinning(Game game, Player player, WinningTileSet wts)
        {
            game.Count++;
            Record rec = new Record();
            rec.WinningTileSet = wts;
            rec.Winner = player;
            rec.GameNo = game.Count;

            int tempPts = GetTotalPointForHand(game, wts.Hands);
            //TODO check for max point for game logic
            tempPts = tempPts > 10 ? 10 : tempPts;

            int playerPoints = tempPts;

            var isSelfPick = wts.Hands.Where(h => h.WinningType == WinningType.SelfDraw).FirstOrDefault();
            if (isSelfPick != null)
            {
                playerPoints = tempPts * 3;
                player.CurrentPoint += playerPoints;
                DistributePointForSelfDraw(game, player, tempPts);
            }
            else
            {
                player.CurrentPoint += playerPoints;
                if (game.LastTile != null)
                {
                    var pp = GetPlayerByConnectionId(game, game.LastTile.Owner);
                    rec.Feeder = pp;
                    pp.CurrentPoint -= playerPoints;
                }
            }
            game.Records.Add(rec);
        }

        private int GetTotalPointForHand(Game game, List<HandWorth> hand)
        {
            int ret = 0;            
            foreach (var h in hand)
            {                
                ret += game.PointSystem[h.WinningType];
            }
            return ret;
        }

        private void AddWinningHand(Game game, WinningTileSet ws, WinningType wt)
        {
            //add the handworth for 13 wonders
            HandWorth hw = new HandWorth();
            hw.WinningType = wt;
            hw.HandName = wt.ToString();
            hw.Point = game.PointSystem[wt];

            ws.Hands.Add(hw);
            ws.WinningTypes.Add(wt.ToString(), game.PointSystem[wt]);
            ws.TotalPoints += game.PointSystem[wt];
        }

        private void CalculateWinning(Game game, Player player, WinningTileSet ws) 
        {
            var winningTypes = PointCalculator.GetWinningType(game, ws, player);
            int tempPts = 0;

            if (winningTypes != null && winningTypes.Count() > 0)
            {
                foreach (var item in winningTypes)
                {
                    HandWorth temp = new HandWorth();
                    temp.WinningType = item;
                    temp.HandName = item.ToString();
                    temp.Point = game.PointSystem[item];

                    ws.Hands.Add(temp);
                    ws.WinningTypes.Add(item.ToString(), game.PointSystem[item]);
                    tempPts += game.PointSystem[item];
                }
                ws.TotalPoints += tempPts;
            }
        }
        
        private WinningTileSet BuildWinningTilesForWeirdSet(Game game, Player player, List<Tile> tiles)
        {
            WinningTileSet ret = new WinningTileSet();
 
            TileSet t1 = new TileSet();
            t1.isRevealed = false;
            t1.TileSetType = TileSetType.Weird;
            t1.TileType = TileType.Mix;
            
            List<Tile> temp1 = new List<Tile>();
            for (int i = 0; i < 3; i++)
            {
                temp1.Add(tiles[i]);
            }
            t1.Tiles = temp1;

            ret.Sets[0] = t1;

            TileSet t2 = new TileSet();
            t2.isRevealed = false;
            t2.TileSetType = TileSetType.Weird;
            t2.TileType = TileType.Mix;
            
            List<Tile> temp2 = new List<Tile>();
            for (int i = 3; i < 6; i++)
            {
                temp2.Add(tiles[i]);
            }
            t2.Tiles = temp2;

            ret.Sets[1] = t2;

            TileSet t3 = new TileSet();
            t3.isRevealed = false;
            t3.TileSetType = TileSetType.Weird;
            t3.TileType = TileType.Mix;

            List<Tile> temp3 = new List<Tile>();
            for (int i = 6; i < 9; i++)
            {
                temp3.Add(tiles[i]);
            }
            t3.Tiles = temp3;

            ret.Sets[2] = t3;

            TileSet t4 = new TileSet();
            t4.isRevealed = false;
            t4.TileSetType = TileSetType.Weird;
            t4.TileType = TileType.Mix;

            List<Tile> temp4 = new List<Tile>();
            for (int i = 9; i < 12; i++)
            {
                temp4.Add(tiles[i]);
            }
            t4.Tiles = temp4;

            ret.Sets[3] = t4;

            TileSet tEye = new TileSet();
            tEye.isRevealed = false;
            tEye.TileSetType = TileSetType.Weird;
            tEye.TileType = TileType.Mix;
            List<Tile> tempEye = new List<Tile>();
            for (int i = 12; i < 14; i++)
            {
                tempEye.Add(tiles[i]);
            }
            tEye.Tiles = tempEye;
            ret.Eye = tEye;

            var playerFlowerTiles = game.Board.Tiles.Where(t => t.Owner == player.ConnectionId && t.Type == TileType.Flower);

            if (playerFlowerTiles != null && playerFlowerTiles.Count() > 0)
            {
                foreach (var t in playerFlowerTiles)
                {
                    ret.Flowers.Add(t);
                }            
            }
            return ret;
        }
        
        private bool CheckForAllPair(IEnumerable<Tile> tilesToTestForwin)
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
                        break;
                    }
                }
            }
            return isAllPair;
        }

        private List<Tile> CheckFor13Wonder(IEnumerable<Tile> tilesToTestForwin)
        {
            var thirteenWonderTiles = TileBuilder.BuildThirteenWonder();
            List<Tile> tempTilesToCheck13Wonders = new List<Tile>();
            bool foundEyesFor13Wonders = false;
            bool is13Wonders = true;
            foreach (var t in thirteenWonderTiles)
            {
                var dTile = tilesToTestForwin.Where(tt => (tt.Type == t.Type) && (tt.Value == t.Value));

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
            if (is13Wonders)
            {
                return tempTilesToCheck13Wonders;
            }
            else
            {
                return null;
            }
        }

        private void DistributePointForSelfDraw(Game game, Player player, int pts)
        {
            if (game.Player1.ConnectionId == player.ConnectionId)
            {
                game.Player2.CurrentPoint -= pts;
                game.Player3.CurrentPoint -= pts;
                game.Player4.CurrentPoint -= pts;
            }
            else if (game.Player2.ConnectionId == player.ConnectionId)
            {
                game.Player1.CurrentPoint -= pts;
                game.Player3.CurrentPoint -= pts;
                game.Player4.CurrentPoint -= pts;
            }
            else if (game.Player3.ConnectionId == player.ConnectionId)
            {
                game.Player1.CurrentPoint -= pts;
                game.Player2.CurrentPoint -= pts;
                game.Player4.CurrentPoint -= pts;
            }
            else if (game.Player4.ConnectionId == player.ConnectionId)
            {
                game.Player1.CurrentPoint -= pts;
                game.Player2.CurrentPoint -= pts;
                game.Player3.CurrentPoint -= pts;
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
            if (game.DiceMovedCount > 4)
            {
                game.CurrentWind = WindDirection.South;
            }
            else if (game.DiceMovedCount > 8)
            {
                game.CurrentWind = WindDirection.West;
            }
            else if (game.DiceMovedCount > 12)
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

        public void SetNextGamePlayerToStart(Game game)
        {
            if (game.Player1.ConnectionId == game.DiceRoller)
            {
                game.DiceRoller = game.Player2.ConnectionId;
                game.PlayerTurn = game.Player2.ConnectionId;
            }
            else if (game.Player2.ConnectionId == game.DiceRoller)
            {
                game.DiceRoller = game.Player3.ConnectionId;
                game.PlayerTurn = game.Player3.ConnectionId;
            }
            else if (game.Player3.ConnectionId == game.DiceRoller)
            {
                game.DiceRoller = game.Player4.ConnectionId;
                game.PlayerTurn = game.Player4.ConnectionId;
            }
            else if (game.Player4.ConnectionId == game.DiceRoller)
            {
                game.DiceRoller = game.Player1.ConnectionId;
                game.PlayerTurn = game.Player1.ConnectionId;
            }
        }

        public void SetNextPlayerTurn(Game game)
        {
            if (game != null)
            {
                if (game.PlayerTurn == game.Player1.ConnectionId)
                {
                    game.PlayerTurn = game.Player2.ConnectionId;
                    game.Player2.CanPickTile = true;
                }
                else if (game.PlayerTurn == game.Player2.ConnectionId)
                {
                    game.PlayerTurn = game.Player3.ConnectionId;
                    game.Player3.CanPickTile = true;
                }
                else if (game.PlayerTurn == game.Player3.ConnectionId)
                {
                    game.PlayerTurn = game.Player4.ConnectionId;
                    game.Player4.CanPickTile = true;
                }
                else if (game.PlayerTurn == game.Player4.ConnectionId)
                {
                    game.PlayerTurn = game.Player1.ConnectionId;
                    game.Player1.CanPickTile = true;
                }
            }
        }

        public WinningTileSet BuildWinningTiles(IEnumerable<Tile> playerTiles, List<TileSet> tilesets) 
        {
            WinningTileSet ret = new WinningTileSet();
            bool tileSetIsLegit = false;
            var flowerTiles = playerTiles.Where(t => t.Type == TileType.Flower);
            if (flowerTiles != null && flowerTiles.Count() > 0)
            {
                foreach (var t in flowerTiles)
                {
                    ret.Flowers.Add(t);
                }
            }
            var tiles = playerTiles.Where(t => t.Type != TileType.Flower);

            //set the tileset that was open
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
                var tilesWithoutEyes = tiles.OrderBy(t => t.Value).ToList();
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
                    TileSet theEye = new TileSet() { isRevealed = false, Tiles = eyes, TileSetType = TileSetType.Eye, TileType = eyes.First().Type };

                    ret.Eye = theEye;
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

        public void PopulatePoint(Game game)
        {
            game.PointSystem.Add(WinningType.NoFlower, 1);
            game.PointSystem.Add(WinningType.Straight, 1);
            game.PointSystem.Add(WinningType.Pong, 3);
            game.PointSystem.Add(WinningType.MixPureHand, 3);
            game.PointSystem.Add(WinningType.PureHand, 7);
            game.PointSystem.Add(WinningType.PureHonorHand, 10);
            game.PointSystem.Add(WinningType.SevenPairs, 7);
            game.PointSystem.Add(WinningType.ThirteenOrphans, 10);
            game.PointSystem.Add(WinningType.LittleDragon, 10);
            game.PointSystem.Add(WinningType.BigDragon, 10);
            game.PointSystem.Add(WinningType.LittleFourWind, 10);
            game.PointSystem.Add(WinningType.BigFourWind, 10);
            game.PointSystem.Add(WinningType.AllKong, 10);
            game.PointSystem.Add(WinningType.AllHiddenPongAndSelfPick, 10);
            game.PointSystem.Add(WinningType.AllTerminal, 10);
            game.PointSystem.Add(WinningType.SelfDraw, 1);
            game.PointSystem.Add(WinningType.ConcealedHand, 1);
            game.PointSystem.Add(WinningType.WinOnLastTile, 1);
            game.PointSystem.Add(WinningType.OneGoodFlower, 1);
            game.PointSystem.Add(WinningType.TwoGoodFlower, 2);
            game.PointSystem.Add(WinningType.AllFourFlowerSameType, 1);
            game.PointSystem.Add(WinningType.OneGoodWind, 1);
            game.PointSystem.Add(WinningType.TwoGoodWind, 2);
            game.PointSystem.Add(WinningType.HeavenlyHand, 10);
            game.PointSystem.Add(WinningType.EarthlyHand, 10);
            game.PointSystem.Add(WinningType.RedDragon, 1);
            game.PointSystem.Add(WinningType.GreenDragon, 1);
            game.PointSystem.Add(WinningType.WhiteDragon, 1);
        }

        private TileSet GetOneWinningTileSet(IEnumerable<Tile> tiles)
        {
            TileSet ret = new TileSet();

            //try get pong tile
            foreach (var t in tiles)
            {
                var temp = FindPongTiles(t, tiles);
                if (temp != null && temp.Count() == 3)
                {
                    ret.Tiles = temp;
                    ret.TileSetType = TileSetType.Pong;
                    ret.isRevealed = false;
                    ret.TileType = temp.First().Type;
                    return ret;
                }
            }

            //if no pong tile, try get straight/chow tile
            foreach (var t in tiles)
            {
                var temp = FindStraightTiles(t, tiles);
                if (temp != null && temp.Count() == 3)
                {
                    ret.Tiles = temp;
                    ret.TileSetType = TileSetType.Chow;
                    ret.isRevealed = false;
                    ret.TileType = temp.First().Type;
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
                TileSetType = type,
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
    }
}