using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class Board
    {

        private List<Tile> _tiles = new List<Tile>();

        public List<Tile> Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
        }
        public void CreateTiles(){
            for (var i = 1; i < 5; i++)
            {
                _tiles.Add(new Tile() { Type = TileType.Money, Value = TileValue.One, Owner = "board", Name = i + "MoneyOne" });
                _tiles.Add(new Tile() { Type = TileType.Money, Value = TileValue.Two, Owner = "board", Name = i + "MoneyTwo" });
                _tiles.Add(new Tile() { Type = TileType.Money, Value = TileValue.Three, Owner = "board", Name = i + "MoneyThree" });
                _tiles.Add(new Tile() { Type = TileType.Money, Value = TileValue.Four, Owner = "board", Name = i + "MoneyFour" });
                _tiles.Add(new Tile() { Type = TileType.Money, Value = TileValue.Five, Owner = "board", Name = i + "MoneyFive" });
                _tiles.Add(new Tile() { Type = TileType.Money, Value = TileValue.Six, Owner = "board", Name = i + "MoneySix" });
                _tiles.Add(new Tile() { Type = TileType.Money, Value = TileValue.Seven, Owner = "board", Name = i + "MoneySeven" });
                _tiles.Add(new Tile() { Type = TileType.Money, Value = TileValue.Eight, Owner = "board", Name = i + "MoneyEight" });
                _tiles.Add(new Tile() { Type = TileType.Money, Value = TileValue.Nine, Owner = "board", Name = i + "MoneyNine" });

                _tiles.Add(new Tile() { Type = TileType.Round, Value = TileValue.One, Owner = "board", Name = i + "RoundOne" });
                _tiles.Add(new Tile() { Type = TileType.Round, Value = TileValue.Two, Owner = "board", Name = i + "RoundTwo" });
                _tiles.Add(new Tile() { Type = TileType.Round, Value = TileValue.Three, Owner = "board", Name = i + "RoundThree" });
                _tiles.Add(new Tile() { Type = TileType.Round, Value = TileValue.Four, Owner = "board", Name = i + "RoundFour" });
                _tiles.Add(new Tile() { Type = TileType.Round, Value = TileValue.Five, Owner = "board", Name = i + "RoundFive" });
                _tiles.Add(new Tile() { Type = TileType.Round, Value = TileValue.Six, Owner = "board", Name = i + "RoundSix" });
                _tiles.Add(new Tile() { Type = TileType.Round, Value = TileValue.Seven, Owner = "board", Name = i + "RoundSeven" });
                _tiles.Add(new Tile() { Type = TileType.Round, Value = TileValue.Eight, Owner = "board", Name = i + "RoundEight" });
                _tiles.Add(new Tile() { Type = TileType.Round, Value = TileValue.Nine, Owner = "board", Name = i + "RoundNine" });

                _tiles.Add(new Tile() { Type = TileType.Stick, Value = TileValue.One, Owner = "board", Name = i + "StickOne" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Value = TileValue.Two, Owner = "board", Name = i + "StickTwo" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Value = TileValue.Three, Owner = "board", Name = i + "StickThree" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Value = TileValue.Four, Owner = "board", Name = i + "StickFour" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Value = TileValue.Five, Owner = "board", Name = i + "StickFive" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Value = TileValue.Six, Owner = "board", Name = i + "StickSix" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Value = TileValue.Seven, Owner = "board", Name = i + "StickSeven" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Value = TileValue.Eight, Owner = "board", Name = i + "StickEight" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Value = TileValue.Nine, Owner = "board", Name = i + "StickNine" });

                _tiles.Add(new Tile() { Type = TileType.Dragon, Value = TileValue.DragonGreen, Owner = "board", Name = i + "DragonGreen" });
                _tiles.Add(new Tile() { Type = TileType.Dragon, Value = TileValue.DragonRed, Owner = "board", Name = i + "DragonRed" });
                _tiles.Add(new Tile() { Type = TileType.Dragon, Value = TileValue.DragonWhite, Owner = "board", Name = i + "DragonWhite" });

                _tiles.Add(new Tile() { Type = TileType.Wind, Value = TileValue.WindNorth, Owner = "board", Name = i + "WindNorth" });
                _tiles.Add(new Tile() { Type = TileType.Wind, Value = TileValue.WindEast, Owner = "board", Name = i + "WindEast" });
                _tiles.Add(new Tile() { Type = TileType.Wind, Value = TileValue.WindSouth, Owner = "board", Name = i + "WindSouth" });
                _tiles.Add(new Tile() { Type = TileType.Wind, Value = TileValue.WindWest, Owner = "board", Name = i + "WindWest" });

                _tiles.Add(new Tile() { Type = TileType.Flower, Value = TileValue.FlowerNumeric, Owner = "board", Name = i + "FlowerNumeric" });
                _tiles.Add(new Tile() { Type = TileType.Flower, Value = TileValue.FlowerRoman, Owner = "board", Name = i + "FlowerRoman" });
            }

            int _tileid = 1;
            foreach (var tile in _tiles)
            {
                tile.Id = _tileid;
                _tileid++;
            }
        }
    }
}