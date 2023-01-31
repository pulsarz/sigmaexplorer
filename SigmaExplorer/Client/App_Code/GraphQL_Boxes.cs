using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigmaExplorer.Shared
{

    public class GQLBoxesTokensAsset
    {
        public GQLToken token { get; set; }
        public string amount { get; set; }
    }

    public class GQLBoxesTokensBox
    {
        public string boxId { get; set; }
        public List<GQLBoxesTokensAsset> assets { get; set; }
    }

    public class GQLBoxesTokensResult
    {
        public List<GQLBoxesTokensBox> boxes { get; set; }
    }

}
