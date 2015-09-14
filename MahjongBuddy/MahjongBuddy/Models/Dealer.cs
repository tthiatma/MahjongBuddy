using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    /// <summary>
    /// Dealer - imagine having a dealer in poker table
    /// Dealer class grabs properties from Game class that needs to be shown to player
    /// * Tell the client how many tiles left
    /// * Tell what's the current wind
    /// * Tell what's the last tile that was thrown
    /// * Tell when no one can make a move
    /// </summary>
    public class Dealer
    {
        public WindDirection CurrentWind { get { return Game.CurrentWind; } }
        public int TilesLeft { get { return Game.TilesLeft; } }
        public Tile LastTile { get { return Game.LastTile; } }
        public bool HaltMove { get { return Game.HaltMove; }  }
        public string PlayerTurn { get { return Game.PlayerTurn; } }

        [JsonIgnore]
        public Game Game { get; set; }

        public Dealer(Game game)
        {
            Game = game;
        }
    }
    
}