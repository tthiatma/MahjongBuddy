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
                _tiles.Add(new Tile() { Type = TileType.Money, Status= TileStatus.Unrevealed, Value = TileValue.One, Owner = "board", Name = i + "MoneyOne", Image = "/Content/images/tiles/64px/man/man1.png" });
                _tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.Unrevealed, Value = TileValue.Two, Owner = "board", Name = i + "MoneyTwo", Image = "/Content/images/tiles/64px/man/man2.png" });
                _tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.Unrevealed, Value = TileValue.Three, Owner = "board", Name = i + "MoneyThree", Image = "/Content/images/tiles/64px/man/man3.png" });
                _tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.Unrevealed, Value = TileValue.Four, Owner = "board", Name = i + "MoneyFour", Image = "/Content/images/tiles/64px/man/man4.png" });
                _tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.Unrevealed, Value = TileValue.Five, Owner = "board", Name = i + "MoneyFive", Image = "/Content/images/tiles/64px/man/man5.png" });
                _tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.Unrevealed, Value = TileValue.Six, Owner = "board", Name = i + "MoneySix", Image = "/Content/images/tiles/64px/man/man6.png" });
                _tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.Unrevealed, Value = TileValue.Seven, Owner = "board", Name = i + "MoneySeven", Image = "/Content/images/tiles/64px/man/man7.png" });
                _tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.Unrevealed, Value = TileValue.Eight, Owner = "board", Name = i + "MoneyEight", Image = "/Content/images/tiles/64px/man/man8.png" });
                _tiles.Add(new Tile() { Type = TileType.Money, Status = TileStatus.Unrevealed, Value = TileValue.Nine, Owner = "board", Name = i + "MoneyNine", Image = "/Content/images/tiles/64px/man/man9.png" });

                _tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.Unrevealed, Value = TileValue.One, Owner = "board", Name = i + "RoundOne", Image = "/Content/images/tiles/64px/pin/pin1.png" });
                _tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.Unrevealed, Value = TileValue.Two, Owner = "board", Name = i + "RoundTwo", Image = "/Content/images/tiles/64px/pin/pin2.png" });
                _tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.Unrevealed, Value = TileValue.Three, Owner = "board", Name = i + "RoundThree", Image = "/Content/images/tiles/64px/pin/pin3.png" });
                _tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.Unrevealed, Value = TileValue.Four, Owner = "board", Name = i + "RoundFour", Image = "/Content/images/tiles/64px/pin/pin4.png" });
                _tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.Unrevealed, Value = TileValue.Five, Owner = "board", Name = i + "RoundFive", Image = "/Content/images/tiles/64px/pin/pin5.png" });
                _tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.Unrevealed, Value = TileValue.Six, Owner = "board", Name = i + "RoundSix", Image = "/Content/images/tiles/64px/pin/pin6.png" });
                _tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.Unrevealed, Value = TileValue.Seven, Owner = "board", Name = i + "RoundSeven", Image = "/Content/images/tiles/64px/pin/pin7.png" });
                _tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.Unrevealed, Value = TileValue.Eight, Owner = "board", Name = i + "RoundEight", Image = "/Content/images/tiles/64px/pin/pin8.png" });
                _tiles.Add(new Tile() { Type = TileType.Round, Status = TileStatus.Unrevealed, Value = TileValue.Nine, Owner = "board", Name = i + "RoundNine", Image = "/Content/images/tiles/64px/pin/pin9.png" });

                _tiles.Add(new Tile() { Type = TileType.Stick, Status = TileStatus.Unrevealed, Value = TileValue.One, Owner = "board", Name = i + "StickOne", Image = "/Content/images/tiles/64px/bamboo/bamboo1.png" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Status = TileStatus.Unrevealed, Value = TileValue.Two, Owner = "board", Name = i + "StickTwo", Image = "/Content/images/tiles/64px/bamboo/bamboo2.png" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Status = TileStatus.Unrevealed, Value = TileValue.Three, Owner = "board", Name = i + "StickThree", Image = "/Content/images/tiles/64px/bamboo/bamboo3.png" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Status = TileStatus.Unrevealed, Value = TileValue.Four, Owner = "board", Name = i + "StickFour", Image = "/Content/images/tiles/64px/bamboo/bamboo4.png" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Status = TileStatus.Unrevealed, Value = TileValue.Five, Owner = "board", Name = i + "StickFive", Image = "/Content/images/tiles/64px/bamboo/bamboo5.png" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Status = TileStatus.Unrevealed, Value = TileValue.Six, Owner = "board", Name = i + "StickSix", Image = "/Content/images/tiles/64px/bamboo/bamboo6.png" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Status = TileStatus.Unrevealed, Value = TileValue.Seven, Owner = "board", Name = i + "StickSeven", Image = "/Content/images/tiles/64px/bamboo/bamboo7.png" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Status = TileStatus.Unrevealed, Value = TileValue.Eight, Owner = "board", Name = i + "StickEight", Image = "/Content/images/tiles/64px/bamboo/bamboo8.png" });
                _tiles.Add(new Tile() { Type = TileType.Stick, Status = TileStatus.Unrevealed, Value = TileValue.Nine, Owner = "board", Name = i + "StickNine", Image = "/Content/images/tiles/64px/bamboo/bamboo9.png" });

                _tiles.Add(new Tile() { Type = TileType.Dragon, Status = TileStatus.Unrevealed, Value = TileValue.DragonGreen, Owner = "board", Name = i + "DragonGreen", Image = "/Content/images/tiles/64px/dragon/dragon-green.png" });
                _tiles.Add(new Tile() { Type = TileType.Dragon, Status = TileStatus.Unrevealed, Value = TileValue.DragonRed, Owner = "board", Name = i + "DragonRed", Image = "/Content/images/tiles/64px/dragon/dragon-chun.png" });
                _tiles.Add(new Tile() { Type = TileType.Dragon, Status = TileStatus.Unrevealed, Value = TileValue.DragonWhite, Owner = "board", Name = i + "DragonWhite", Image = "/Content/images/tiles/64px/dragon/dragon-white.png" });

                _tiles.Add(new Tile() { Type = TileType.Wind, Status = TileStatus.Unrevealed, Value = TileValue.WindNorth, Owner = "board", Name = i + "WindNorth", Image = "/Content/images/tiles/64px/wind/wind-north.png" });
                _tiles.Add(new Tile() { Type = TileType.Wind, Status = TileStatus.Unrevealed, Value = TileValue.WindEast, Owner = "board", Name = i + "WindEast", Image = "/Content/images/tiles/64px/wind/wind-east.png" });
                _tiles.Add(new Tile() { Type = TileType.Wind, Status = TileStatus.Unrevealed, Value = TileValue.WindSouth, Owner = "board", Name = i + "WindSouth", Image = "/Content/images/tiles/64px/wind/wind-south.png" });
                _tiles.Add(new Tile() { Type = TileType.Wind, Status = TileStatus.Unrevealed, Value = TileValue.WindWest, Owner = "board", Name = i + "WindWest", Image = "/Content/images/tiles/64px/wind/wind-west.png" });
            }
            _tiles.Add(new Tile() { Type = TileType.Flower, Status = TileStatus.Unrevealed, Value = TileValue.FlowerNumericOne, Owner = "board", Name = "137FlowerNumeric", Image = "/Content/images/tiles/64px/flower/flower1.png" });
            _tiles.Add(new Tile() { Type = TileType.Flower, Status = TileStatus.Unrevealed, Value = TileValue.FlowerNumericTwo, Owner = "board", Name = "138FlowerNumeric", Image = "/Content/images/tiles/64px/flower/flower2.png" });
            _tiles.Add(new Tile() { Type = TileType.Flower, Status = TileStatus.Unrevealed, Value = TileValue.FlowerNumericThree, Owner = "board", Name = "139FlowerNumeric", Image = "/Content/images/tiles/64px/flower/flower3.png" });
            _tiles.Add(new Tile() { Type = TileType.Flower, Status = TileStatus.Unrevealed, Value = TileValue.FlowerNumericFour, Owner = "board", Name = "140FlowerNumeric", Image = "/Content/images/tiles/64px/flower/flower4.png" });

            _tiles.Add(new Tile() { Type = TileType.Flower, Status = TileStatus.Unrevealed, Value = TileValue.FlowerRomanOne, Owner = "board", Name = "141FlowerRoman", Image = "/Content/images/tiles/64px/flower/flower1a.png" });
            _tiles.Add(new Tile() { Type = TileType.Flower, Status = TileStatus.Unrevealed, Value = TileValue.FlowerRomanTwo, Owner = "board", Name = "142FlowerRoman", Image = "/Content/images/tiles/64px/flower/flower2b.png" });
            _tiles.Add(new Tile() { Type = TileType.Flower, Status = TileStatus.Unrevealed, Value = TileValue.FlowerRomanThree, Owner = "board", Name = "143FlowerRoman", Image = "/Content/images/tiles/64px/flower/flower3c.png" });
            _tiles.Add(new Tile() { Type = TileType.Flower, Status = TileStatus.Unrevealed, Value = TileValue.FlowerRomanFour, Owner = "board", Name = "144FlowerRoman", Image = "/Content/images/tiles/64px/flower/flower4d.png" });


            int _tileid = 1;
            foreach (var tile in _tiles)
            {
                tile.Id = _tileid;
                _tileid++;
            }
        }
    }
}