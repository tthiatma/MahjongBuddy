using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class PointCalculator
    {
        private int _totalPoints;

        public int GetTotalPoints(IEnumerable<Tile> tiles, Player player) 
        {
            _totalPoints = 0;
            Action<IEnumerable<Tile>, Player> Calculate;            
            Calculate = CalculateAllTilesNeverRevealed;
            Calculate += CalculateStraight;
            Calculate += CalculatePong;
            Calculate += CalculateFlower;

            Calculate(tiles, player);

            //TODO obey game setting for points limit
            if (_totalPoints >= 10)
            {
                return 10;
            }
            else
            {
                return _totalPoints;            
            }
        }

        private void CalculateWind(IEnumerable<Tile> tiles, Player player)
        {
            TileValue windVal = TileValue.WindEast;
            if(player.Wind == WindDirection.East){windVal = TileValue.WindEast;}
            if(player.Wind == WindDirection.South){windVal = TileValue.WindSouth;}
            if(player.Wind == WindDirection.West){windVal = TileValue.WindWest;}
            if(player.Wind == WindDirection.North){windVal = TileValue.WindNorth;}

            var MatchedUserWindTile = tiles.Where(t => t.Type == TileType.Wind && t.Value == windVal);

            if (MatchedUserWindTile != null && MatchedUserWindTile.Count() >= 3) 
            {
                _totalPoints += 1;
            }
        }

        private void CalculateTileSameTypeWithWildCard(IEnumerable<Tile> tiles, Player player)
        {
            var openedTiles = tiles.Where(t => t.Status == TileStatus.UserGraveyard && t.Type != TileType.Flower);

            if (openedTiles != null && openedTiles.Count() > 0)
            {
                //this means user revealed card
            }
            else 
            {
                _totalPoints += 1;
            }
            
        }

        private void CalculateTileAllSameType(IEnumerable<Tile> tiles, Player player)
        { 
        
        }

        private void CalculateKong(IEnumerable<Tile> tiles, Player player)
        {
            _totalPoints += 1;

        }
        
        private void CalculateFlower(IEnumerable<Tile> tiles, Player player)
        {
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
                    _totalPoints += 1;
                }
            }

            var FlowerTiles = tiles.Where(t => t.Type == TileType.Flower);
            if (FlowerTiles != null && FlowerTiles.Count() >= 4)
            {
                var numericFlower = FlowerTiles.Where(t => t.Value == TileValue.FlowerNumericOne || t.Value == TileValue.FlowerNumericTwo || t.Value == TileValue.FlowerNumericThree || t.Value == TileValue.FlowerNumericFour);

                if (numericFlower != null && numericFlower.Count() == 4) 
                {
                    _totalPoints += 1;
                }

                var romanFlower = FlowerTiles.Where(t => t.Value == TileValue.FlowerRomanOne || t.Value == TileValue.FlowerRomanTwo || t.Value == TileValue.FlowerRomanThree || t.Value == TileValue.FlowerRomanFour);

                if (romanFlower != null && romanFlower.Count() == 4)
                {
                    _totalPoints += 1;
                }
            }
           
        }

        private void CalculateAllTilesNeverRevealed(IEnumerable<Tile> tiles, Player player)
        {
            _totalPoints += 1;

        }

        private void CalculateStraight(IEnumerable<Tile> tiles, Player player) 
        {
            _totalPoints += 1;
        }

        private void CalculatePong(IEnumerable<Tile> tiles, Player player)
        {
            _totalPoints += 1;
        }

        private IEnumerable<IEnumerable<Tile>> FindAllThreeStraight(IEnumerable<Tile> tiles)
        {
            var straightMoneyTiles = FindStraightByType(TileType.Money, tiles);
            var straightRoundTiles = FindStraightByType(TileType.Round, tiles);
            var straightStickTiles = FindStraightByType(TileType.Stick, tiles);

            return straightMoneyTiles.Concat(straightRoundTiles).Concat(straightStickTiles);
        }

        private IEnumerable<IEnumerable<Tile>> FindStraightByType(TileType type, IEnumerable<Tile> tile)
        {
            var ret = new List<IEnumerable<Tile>>();

            var sameTypeTiles = tile.Where(t => t.Type == type);
            foreach (var t in sameTypeTiles)
            {
                if (sameTypeTiles.Any(ti => ti.Value == (t.Value - 2)) && sameTypeTiles.Any(ti => ti.Value == (t.Value - 1)))
                {
                    var temp = new List<Tile>();
                    temp.Add(t);
                    temp.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value - 2)).First());
                    temp.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value - 1)).First());
                    ret.Add(temp);
                }

                if (sameTypeTiles.Any(ti => ti.Value == (t.Value - 1)) && sameTypeTiles.Any(ti => ti.Value == (t.Value + 1)))
                {
                    var temp = new List<Tile>();
                    temp.Add(t);
                    temp.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value - 1)).First());
                    temp.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value + 1)).First());
                    ret.Add(temp);
                }

                if (sameTypeTiles.Any(ti => ti.Value == (t.Value + 1)) && sameTypeTiles.Any(ti => ti.Value == (t.Value + 2)))
                {
                    var temp = new List<Tile>();
                    temp.Add(t);
                    temp.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value + 1)).First());
                    temp.Add(sameTypeTiles.Where(ti => ti.Value == (t.Value + 2)).First());
                    ret.Add(temp);
                }
            }
            return ret;
        }

    }
}