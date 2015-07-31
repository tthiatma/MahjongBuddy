using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class PointCalculator
    {
        private int _totalPoints;

        public int GetTotalPoints(Game game, IEnumerable<Tile> tiles, Player player) 
        {
            _totalPoints = 0;
            Func<Game, IEnumerable<Tile>, Player, int> Calculate;            
            Calculate = CalculateAllTilesNeverRevealed;
            Calculate += CalculateWind;
            Calculate += CalculateTileSameTypeWithWildCard;
            Calculate += CalculateTileAllSameType;
            Calculate += CalculateKong;
            Calculate += CalculateFlower;
            Calculate += CalculateStraight;
            Calculate += CalculatePong;

            Calculate(game, tiles, player);

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

        public int CalculateAllTilesNeverRevealed(Game game, IEnumerable<Tile> tiles, Player player)
        {
            int pts = 0;

            bool foundRevealed = false;

            foreach (var t in tiles)
            {
                if (t.Status == TileStatus.UserGraveyard && t.Type != TileType.Flower)
                {
                    foundRevealed = true;
                }
            }

            if (!foundRevealed)
            {
                pts += 1;
            }

            _totalPoints += pts;

            return pts;
        }

        public int CalculateWind(Game game, IEnumerable<Tile> tiles, Player player)
        {
            int pts = 0;

            TileValue windVal = TileValue.WindEast;
            if(player.Wind == WindDirection.East){windVal = TileValue.WindEast;}
            if(player.Wind == WindDirection.South){windVal = TileValue.WindSouth;}
            if(player.Wind == WindDirection.West){windVal = TileValue.WindWest;}
            if(player.Wind == WindDirection.North){windVal = TileValue.WindNorth;}

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
            _totalPoints += pts;

            return pts;
        }

        public int CalculateTileSameTypeWithWildCard(Game game, IEnumerable<Tile> tiles, Player player)
        {
            int pts = 0;

            
           
            return pts;
        }

        public int CalculateTileAllSameType(Game game, IEnumerable<Tile> tiles, Player player)
        {
            int pts = 0;

            var firstTileType = tiles.Where(t => t.Type != TileType.Dragon && t.Type != TileType.Wind && t.Type != TileType.Flower).FirstOrDefault();

            if (firstTileType != null)
            {
                bool foundDifferentType = false;
                foreach (var t in tiles)
                {
                    if (t.Type != firstTileType.Type) { foundDifferentType = true; }
                }

                if (!foundDifferentType) { pts += 7; }            
            }
            
            _totalPoints += pts;

            return pts;
        }

        public int CalculateKong(Game game, IEnumerable<Tile> tiles, Player player)
        {
            int pts = 0;

            _totalPoints += 1;

            return pts;
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
            _totalPoints += pts;
            return pts;
        }

        public int CalculateStraight(Game game, IEnumerable<Tile> tiles, Player player) 
        {
            int pts = 0;



            _totalPoints += pts;

            return pts;
        }

        public int CalculatePong(Game game, IEnumerable<Tile> tiles, Player player)
        {
            int pts = 0;

            _totalPoints += 1;

            return pts;
        }
    }
}