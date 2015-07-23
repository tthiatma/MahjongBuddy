using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;
using MahjongBuddy.Models;

namespace MahjongBuddy
{
    [Export]
    public class GameLogic
    {
        public Game Game { get; set; }

        public bool DoPong(Game game, string userConnectionId)
        {
            var activePlayerTiles = game.Board.Tiles.Where(t => t.Owner == userConnectionId && t.Status == TileStatus.UserActive);
            var thrownTile = game.LastTile;
            if (thrownTile != null && thrownTile.Owner != userConnectionId)
            {
                var matchedTileTypeAndValue = activePlayerTiles.Where(t => t.Type == thrownTile.Type && t.Value == thrownTile.Value);
                if (matchedTileTypeAndValue.Count() >= 2)
                {
                    var playerTilesPong = matchedTileTypeAndValue.Take(2).ToList();
                    playerTilesPong.Add(thrownTile);
                    CommandTileToPlayerGraveyard(game, playerTilesPong, userConnectionId);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void CommandTileToPlayerGraveyard(Game game, IEnumerable<Tile> tiles, string playerConnectionId, bool replaceTile = false)
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


    }
}