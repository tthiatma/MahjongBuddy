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
        public int Counter { get; set; }
    }

    public enum TileStatus
    {
        JustPicked = 1,
        UserActive = 2,
        UserGraveyard = 3,
        BoardGraveyard = 4
    }

    public enum TileType
    { 
        Round,
        Money,
        Stick,
        Wind,
        Dragon,
        Flower
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
        FlowerRoman = 17,
        FlowerNumeric = 18,
    }
}