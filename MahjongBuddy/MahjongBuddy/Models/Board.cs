using MahjongBuddy.Models.Tiles;
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
                //1 - 35 - 69 - 103
                _tiles.Add(new Money1() {Name = i + "MoneyOne"});
                _tiles.Add(new Money2() {Name = i + "MoneyTwo"});
                _tiles.Add(new Money3() {Name = i + "MoneyThree"});
                _tiles.Add(new Money4() {Name = i + "MoneyFour"});
                _tiles.Add(new Money5() {Name = i + "MoneyFive"});
                _tiles.Add(new Money6() {Name = i + "MoneySix"});
                _tiles.Add(new Money7() {Name = i + "MoneySeven"});
                _tiles.Add(new Money8() {Name = i + "MoneyEight"});
                _tiles.Add(new Money9() {Name = i + "MoneyNine"});

                //10 - 44 - 78 - 112
                _tiles.Add(new Round1() {Name = i + "RoundOne"});
                _tiles.Add(new Round2() {Name = i + "RoundTwo"});
                _tiles.Add(new Round3() {Name = i + "RoundThree"});
                _tiles.Add(new Round4() {Name = i + "RoundFour"});
                _tiles.Add(new Round5() {Name = i + "RoundFive"});
                _tiles.Add(new Round6() {Name = i + "RoundSix"});
                _tiles.Add(new Round7() {Name = i + "RoundSeven"});
                _tiles.Add(new Round8() {Name = i + "RoundEight"});
                _tiles.Add(new Round9() {Name = i + "RoundNine"});

                //19 - 53 - 87 - 121
                _tiles.Add(new Stick1() {Name = i + "StickOne"});
                _tiles.Add(new Stick2() {Name = i + "StickTwo"});
                _tiles.Add(new Stick3() {Name = i + "StickThree"});
                _tiles.Add(new Stick4() {Name = i + "StickFour"});
                _tiles.Add(new Stick5() {Name = i + "StickFive"});
                _tiles.Add(new Stick6() {Name = i + "StickSix"});
                _tiles.Add(new Stick7() {Name = i + "StickSeven"});
                _tiles.Add(new Stick8() {Name = i + "StickEight"});
                _tiles.Add(new Stick9() {Name = i + "StickNine"});

                //28 - 62 - 96 - 130
                _tiles.Add(new DragonGreen() {Name = i + "DragonGreen"});
                _tiles.Add(new DragonRed() {Name = i + "DragonRed"});
                _tiles.Add(new DragonWhite() {Name = i + "DragonWhite"});

                //31 - 65 - 99 - 133
                _tiles.Add(new WindNorth() { Name = i + "WindNorth" });
                _tiles.Add(new WindEast() { Name = i + "WindEast" });
                _tiles.Add(new WindSouth() { Name = i + "WindSouth" });
                _tiles.Add(new WindWest() { Name = i + "WindWest" });
                
            }
            _tiles.Add(new FlowerNumeric1() { Name = "137FlowerNumeric" });
            _tiles.Add(new FlowerNumeric2() { Name = "138FlowerNumeric" });
            _tiles.Add(new FlowerNumeric3() { Name = "139FlowerNumeric" });
            _tiles.Add(new FlowerNumeric4() { Name = "140FlowerNumeric" });

            _tiles.Add(new FlowerRoman1() { Name = "141FlowerRoman" });
            _tiles.Add(new FlowerRoman2() { Name = "142FlowerRoman" });
            _tiles.Add(new FlowerRoman3() { Name = "143FlowerRoman" });
            _tiles.Add(new FlowerRoman4() { Name = "144FlowerRoman" });


            int _tileid = 1;
            foreach (var tile in _tiles)
            {
                tile.Id = _tileid;
                _tileid++;
            }
        }
    }
}