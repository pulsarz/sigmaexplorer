using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigmaExplorer.Shared
{
    public class MinerInfo
    {
        public string address { get; set; }
        public string name { get; set; }
    }

    public class MinerDistribution
    {
        public string address { get; set; }
        public int blocks { get; set; }
    }

    public class HashrateInfo
    {
        public int height { get; set; }
        public DateTime timestamp { get; set; }
        public double hashrate { get; set; }
        public double hashrateTHs { get; set; }
        public double difficulty { get; set; }
        public double difficultyPH { get; set; }
    }
}
