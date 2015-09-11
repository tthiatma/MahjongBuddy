using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    /// <summary>
    /// Dealer - imagine having a dealer in poker table
    /// * Tell the client how many tiles left
    /// * Tell what's the current wind
    /// * Tell what's the last tile that was thrown
    /// * Tell when no one can make a move
    /// </summary>
    public class Dealer
    {
        public WindDirection CurrentWind { get; set; }
        public int TilesLeft { get; set; }
        public Tile LastTile { get; set; }
        public bool HaltMove { get; set; }
    }
}