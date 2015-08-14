using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class Record
    {
        public int GameNo { get; set; }
        public Player Winner { get; set; }
        public Player Feeder { get; set; }
        public WinningTileSet WinningTileSet { get; set; }

        public Record()
        {
            WinningTileSet = new WinningTileSet();
        }

    }
}