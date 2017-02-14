using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBAScraper
{
    class Match
    {
        public int MatchNumber { get; set; }
        public string[] Teams { get; set; }
        public string CompLevel { get; set; }

        public string Tuple
        {
            get
            {
                return String.Empty;
            }
        }
    }
}
