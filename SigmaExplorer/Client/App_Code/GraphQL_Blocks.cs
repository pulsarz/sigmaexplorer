using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigmaExplorer.Shared
{
    public class GQLBlockTimestampOnly
    {
        public int height { get; set; }
        public string timestamp { get; set; }
        public string difficulty { get; set; }
        public string minerAddress { get; set; }
    }
    public class GQLBlockTimestampOnlyResult
    {
        public List<GQLBlockTimestampOnly> blocks { get; set; }
    }
    public class GQLBlockMinimal
    {
        public string headerId { get; set; }
        public int height { get; set; }
        public string timestamp { get; set; }
        public int txsCount { get; set; }
        public string minerAddress { get; set; }
        public string minerReward { get; set; }
        public string difficulty { get; set; }
        public int blockSize { get; set; }
    }
    public class GQLBlocksMinimalResult
    {
        public List<GQLBlockMinimal> blocks { get; set; }
    }

    public class GQLBlockDetailHeaderPoWSolutions
    {
        public string pk { get; set; }
        public string w { get; set; }
        public string n { get; set; }
        public string d { get; set; }
    }
    public class GQLBlockDetailHeaderExtension
    {
        public string headerId { get; set; }
        public string digest { get; set; }
        public string fields { get; set; }//json object, broken in graphql < 0.4.1
    }
    public class GQLBlockDetailHeaderADProof
    {
        public string headerId { get; set; }
        public string digest { get; set; }
        public string proofBytes { get; set; }
    }
    public class GQLBlockDetailHeader
    {
        public string headerId { get; set; }
        public string parentId { get; set; }
        public string difficulty { get; set; }
        public string extensionHash { get; set; }
        public int version { get; set; }
        public int[] votes { get; set; }
        public string adProofsRoot { get; set; }
        public string transactionsRoot { get; set; }
        public string stateRoot { get; set; }
        public string nBits { get; set; }
        public GQLBlockDetailHeaderPoWSolutions powSolutions { get; set; }
        public GQLBlockDetailHeaderExtension extension { get; set; }
        public GQLBlockDetailHeaderADProof adProof { get; set; }
    }
    public class GQLBlockDetail
    {
        public int height { get; set; }
        public string timestamp { get; set; }
        public GQLBlockDetailHeader header { get; set; }
        public int blockSize { get; set; }
    }

    public class GQLBlockDetailResult
    {
        public List<GQLBlockDetail> blocks { get; set; }
    }

    public class GQLCountBlockTransactions
    {
        public int txsCount { get; set; }
    }
    public class GQLCountBlockTransactionsResult
    {
        public List<GQLCountBlockTransactions> blocks { get; set; }
    }

}
