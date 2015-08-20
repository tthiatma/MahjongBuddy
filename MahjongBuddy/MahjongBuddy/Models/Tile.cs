using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class Tile
    {
        public int Id { get; set; }
        public TileType Type { get; set; }
        public TileValue Value { get; set; }
        public TileStatus Status { get; set; }         
        public string Image { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int OpenTileCounter { get; set; }
    }

    public enum TileStatus
    {
        //Tile belongs to the board
        Unrevealed = 0,
        //Tile just picked from the board
        JustPicked = 1,
        //Tile that is on player's hand
        UserActive = 2,
        //Tile is kept by player
        UserGraveyard = 3,
        //Tile is open and thrown to the board
        BoardGraveyard = 4
    }

    public enum TileType
    { 
        Round,
        Money,
        Stick,
        Wind,
        Dragon,
        Flower,
        Mix
    }

    public enum TileValue
    { 
        One = 1,
        Two = 2,
        Three =3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        DragonRed = 10,
        DragonGreen = 11,
        DragonWhite = 12,
        WindNorth = 13,
        WindSouth = 14,
        WindEast = 15,
        WindWest = 16,
        FlowerRomanOne = 17,
        FlowerRomanTwo = 18,
        FlowerRomanThree = 19,
        FlowerRomanFour = 20,
        FlowerNumericOne = 21,
        FlowerNumericTwo = 22,
        FlowerNumericThree = 23,
        FlowerNumericFour = 24,
    }
}