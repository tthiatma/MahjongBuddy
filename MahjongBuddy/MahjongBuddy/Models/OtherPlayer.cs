using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class OtherPlayer : Player
    {
        public int ActiveTilesCount { get { return ActivePlayer != null ? ActivePlayer.ActiveTiles.Count() : 0; } }

        [JsonIgnore]
        public ActivePlayer ActivePlayer { get; set; }

        public OtherPlayer(string name, string group, string hash) 
            :base(name, group, hash)
        {
        }

        public OtherPlayer(Player player) :base(player.Name, player.Group , player.Hash)
        { 
        
        }
    }
}