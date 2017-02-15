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
                var tupleString = "(";
                foreach(var team in Teams)
                {
                    tupleString = tupleString + team + ",";
                }
                tupleString = tupleString.Remove(tupleString.Length - 1, 1) + ")";
                return tupleString;
            }
        }

        public string SelectOption(int index)
        {
            var tupleString = index.ToString() + ": [";
            foreach (var team in Teams)
            {
                tupleString = tupleString + team + ",";
            }
            tupleString = tupleString.Remove(tupleString.Length - 1, 1) + "]";
            return tupleString;
        }
    }
}
