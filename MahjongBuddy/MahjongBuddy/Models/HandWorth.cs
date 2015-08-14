using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{
    public class HandWorth
    {
        public WinningType WinningType { get; set; }
        public string HandName { get; set; }
        public int Point { get; set; }
    }
}