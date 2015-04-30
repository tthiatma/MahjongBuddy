using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public static class PointCalculator
    {
        static int GetTotalPoints(IEnumerable<Tile> tiles) 
        {
            int ret = 0;

            if (IsStraight(tiles))
            {
                ret++;
            }

            if (IsPongPong(tiles))
            {
                ret++;
            }

            return ret;

        }

        static bool IsStraight(IEnumerable<Tile> tiles) 
        {         
            return false;
        }

        static bool IsPongPong(IEnumerable<Tile> tiles)
        {
            return false;
        }

        static private IEnumerable<IEnumerable<Tile>> FindAllThreeStraight(IEnumerable<Tile> tiles)
        {
            var straightMoneyTiles = FindStraightByType(TileType.Money, tiles);
            var straightRoundTiles = FindStraightByType(TileType.Round, tiles);
            var straightStickTiles = FindStraightByType(TileType.Stick, tiles);

            return straightMoneyTiles.Concat(straightRoundTiles).Concat(straightStickTiles);
        }

        static private IEnumerable<IEnumerable<Tile>> FindStraightByType(TileType type, IEnumerable<Tile> tile)
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