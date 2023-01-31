using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigmaExplorer.Shared
{
    public class BlockTimeInfo
    {
        public int height { get; set; }
        public double minedForMs { get; set; }
        public double minedForSeconds { get; set; }
    }

}
