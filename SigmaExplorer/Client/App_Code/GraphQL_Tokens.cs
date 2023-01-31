using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigmaExplorer.Shared
{
    public class GQLToken
    {
        public string tokenId { get; set; }
        public string name { get; set; }
        public int? decimals { get; set; }
        public string type { get; set; }
    }

    public class GQLTokenResult
    {
        public List<GQLToken> tokens { get; set; }
    }

    public class GQLTokenDetailTransaction
    {
        public string transactionId { get; set; }
        public string timestamp { get; set; }
        public int inclusionHeight { get; set; }
    }
    public class GQLTokenDetailBox
    {
        public GQLAdditionalRegisters? additionalRegisters { get; set; }
        public GQLTokenDetailTransaction transaction { get; set; }
    }
    public class GQLTokenDetail
    {
        public string tokenId { get; set; }
        public string emissionAmount { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public int? decimals { get; set; }
        public GQLTokenDetailBox box { get; set; }
    }

    public class GQLTokenDetailResult
    {
        public List<GQLTokenDetail> tokens { get; set; }
    }

}
