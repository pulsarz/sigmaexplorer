using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigmaExplorer.Shared
{
    public class GQLAddressBalance
    {
        public string nanoErgs { get; set; }
        public List<GQLToken> assets { get; set; }
    }

    public class GQLAddress
    {
        public string address { get; set; }
        public int transactionsCount { get; set; }
        public bool used { get; set; }
        public GQLAddressBalance balance { get; set; }
    }

    public class GQLAddressResult
    {
        public List<GQLAddress> addresses { get; set; }
    }
}
