using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using System.Text.Json;
using SigmaExplorer.Shared;

public class GraphQLInterface
{
    private static readonly GraphQLHttpClient _graphQLClient = new GraphQLHttpClient(Globals.GraphQLEndpoint, new SystemTextJsonSerializer());

    public static async Task<GQLStateWrapper> GetNetworkState()
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query State($take: Int) {
                  state {
                    height
                    network
                    difficulty
                    blockId
                  }
                  blocks(take: $take) {
                    height
                    timestamp
                    difficulty
                    minerAddress
                  }
                }
            ",
            OperationName = "State",
            Variables = new
            {
                take = 1
            }
        };


        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLStateWrapper>(query);
        return graphQLResponse.Data;
    }
    public static async Task<List<GQLBlockMinimal>> GetLatestBlocksBatch(int iTake, int iOffset)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query GetBlocks($take:Int, $skip: Int) {
                  blocks(take:$take, skip:$skip) {
                    headerId
                    height
                    timestamp
                    txsCount
                    minerAddress
                    minerReward
                    difficulty
                    blockSize
                  }
                }
            ",
            OperationName = "GetBlocks",
            Variables = new
            {
                skip = iOffset,
                take = iTake
            }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLBlocksMinimalResult>(query);
        return graphQLResponse.Data.blocks;
    }
    public static async Task<List<GQLBlockMinimal>> GetLatestBlocks(int iCount, int iOffset=0)
    {
        List<GQLBlockMinimal> blocks = new List<GQLBlockMinimal>();

        for (var i = 0; i < iCount;)
        {
            List<GQLBlockMinimal> blocksBatch = await GetLatestBlocksBatch(Math.Min(Globals.GraphQLBatchSize, (iCount - i)), iOffset + i);
            i += blocksBatch.Count;
            blocks.AddRange(blocksBatch);
        }

        return blocks;
    }

    public static async Task<List<GQLBlockTimestampOnly>?> GetLatestBlockTimestampsBatch(int iTake, int iOffset, int? minHeight, int? maxHeight)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query GetBlocks($take:Int, $skip: Int, $minHeight: Int, $maxHeight: Int) {
                  blocks(take:$take, skip:$skip, minHeight: $minHeight, maxHeight: $maxHeight) {
                    height
                    timestamp
                    difficulty
                    minerAddress
                  }
                }
            ",
            OperationName = "GetBlocks",
            Variables = new
            {
                skip = iOffset,
                take = iTake,
                minHeight = minHeight,
                maxHeight = maxHeight
            }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLBlockTimestampOnlyResult>(query);
        return graphQLResponse.Data?.blocks;
    }

    public static async Task<List<GQLBlockTimestampOnly>> GetLatestBlockTimestamps(int iMaxCount, int iOffset, int? minHeight, int? maxHeight)
    {
        var blocks = new List<GQLBlockTimestampOnly>();
        
        var remainingCount = iMaxCount;

        while (remainingCount > 0)
        {
            var batchSize = Math.Min(Globals.GraphQLBatchSize, remainingCount);
            var blocksBatch = await GetLatestBlockTimestampsBatch(batchSize, iOffset + blocks.Count, minHeight, maxHeight);

            if (blocksBatch == null || blocksBatch.Count == 0) break;

            blocks.AddRange(blocksBatch);
            remainingCount -= blocksBatch.Count;

            if (blocksBatch.Count < batchSize) break;
        }

        return blocks;
    }

    public static async Task<GQLBlockDetail> GetBlock(string? headerId, int? height)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query GetBlockDetail($headerId:String, $height:Int) {
                  blocks(headerId: $headerId, height: $height) {
                    height
                    timestamp
                    header {
                      headerId
                      parentId
                      difficulty
                      extensionHash
                      version
                      votes
                      adProofsRoot
                      transactionsRoot
                      stateRoot
                      nBits
                      powSolutions
                      extension {
                        headerId
                        digest
                      }
                      adProof {
                        headerId
                        digest
                        proofBytes
                      }
                    }
                    blockSize
                  }
                }
            ",
            OperationName = "GetBlockDetail",
            Variables = new
            {
                headerId = headerId,
                height = height
            }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLBlockDetailResult>(query);
        return graphQLResponse.Data.blocks.FirstOrDefault();
    }

    public static async Task<int> CountTransactions(string? headerId, string? address)
    {
        string[] addresses = new string[] { address };
        if (headerId != null && headerId != "")
        {
            var query = new GraphQLRequest
            {
                Query = @"
                query CountTransactionsBlock($headerId: String) {
                  blocks(headerId: $headerId) {
                    txsCount
                  }
                }
            ",
                OperationName = "CountTransactionsBlock",
                Variables = new
                {
                    headerId = headerId
                }
            };

            var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLCountBlockTransactionsResult>(query);
            return graphQLResponse.Data.blocks.First().txsCount;
           
        }
        else
        {
            var query = new GraphQLRequest
            {
                Query = @"
                query CountTransactionsAddress($addresses: [String!]!) {
                  addresses(addresses: $addresses) {
                    transactionsCount
                  }
                }
            ",
                OperationName = "CountTransactionsAddress",
                Variables = new
                {
                    addresses = addresses
                }
            };

            var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLCountAddressTransactionsResult>(query);
            return graphQLResponse.Data.addresses.First().transactionsCount;
        }
    }

    public static async Task<int> CountMempoolTransactions(string? address)
    {
        if (address == null)
        {
            var query = new GraphQLRequest
            {
                Query = @"
                query GetMempoolTXCount {
                  mempool {
                    transactionsCount
                  }
                }
            ",
                OperationName = "GetMempoolTXCount"
            };

            var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLCountMempoolTransactionsResult>(query);
            return graphQLResponse.Data.mempool.transactionsCount;
        }
        else
        {
            var query = new GraphQLRequest
            {
                Query = @"
                query GetMempoolTXCount($address: String) {
                  mempool {
                    transactions(address: $address) {
                      transactionId
                    }
                  }
                }
            ",
                OperationName = "GetMempoolTXCount",
                Variables = new
                {
                    address = address
                }
            };

            var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLMempoolTransactionsResult>(query);
            return graphQLResponse.Data.mempool.transactions.Count();
        }
    }

    public static async Task<List<GQLTransactionDetail>> GetTransactionsDetailBatch(int iTake, int iOffset, string? headerId, string? address, string? transactionId = null)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query GetTransactions($take: Int, $skip: Int, $headerId: String, $address: String, $transactionId: String) {
                  transactions(take: $take, skip: $skip, headerId: $headerId, address: $address, transactionId: $transactionId) {
                    transactionId
                    timestamp
                    inclusionHeight
                    size
                    inputs {
                      index
                      box {
                        address
                        transactionId
                        value
                        assets {
                          tokenId
                          amount
                        }
                      }
                    }
                    outputs {
                      address
                      value
                      assets {
                        tokenId
                        amount
                      }
                    }
                  }
                }
            ",
            OperationName = "GetTransactions",
            Variables = new
            {
                skip = iOffset,
                take = iTake,
                headerId = headerId,
                address = address,
                transactionId = transactionId
            }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLTransactionDetailResult>(query);
        return graphQLResponse.Data.transactions;
    }
    public static async Task<List<GQLTransactionDetail>> GetMempoolTransactionsDetailBatch(int iTake, int iOffset, string? address, string? transactionId = null)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query GetMempoolTX($take: Int, $skip: Int, $address: String, $transactionId: String) {
                  mempool {
                    transactionsCount
                    transactions(take: $take, skip: $skip, address: $address, transactionId: $transactionId) {
                      transactionId
                      size
                      timestamp
                      inputs {
                        index
                        box {
                          address
                          transactionId
                          value
                          assets {
                            tokenId
                            amount
                          }
                        }
                      }
                      outputs {
                        address
                        value
                        assets {
                          tokenId
                          amount
                        }
                      }
                    }
                  }
                }
            ",
            OperationName = "GetMempoolTX",
            Variables = new
            {
                skip = iOffset,
                take = iTake,
                address = address,
                transactionId = transactionId
            }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLMempoolTransactionsResult>(query);
        return graphQLResponse.Data.mempool.transactions;
    }
    //mempool only
    public static async Task<List<GQLTransactionDetail>> GetAllMempoolTransactionsDetail()
    {
        List<GQLTransactionDetail> txes = new List<GQLTransactionDetail>();

        do
        {
            List<GQLTransactionDetail> txesBatch = await GetMempoolTransactionsDetailBatch(Globals.GraphQLBatchSize, txes.Count, null, null);
            txes.AddRange(txesBatch);
            if (txesBatch.Count < Globals.GraphQLBatchSize) break;
        }
        while (true);

        return txes;
    }
    //this only returns the value fields of the input and outputs, we need this to calculate the amount of inputs/outputs
    public static async Task<List<GQLTransactionDetail>> GetMempoolTransactionsMinimalBatch(int iTake, int iOffset, string? address, string? transactionId = null)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query GetMempoolTX($take: Int, $skip: Int, $address: String, $transactionId: String) {
                  mempool {
                    transactionsCount
                    transactions(take: $take, skip: $skip, address: $address, transactionId: $transactionId) {
                      transactionId
                      size
                      timestamp
                    }
                  }
                }
            ",
            OperationName = "GetMempoolTX",
            Variables = new
            {
                skip = iOffset,
                take = iTake,
                address = address,
                transactionId = transactionId
            }
        };

        //01/10/2022 removed inputs and outputs (both only index) this reduced time fro; 500ms to 80ms and size from 50kb to 6kb

        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLMempoolTransactionsResult>(query);
        return graphQLResponse.Data.mempool.transactions;
    }
    //mempool only
    public static async Task<List<GQLTransactionDetail>> GetAllMempoolTransactionsMinimalBatch()
    {
        List<GQLTransactionDetail> txes = new List<GQLTransactionDetail>();

        do
        {
            List<GQLTransactionDetail> txesBatch = await GetMempoolTransactionsMinimalBatch(Globals.GraphQLBatchSize, txes.Count, null, null);
            txes.AddRange(txesBatch);
            if (txesBatch.Count < Globals.GraphQLBatchSize) break; // no more transactions to fetch
        }
        while (true);

        //return txes ordered by newest first.
        return txes.OrderByDescending(e => Convert.ToDouble(e.timestamp)).ToList();
    }
    //Includes confirmed and unconfirmed txes if headerId = null
    public static async Task<List<GQLTransactionDetail>> GetAllTransactionsDetail(string? headerId, string? address, string? transactionId = null)
    {
        List<GQLTransactionDetail> txes = new List<GQLTransactionDetail>();

        await Task.WhenAll(
            GetTransactionsDetailBatchesAsync(txes, headerId, address, transactionId),
            GetMempoolTransactionsDetailBatchesAsync(txes, address, transactionId)
        );

        return txes;
    }

    private static async Task GetTransactionsDetailBatchesAsync(List<GQLTransactionDetail> txes, string? headerId, string? address, string? transactionId)
    {
        do
        {
            List<GQLTransactionDetail> txesBatch = await GetTransactionsDetailBatch(Globals.GraphQLBatchSize, txes.Count, headerId, address, transactionId);
            txes.AddRange(txesBatch);
            if (txesBatch.Count < Globals.GraphQLBatchSize) break;
        }
        while (true);
    }

    private static async Task GetMempoolTransactionsDetailBatchesAsync(List<GQLTransactionDetail> txes, string? address, string? transactionId)
    {
        do
        {
            List<GQLTransactionDetail> txesBatch = await GetMempoolTransactionsDetailBatch(Globals.GraphQLBatchSize, txes.Count, address, transactionId);
            txes.AddRange(txesBatch);
            if (txesBatch.Count < Globals.GraphQLBatchSize) break;
        }
        while (true);
    }
    //Works for both confirmed and unconfirmed txes.
    public static async Task<GQLTransactionDetail?> GetTransaction(string transactionId)
    {
        List<GQLTransactionDetail> txes = await GetAllTransactionsDetail(null, null, transactionId);
        if (txes != null && txes.Count > 0)
        {
            return txes.First();
        }
        return null;
    }

    //Get the full details of a transaction. Mempool and non mempool, i think.
    public static async Task<GQLTransactionDetailFull?> GetTransactionDetailFullBatch(string transactionId)
    {

        /*
         left out the output assets because it causes the query to become nonresponsive
         */
        var query = new GraphQLRequest
        {
            Query = @"
                query GetTransactionFullDetail($transactionId: String) {
                  transactions(transactionId: $transactionId) {
                    transactionId
                    size
                    timestamp
                    inclusionHeight
                    inputs {
                      box {
                        boxId
                        value
                        index
                        transaction {
                          headerId
                          transactionId
                          index
                          globalIndex
                          timestamp
                          inclusionHeight
                        }
                        ergoTree
                        address
                        assets {
                          tokenId
                          amount
                        }
                        additionalRegisters
                      }
                    }
                    outputs {
                      boxId
                      transactionId
                      headerId
                      value
                      index
                      globalIndex
                      creationHeight
                      settlementHeight
                      ergoTree
                      address

                      additionalRegisters
                      spentBy {
                        transactionId
                        mainChain
                      }
                    }
                  }
                }
            ",
            OperationName = "GetTransactionFullDetail",
            Variables = new
            {
                transactionId = transactionId
            }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLTransactionDetailFullResult>(query);
        var list = graphQLResponse.Data.transactions;
        if (list.Count == 0) return null;
        return list.First();
    }

    public static async Task<List<GQLAddress>> GetAddresses(List<string> addresses)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query GetAddresses($addresses: [String!]!) {
                  addresses(addresses: $addresses) {
                    address
                    transactionsCount
                    used
                    balance {
                      nanoErgs
                      assets {
                        amount
                        tokenId
                        name
                        decimals
                      }
                    }
                  }
                }
            ",
            OperationName = "GetAddresses",
            Variables = new
            {
                addresses = addresses.ToArray()
            }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLAddressResult>(query);
        return graphQLResponse.Data?.addresses;
    }

    public static async Task<GQLAddress> GetAddressInfo(string address)
    {
        List<GQLAddress>? addresses = await GetAddresses(new List<string>() { address });
        return addresses?.First();
    }

    public static async Task<List<GQLToken>?> GetTokenInfoByIds(List<string> tokenIds)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query TokensInfo($tokenIds: [String!]) {
                  tokens(tokenIds: $tokenIds) {
                    tokenId
                    name
                    decimals
                    type
                  }
                }
            ",
            OperationName = "TokensInfo",
            Variables = new
            {
                tokenIds = tokenIds.ToArray()
            }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLTokenResult>(query);
        if (graphQLResponse.Data.tokens == null || graphQLResponse.Data.tokens.Count == 0)
        {
            return null;
        }
        return graphQLResponse.Data.tokens;
    }


    public static async Task<GQLTokenDetail?> GetTokenDetailById(string tokenId)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query TokenInfo($tokenId: String) {
                  tokens(tokenId: $tokenId) {
                    tokenId
                    emissionAmount
                    name
                    description
                    type
                    box {
                      additionalRegisters
                      transaction {
                        transactionId
                        timestamp
                        inclusionHeight
                      }
                    }
                  }
                }
            ",
            OperationName = "TokenInfo",
            Variables = new
            {
                tokenId = tokenId
            }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLTokenDetailResult>(query);
        if (graphQLResponse.Data.tokens == null || graphQLResponse.Data.tokens.Count == 0)
        {
            return null;
        }
        GQLTokenDetail token = graphQLResponse.Data.tokens.First();
        return token;
    }

    public static async Task<List<GQLBoxesTokensBox>?> GetUnspentBoxesForAssetsForAddress(int iTake, int iOffset, string address)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query GetAssets($spent: Boolean, $address: String, $take: Int, $skip: Int) {
                  boxes(spent: $spent, address: $address, take: $take, skip: $skip) {
                    boxId
                    assets {
                      token {
                        tokenId
                        name
                        decimals
                        type
                      }
                      amount
                    }
                  }
                }
            ",
            OperationName = "GetAssets",
            Variables = new
            {
                skip = iOffset,
                take = iTake,
                address = address,
                spent = false
            }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<GQLBoxesTokensResult>(query);
        if (graphQLResponse.Data.boxes == null || graphQLResponse.Data.boxes.Count == 0)
        {
            return null;
        }
        
        return graphQLResponse.Data.boxes;
    }

    public static async Task<List<GQLBoxesTokensAsset>> GetAllUnspentAssetsForAddress(string address)
    {
        List<GQLBoxesTokensBox> boxes = new List<GQLBoxesTokensBox>();

        do
        {
            List<GQLBoxesTokensBox>? boxesBatch = await GetUnspentBoxesForAssetsForAddress(Globals.GraphQLBatchSize, boxes.Count, address);
            if (boxesBatch == null) break;
            boxes.AddRange(boxesBatch);
            if (boxes.Count < Globals.GraphQLBatchSize) break;
        }
        while (true);

        List<GQLBoxesTokensAsset> assets = boxes
        .SelectMany(box => box.assets)
        .GroupBy(asset => asset.token.tokenId)
        .Select(group => new GQLBoxesTokensAsset { token = group.First().token, amount = group.Sum(asset => Convert.ToDouble(asset.amount)).ToString() })
        .ToList();

        return assets;
    }
}
