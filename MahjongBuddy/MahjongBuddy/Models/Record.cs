using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class Record
    {
        public int GameNo { get; set; }
        public string WinnerUserID { get; set; }
        public IEnumerable<Tile> WinningTiles { get; set; }
    }
}