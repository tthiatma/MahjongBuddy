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
        private List<Tile> _graveyardTiles = new List<Tile>();

        public List<Tile> GraveyardTiles { get { return _graveyardTiles; } set {_graveyardTiles = value;} }
        public List<Tile> Tiles{ get { return _tiles; } set { _tiles = value; } }
        public void CreateTiles(){

            for (var i = 1; i < 5; i++)
            {
                //1 - 35 - 69 - 103
                _tiles.Add(new Money1() {Name = i + "MoneyOne"});   //0/34/68/102
                _tiles.Add(new Money2() {Name = i + "MoneyTwo"});   //1/35/69/103
                _tiles.Add(new Money3() {Name = i + "MoneyThree"}); //2/36/70/104
                _tiles.Add(new Money4() {Name = i + "MoneyFour"});  //3/37/71/105
                _tiles.Add(new Money5() {Name = i + "MoneyFive"});  //4/38/72/106
                _tiles.Add(new Money6() {Name = i + "MoneySix"});   //5/39/73/107
                _tiles.Add(new Money7() {Name = i + "MoneySeven"}); //6/40/74/108
                _tiles.Add(new Money8() {Name = i + "MoneyEight"}); //7/41/75/109
                _tiles.Add(new Money9() {Name = i + "MoneyNine"});  //8/42/76/110

                //10 - 44 - 78 - 112
                _tiles.Add(new Round1() {Name = i + "RoundOne"});   //43
                _tiles.Add(new Round2() {Name = i + "RoundTwo"});   //44
                _tiles.Add(new Round3() {Name = i + "RoundThree"}); //45
                _tiles.Add(new Round4() {Name = i + "RoundFour"});  //46
                _tiles.Add(new Round5() {Name = i + "RoundFive"});  //47
                _tiles.Add(new Round6() {Name = i + "RoundSix"});   //48
                _tiles.Add(new Round7() {Name = i + "RoundSeven"}); //49
                _tiles.Add(new Round8() {Name = i + "RoundEight"}); //50
                _tiles.Add(new Round9() {Name = i + "RoundNine"});  //51

                //19 - 53 - 87 - 121
                _tiles.Add(new Stick1() {Name = i + "StickOne"});   //52
                _tiles.Add(new Stick2() {Name = i + "StickTwo"});   //53
                _tiles.Add(new Stick3() {Name = i + "StickThree"}); //54
                _tiles.Add(new Stick4() {Name = i + "StickFour"});  //55
                _tiles.Add(new Stick5() {Name = i + "StickFive"});  //56
                _tiles.Add(new Stick6() {Name = i + "StickSix"});   //57
                _tiles.Add(new Stick7() {Name = i + "StickSeven"}); //58
                _tiles.Add(new Stick8() {Name = i + "StickEight"}); //59
                _tiles.Add(new Stick9() {Name = i + "StickNine"});  //60

                //28 - 62 - 96 - 130
                _tiles.Add(new DragonGreen() {Name = i + "DragonGreen"});   //27/61/95/129
                _tiles.Add(new DragonRed() {Name = i + "DragonRed"});       //28/62/96/130
                _tiles.Add(new DragonWhite() {Name = i + "DragonWhite"});   //29/63/97/131

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