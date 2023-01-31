using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigmaExplorer.Shared
{
   /* public class GQLTransactionDetailAssetToken
    {
        public string name { get; set; }
        public int? decimals { get; set; }
        public string type { get; set; }
    }*/
    public class GQLTransactionDetailAsset
    {
        public string tokenId { get; set; }
        public string amount { get; set; }
        //public GQLTransactionDetailAssetToken token { get; set; }/*causes 60s timeout in outputs..*/
    }
    public class GQLTransactionDetailBox
    {
        public string? address { get; set; }
        public string? transactionId { get; set; }
        public string value { get; set; }
        public List<GQLTransactionDetailAsset>? assets { get; set; }
    }
    public class GQLTransactionDetailInput
    {
        public int? index { get; set; }
        public GQLTransactionDetailBox box { get; set; }
    }
    public class GQLTransactionDetailOutput
    {
        public string? address { get; set; }
        public string value { get; set; }
        public int? index { get; set; }
        public List<GQLTransactionDetailAsset>? assets { get; set; }
    }
    public class GQLTransactionDetail
    {
        public string transactionId { get; set; }
        public string timestamp { get; set; }
        public int? inclusionHeight { get; set; }
        public long size { get; set; }

        public List<GQLTransactionDetailInput> inputs { get; set; }
        public List<GQLTransactionDetailOutput> outputs { get; set; }
    }
    public class GQLTransactionDetailResult
    {
        public List<GQLTransactionDetail> transactions { get; set; }
    }

    public class GQLCountAddressTransactions
    {
        public int transactionsCount { get; set; }
    }
    public class GQLCountAddressTransactionsResult
    {
        public List<GQLCountAddressTransactions> addresses { get; set; }
    }

    public class GQLMempool
    {
        public int? transactionsCount { get; set; }
        public List<GQLTransactionDetail?> transactions { get; set; }
    }
    public class GQLMempoolTransactionsResult
    {
        public GQLMempool mempool { get; set; }
    }

    public class GQLCountMempoolTransactions
    {
        public int transactionsCount { get; set; }
    }
    public class GQLCountMempoolTransactionsResult
    {
        public GQLCountMempoolTransactions mempool { get; set; }
    }



    public class GQLTransactionDetailFullInputOriginTransaction
    {
        public string headerId { get; set; }
        public string transactionId { get; set; }
        public int index { get; set; }
        public string globalIndex { get; set; }
        public string timestamp { get; set; }
        public int? inclusionHeight { get; set; }
    }
    public class GQLTransactionDetailFullBox
    {
        public string boxId { get; set; }
        public string value { get; set; }
        public int index { get; set; }
        public string ergoTree { get; set; }
        public string address { get; set; }
        public string additionalRegisters { get; set; }//json
        public List<GQLTransactionDetailAsset>? assets { get; set; }
        public GQLTransactionDetailFullInputOriginTransaction transaction { get; set; }
    }
    public class GQLTransactionDetailFullSpentBy
    {
        public string transactionId { get; set; }
        public bool mainChain { get; set; }
    }

    public class GQLTransactionDetailFullInput
    {
        public GQLTransactionDetailBox box { get; set; }
    }
    public class GQLTransactionDetailFullOutput
    {
        public string boxId { get; set; }
        public string transactionId { get; set; }
        public string? headerId { get; set; }
        public string value { get; set; }
        public int index { get; set; }
        public string? globalIndex { get; set; }
        public int creationHeight { get; set; }
        public int? settlementHeight { get; set; }
        public string ergoTree { get; set; }
        public string address { get; set; }
        public string additionalRegisters { get; set; }//json

        public GQLTransactionDetailFullSpentBy? spentBy { get; set; }
        //public List<GQLTransactionDetailAsset>? assets { get; set; }
    }
    public class GQLTransactionDetailFull
    {
        public string transactionId { get; set; }
        public long size { get; set; }
        public string timestamp { get; set; }
        public int? inclusionHeight { get; set; }
        

        public List<GQLTransactionDetailFullInput> inputs { get; set; }
        public List<GQLTransactionDetailFullOutput> outputs { get; set; }
    }

    public class GQLTransactionDetailFullResult
    {
        public List<GQLTransactionDetailFull> transactions { get; set; }
    }
}
