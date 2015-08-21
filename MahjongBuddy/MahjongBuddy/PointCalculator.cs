using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace MahjongBuddy.Models
{
    public class PointCalculator
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PointCalculator));

        private List<WinningType> _winningTypes = new List<WinningType>();

        public List<WinningType> GetWinningType(Game game, WinningTileSet wts, Player player)
        {
            Action<Game, WinningTileSet, Player> Calculate;
            Calculate = FindConcealedHand;
            Calculate += FindWind;
            Calculate += FindMixPureHand;
            Calculate += FindPureHand;
            Calculate += FindAllPong;
            Calculate += FindFlower;
            Calculate += FindStraight;
            Calculate += FindSelfDraw;

            Calculate(game, wts, player);

            return _winningTypes;
        }

        //instant win when game started
        public void FindHeavenlyHand(Game game, WinningTileSet wts, Player player)
        {
            if (game.TileCounter == 0)
            {
                _winningTypes.Add(WinningType.HeavenlyHand);
            }            
        }

        //instant win withing 4 turn
        public void FindEarthlyHand(Game game, WinningTileSet wts, Player player)
        {
            if (game.TileCounter > 0 && game.TileCounter < 5)
            {
                _winningTypes.Add(WinningType.EarthlyHand);
            }
        }

        public void FindSelfDraw(Game game, WinningTileSet wts, Player player)
        {
            bool isSelfPick = false;
            if (wts != null)
            {
                for (int i = 0; i < wts.Sets.Length; i++)
                {
                    var set = wts.Sets[i];
                    foreach (var t in set.Tiles)
                    {
                        if (t.Status == TileStatus.JustPicked)
                        {
                            isSelfPick = true;
                        }
                    }
                }
                foreach (var t in wts.Eye.Tiles)
                {
                    if (t.Status == TileStatus.JustPicked)
                    {
                        isSelfPick = true;
                    }
                }
                if (isSelfPick)
                {
                    _winningTypes.Add(WinningType.SelfDraw);
                }
            }
        }

        public void FindConcealedHand(Game game, WinningTileSet wts, Player player)
        {
            bool foundRevealed = false;
            if (wts != null)
            {
                for (int i = 0; i < wts.Sets.Length; i++)
                {
                    var set = wts.Sets[i];
                    if (set.isRevealed)
                    {
                        foundRevealed = true;
                    }
                }
            }
            else
            {
                logger.Error("winning tile set is null when trying to get concealed hand");
            }
           
            if (!foundRevealed)
            {
                _winningTypes.Add(WinningType.ConcealedHand);
            }
        }

        public void FindWind(Game game, WinningTileSet wts, Player player)
        {
            int pts = 0;

            TileValue windVal = TileValue.WindEast;
            if (player.Wind == WindDirection.East) { windVal = TileValue.WindEast; }
            if (player.Wind == WindDirection.South) { windVal = TileValue.WindSouth; }
            if (player.Wind == WindDirection.West) { windVal = TileValue.WindWest; }
            if (player.Wind == WindDirection.North) { windVal = TileValue.WindNorth; }

            List<Tile> tiles = new List<Tile>();

            for (int i = 0; i < wts.Sets.Length; i++)
            {
                var set = wts.Sets[i];
                if (set.TileSetType == TileSetType.Pong)
                {
                    foreach (var t in set.Tiles)
                    {
                        tiles.Add(t);
                    }
                }
            }

            var matchedUserWindTile = tiles.Where(t => t.Value == windVal);

            if (matchedUserWindTile != null && matchedUserWindTile.Count() >= 3)
            {
                pts += 1;
            }

            if (game.CurrentWind == WindDirection.East) { windVal = TileValue.WindEast; }
            if (game.CurrentWind == WindDirection.South) { windVal = TileValue.WindSouth; }
            if (game.CurrentWind == WindDirection.West) { windVal = TileValue.WindWest; }
            if (game.CurrentWind == WindDirection.North) { windVal = TileValue.WindNorth; }

            var gameWindTile = tiles.Where(t => t.Value == windVal);

            if (gameWindTile != null && gameWindTile.Count() >= 3)
            {
                pts += 1;
            }

            if (pts == 1)
            {
                _winningTypes.Add(WinningType.OneGoodWind);          
            }
            else if (pts == 2)
            {
                _winningTypes.Add(WinningType.TwoGoodWind);
            }
        }

        public void FindMixPureHand(Game game, WinningTileSet wts, Player player)
        {
            List<Tile> tiles = new List<Tile>();
            for (int i = 0; i < wts.Sets.Length; i++)
            {
                var set = wts.Sets[i];
                if (set.TileType != TileType.Dragon && set.TileType != TileType.Wind)
                {
                    foreach (var t in set.Tiles)
                    {
                        tiles.Add(t);
                    }
                }
            }
            if (tiles.Count() > 0)
            {
                var dTile = tiles.First().Type;
                var wrongType = tiles.Where(t => t.Type != dTile);
                if (wrongType == null)
                {
                    _winningTypes.Add(WinningType.MixPureHand);
                }
            }
        }

        public void FindPureHand(Game game, WinningTileSet wts, Player player)
        {
            List<Tile> tiles = new List<Tile>();
            for (int i = 0; i < wts.Sets.Length; i++)
            {
                var set = wts.Sets[i];
                if (set.TileSetType == TileSetType.Pong)
                {
                    foreach (var t in set.Tiles)
                    {
                        tiles.Add(t);
                    }
                }
            }
            
            var firstTileType = tiles.Where(t => t.Type != TileType.Dragon && t.Type != TileType.Wind && t.Type != TileType.Flower).FirstOrDefault();

            if (firstTileType != null)
            {
                bool foundDifferentType = false;
                foreach (var t in tiles)
                {
                    if (t.Type != firstTileType.Type) { foundDifferentType = true; }
                }

                if (!foundDifferentType)
                {
                    _winningTypes.Add(WinningType.PureHand);
                }
            }
        }

        public void FindAllPong(Game game, WinningTileSet wts, Player player)
        {
            bool allPong = true;
            bool allConcealed = true;
            for (int i = 0; i < wts.Sets.Length; i++)
            {
                var set = wts.Sets[i];
                if (set.isRevealed)
                {
                    allConcealed = false;
                }

                if (set.TileSetType != TileSetType.Pong)
                {
                    allPong = false;
                    break;
                }                
            }

            if (allPong && allConcealed)
            {
                _winningTypes.Add(WinningType.AllHiddenPongAndSelfPick);
            }
            else if (allPong)
            {
                _winningTypes.Add(WinningType.Pong);            
            }
        }

        public void FindFlower(Game game, WinningTileSet wts, Player player)
        {
            int pts = 0;
            var tiles = wts.Flowers;

            if (tiles.Count == 0)
            {
                _winningTypes.Add(WinningType.NoFlower);  
                return;
            }

            IEnumerable<Tile> matchedUserFlowerTile = null;

            if (player.Wind == WindDirection.East)
            {
                matchedUserFlowerTile = tiles.Where(t => t.Type == TileType.Flower && (t.Value == TileValue.FlowerNumericOne || t.Value == TileValue.FlowerRomanOne));
            }
            if (player.Wind == WindDirection.South)
            {
                matchedUserFlowerTile = tiles.Where(t => t.Type == TileType.Flower && (t.Value == TileValue.FlowerNumericTwo || t.Value == TileValue.FlowerRomanTwo));
            }
            if (player.Wind == WindDirection.West)
            {
                matchedUserFlowerTile = tiles.Where(t => t.Type == TileType.Flower && (t.Value == TileValue.FlowerNumericThree || t.Value == TileValue.FlowerRomanThree));
            }
            if (player.Wind == WindDirection.North)
            {
                matchedUserFlowerTile = tiles.Where(t => t.Type == TileType.Flower && (t.Value == TileValue.FlowerNumericFour || t.Value == TileValue.FlowerRomanFour));
            }

            if (matchedUserFlowerTile != null)
            {
                foreach (var t in matchedUserFlowerTile)
                {
                    pts += 1;
                }
            }

            var FlowerTiles = tiles.Where(t => t.Type == TileType.Flower);
            if (FlowerTiles != null && FlowerTiles.Count() >= 4)
            {
                var numericFlower = FlowerTiles.Where(t => t.Value == TileValue.FlowerNumericOne || t.Value == TileValue.FlowerNumericTwo || t.Value == TileValue.FlowerNumericThree || t.Value == TileValue.FlowerNumericFour);

                if (numericFlower != null && numericFlower.Count() == 4)
                {
                    pts += 1;
                }

                var romanFlower = FlowerTiles.Where(t => t.Value == TileValue.FlowerRomanOne || t.Value == TileValue.FlowerRomanTwo || t.Value == TileValue.FlowerRomanThree || t.Value == TileValue.FlowerRomanFour);

                if (romanFlower != null && romanFlower.Count() == 4)
                {
                    pts += 1;
                }
            }

            if (pts == 1)
            {
                _winningTypes.Add(WinningType.OneGoodFlower);
            }
            else if (pts == 2)
            {
                _winningTypes.Add(WinningType.TwoGoodFlower);            
            }
            else if (pts == 3)
            {
                _winningTypes.Add(WinningType.AllFourFlowerSameType);            
            }
        }
        
        public void FindStraight(Game game, WinningTileSet wts, Player player) 
        {
            //Chow == Straight
            bool allChow = true;
            for (int i = 0; i < wts.Sets.Length; i++)
            {
                var set = wts.Sets[i];
                if (set.TileSetType != TileSetType.Chow)
                {
                    allChow = false;
                    break;
                }
            }

            if (allChow)
            {
                _winningTypes.Add(WinningType.Straight);
            }
        }
        
        public void FindDragonCombo(Game game, WinningTileSet wts, Player player)
        {
            int dragonCount = 0;

            foreach (var s in wts.Sets)
            {
                if (s.TileSetType == TileSetType.Pong && s.Tiles.First().Value == TileValue.DragonRed)
                {
                    _winningTypes.Add(WinningType.RedDragon);
                    dragonCount++;
                }
                else if (s.TileSetType == TileSetType.Pong && s.Tiles.First().Value == TileValue.DragonGreen)
                {
                    _winningTypes.Add(WinningType.GreenDragon);
                    dragonCount++;
                }
                else if (s.TileSetType == TileSetType.Pong && s.Tiles.First().Value == TileValue.DragonWhite)
                {
                    _winningTypes.Add(WinningType.WhiteDragon);
                    dragonCount++;
                }
            }
            if (dragonCount == 3)
            {
                _winningTypes.Add(WinningType.BigDragon);
            }
            else if (dragonCount == 2)
            { 
                //check if the eye is dragon
                if (wts.Eye.TileType == TileType.Dragon)
                {
                    _winningTypes.Add(WinningType.LittleDragon);                     
                }
            }
        }

        public void FindWindCombo(Game game, WinningTileSet wts, Player player)
        {
            int windCount = 0;

            foreach (var s in wts.Sets)
            {
                if (s.TileSetType == TileSetType.Pong && s.Tiles.First().Value == TileValue.WindEast)
                {
                    windCount++;
                }
                else if (s.TileSetType == TileSetType.Pong && s.Tiles.First().Value == TileValue.WindNorth)
                {
                    windCount++;
                }
                else if (s.TileSetType == TileSetType.Pong && s.Tiles.First().Value == TileValue.WindSouth)
                {
                    windCount++;
                }
                else if (s.TileSetType == TileSetType.Pong && s.Tiles.First().Value == TileValue.WindWest)
                {
                    windCount++;
                }
            }
            if (windCount == 4)
            {
                _winningTypes.Add(WinningType.BigFourWind);
            }
            else if (windCount == 3)
            {
                //check if the eye is dragon
                if (wts.Eye.TileType == TileType.Wind)
                {
                    _winningTypes.Add(WinningType.LittleFourWind);
                }
            }
        }

        public void FindAllTerminal(Game game, WinningTileSet wts, Player player)
        {
            int terminalCount = 0;

            foreach (var s in wts.Sets)
            {
                if (s.TileSetType == TileSetType.Pong && s.Tiles.First().Value == TileValue.One)
                {
                    terminalCount++;
                }
                else if (s.TileSetType == TileSetType.Pong && s.Tiles.First().Value == TileValue.Nine)
                {
                    terminalCount++;
                }
                else if (s.TileSetType == TileSetType.Pong && s.Tiles.First().Type == TileType.Dragon)
                {
                    terminalCount++;
                }
                else if (s.TileSetType == TileSetType.Pong && s.Tiles.First().Type == TileType.Wind)
                {
                    terminalCount++;
                }
            }
            if (terminalCount == 4)
            {
                _winningTypes.Add(WinningType.AllTerminal);
            }
        }

        public void FindLastPick(Game game, WinningTileSet wts, Player player)
        {
            if (game.TilesLeft == 0)
            {
                _winningTypes.Add(WinningType.WinOnLastTile);
            }
        }

        public void FindAllKong(Game game, WinningTileSet wts, Player player)
        {
            bool allKong = true;
            for (int i = 0; i < wts.Sets.Length; i++)
            {
                var set = wts.Sets[i];
                if (set.TileSetType != TileSetType.Kong)
                {
                    allKong = false;
                    break;
                }
            }

            if (allKong)
            {
                _winningTypes.Add(WinningType.AllKong);
            }
        }

        public int CalculateFlower(Game game, IEnumerable<Tile> tiles, Player player)
        {
            int pts = 0;

            IEnumerable<Tile> matchedUserFlowerTile = null;

            if (player.Wind == WindDirection.East)
            {
                matchedUserFlowerTile = tiles.Where(t => t.Type == TileType.Flower && (t.Value == TileValue.FlowerNumericOne || t.Value == TileValue.FlowerRomanOne));
            }
            if (player.Wind == WindDirection.South)
            {
                matchedUserFlowerTile = tiles.Where(t => t.Type == TileType.Flower && (t.Value == TileValue.FlowerNumericTwo || t.Value == TileValue.FlowerRomanTwo));
            }
            if (player.Wind == WindDirection.West)
            {
                matchedUserFlowerTile = tiles.Where(t => t.Type == TileType.Flower && (t.Value == TileValue.FlowerNumericThree || t.Value == TileValue.FlowerRomanThree));
            }
            if (player.Wind == WindDirection.North)
            {
                matchedUserFlowerTile = tiles.Where(t => t.Type == TileType.Flower && (t.Value == TileValue.FlowerNumericFour || t.Value == TileValue.FlowerRomanFour));
            }

            if (matchedUserFlowerTile != null)
            {
                foreach (var t in matchedUserFlowerTile)
                {
                    pts += 1;
                }
            }

            var FlowerTiles = tiles.Where(t => t.Type == TileType.Flower);
            if (FlowerTiles != null && FlowerTiles.Count() >= 4)
            {
                var numericFlower = FlowerTiles.Where(t => t.Value == TileValue.FlowerNumericOne || t.Value == TileValue.FlowerNumericTwo || t.Value == TileValue.FlowerNumericThree || t.Value == TileValue.FlowerNumericFour);

                if (numericFlower != null && numericFlower.Count() == 4)
                {
                    pts += 1;
                }

                var romanFlower = FlowerTiles.Where(t => t.Value == TileValue.FlowerRomanOne || t.Value == TileValue.FlowerRomanTwo || t.Value == TileValue.FlowerRomanThree || t.Value == TileValue.FlowerRomanFour);

                if (romanFlower != null && romanFlower.Count() == 4)
                {
                    pts += 1;
                }
            }
            return pts;
        }
    }
}