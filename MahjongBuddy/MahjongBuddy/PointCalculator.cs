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

            Calculate(game, wts, player);

            return _winningTypes;
        }

        public void FindSelfPick(Game game, WinningTileSet wts, Player player)
        { 
        
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
            for (int i = 0; i < wts.Sets.Length; i++)
            {
                var set = wts.Sets[i];
                if (set.TileSetType != TileSetType.Pong)
                {
                    allPong = false;
                    break;
                }
                
            }

            if (allPong)
            {
                _winningTypes.Add(WinningType.Pong);
            }
        }

        public void FindFlower(Game game, WinningTileSet wts, Player player)
        {
            int pts = 0;
            var tiles = wts.Flowers;
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