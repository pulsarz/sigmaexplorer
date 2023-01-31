using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigmaExplorer.Shared
{
    public class GQLState
    {
        public int height { get; set; }
        public string network { get; set; }
        public string difficulty { get; set; }
        public string blockId { get; set; }
    }

    public class GQLStateWrapper
    {
        public GQLState state { get; set; }
        public List<GQLBlockTimestampOnly> blocks { get; set; }//for latest block info
    }
}
