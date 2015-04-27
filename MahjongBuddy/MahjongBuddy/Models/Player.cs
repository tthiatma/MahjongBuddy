using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class Player
    {
        public Player(string name, string group, string hash)
        {
            Name = name;
            Hash = hash;
            Id = Guid.NewGuid().ToString("d");
            Matches = new List<int>();
            Group = group;
        }

        public string ConnectionId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string Group { get; set; }
        public bool IsPlaying { get; set; }
        public bool CanPickTile { get; set; }
        public WindDirection Wind { get; set; }
        public List<int> Matches { get; set; }
        public Point CurrentPoint { get; set; }
        public int Seat { get; set; }

    }
}