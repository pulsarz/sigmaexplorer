﻿using Blazored.LocalStorage;
using SigmaExplorer.Shared;

public static class BlockTools
{
    private static SemaphoreSlim semaphoreNetworkState = new SemaphoreSlim(1, 1);
    private static SemaphoreSlim semaphoreBlocksForStatistics = new SemaphoreSlim(1, 1);
    private static SemaphoreSlim semaphoreTokenInfo = new SemaphoreSlim(1, 1);

    public static async Task TryUpdateNetworkState(bool ForceUpdate = false)
    {
        await semaphoreNetworkState.WaitAsync();

        if (ForceUpdate || Globals.LastNetworkState == null || (DateTime.Now - Globals.LastNetworkStateUpdate).TotalSeconds > 10)
        {
            var newState = await GraphQLInterface.GetNetworkState();

            Globals.LastNetworkState = newState;
            Globals.LastNetworkStateUpdate = DateTime.Now;
        }

        semaphoreNetworkState.Release();
    }

    public static async Task<bool> IsTestnet()
    {
        await TryUpdateNetworkState();
        return (Globals.LastNetworkState.state.network != "mainnet");
    }

    public static async Task<int> GetErgoEpochLength()
    {
        var isTestnet = await IsTestnet();
        if (isTestnet) return 128;//testnet
        return 128;//mainnet
    }

    public static async Task<int> GetErgoDesiredBlockTimeMs()
    {
        var isTestnet = await IsTestnet();
        if (isTestnet) return 1000 * 45;//testnet
        return 1000 * 120;//mainnet
    }

    public static async Task<int> GetCurrentHeight()
    {
        await TryUpdateNetworkState();
        return Globals.LastNetworkState.state.height;
    }

    public static async Task<string?> GetCurrentBlockTimestamp()
    {
        await TryUpdateNetworkState();
        return Globals.LastNetworkState?.blocks?.First()?.timestamp;
    }

    public static async Task<string> GetCurrentDifficulty()
    {
        await TryUpdateNetworkState();
        return Globals.LastNetworkState.state.difficulty;
    }

    //Retrieves multiple tokens in chunks of 20 tokens per api call. Much more efficient for retrieving lots of tokens at once.
    //Not using localstorage anymore.
    public static async Task<List<GQLToken?>> GetTokensInfo(List<string> tokenIds)
    {
        List<GQLToken?> tokens = new List<GQLToken>();
        List<string> tokenIdsNotInCache = new List<string>();

        await semaphoreTokenInfo.WaitAsync();

        foreach (string tokenId in tokenIds)
        {
            GQLToken temp = null;
            if (Globals.tokenCache.TryGetValue(tokenId, out temp))
            {
                tokens.Add(temp.Clone());
            }
            else
            {
                //Need to retrieve it from api
                tokenIdsNotInCache.Add(tokenId);
            }
        }

        if (tokenIdsNotInCache.Count > 0)
        {
            List<GQLToken> newTokens = new List<GQLToken>();
            for (var i = 0; i < tokenIdsNotInCache.Count; i += 20)
            {
                List<GQLToken>? retrievedTokens = await GraphQLInterface.GetTokenInfoByIds(tokenIdsNotInCache.Skip(i).Take(20).ToList());
                if (retrievedTokens != null)
                {
                    newTokens.AddRange(retrievedTokens);
                    tokens.AddRange(newTokens);
                }
            }

            foreach (GQLToken token in newTokens)
            {
                //Insert out token in the global token cache
                Globals.tokenCache.Add(token.tokenId,token);//load it in the global so we dont have to make js calls again untill refresh
            }
        }

        semaphoreTokenInfo.Release();

        return tokens;
    }

    public static string TryGetKnownMinerName(string address, bool trimAddressLength)
    {
        string name = address.Substring(address.Length - 8);

        MinerInfo miner = null;
        if (Globals.KnownMiners.TryGetValue(address, out miner))
        {
            name = miner.name;
        }

        return name;
    }

    public static async Task<TimeSpan> GetTimespanSinceLastBlock()
    {
        string? lastBlockTimestamp = await GetCurrentBlockTimestamp();
        if (lastBlockTimestamp == null) return TimeSpan.MinValue;
        TimeSpan timespanSinceLastBlock = (DateTime.Now - Tools.UnixTimeStampMsToDateTime(Convert.ToDouble(lastBlockTimestamp)));
        return timespanSinceLastBlock;
    }

    public static double GetAverageBlockTime(List<GQLBlockTimestampOnly> Blocks)
    {
        List<double> timeDiffs = new List<double>(Blocks.Count-1);
        double previous = 0;

        Blocks = Blocks.OrderBy(e => Convert.ToDouble(e.timestamp)).ToList();

        foreach (GQLBlockTimestampOnly block in Blocks)
        {
            if (previous != 0)
            {
                double current = Convert.ToDouble(block.timestamp);
                timeDiffs.Add(current - previous);
            }
            previous = Convert.ToDouble(block.timestamp);
        }

        return Math.Round(timeDiffs.Average(), 0);
    }

    //BlockTimeInfo
    public static List<BlockTimeInfo> BuildBlockTimeInfoList(List<GQLBlockTimestampOnly> Blocks)
    {
        double previous = 0;
        List<BlockTimeInfo> timeInfoList = new List<BlockTimeInfo>(Blocks.Count-1);

        Blocks = Blocks.OrderBy(e => Convert.ToDouble(e.timestamp)).ToList();

        foreach (GQLBlockTimestampOnly block in Blocks)
        {
            if (previous != 0)
            {
                double current = Convert.ToDouble(block.timestamp);

                BlockTimeInfo timeInfo = new BlockTimeInfo();
                timeInfo.height = block.height;
                timeInfo.minedForMs = current - previous;
                timeInfo.minedForSeconds = Math.Round(timeInfo.minedForMs / 1000.0, 2);
                timeInfoList.Add(timeInfo);
            }
            previous = Convert.ToDouble(block.timestamp);
        }

        return timeInfoList;
    }
    public static async Task<List<GQLBlockTimestampOnly>> GetLastBlocksForStatistics(ILocalStorageService localStorage, int RequestedAmount)
    {
        List<GQLBlockTimestampOnly> blocks = new List<GQLBlockTimestampOnly>();
        int height = 0;
        bool IsUpdated = false;

        await semaphoreBlocksForStatistics.WaitAsync();
        height = await GetCurrentHeight();
        blocks = Globals.LastBlocksForStats;

        if (RequestedAmount <= Globals.MaxBlocksToStoreForStatsInLocalStorage)
        {
            if (blocks == null || blocks.Count == 0 )
            {
                List<GQLBlockTimestampOnly> temp = await localStorage.GetItemAsync<List<GQLBlockTimestampOnly>>("ErgoStats_LastBlocks_" + Tools.GetHashSHA1String(Globals.GraphQLEndpoint));
                if (temp != null) blocks.AddRange(temp);
            }

            if (blocks == null || blocks.Count == 0)
            {
                List<GQLBlockTimestampOnly> temp = await GraphQLInterface.GetLatestBlockTimestamps(Globals.MaxBlocksToStoreForStatsInLocalStorage, 0, null, height);
                if (temp != null) blocks.AddRange(temp);
                IsUpdated = true;
            }

            int LastHeightInCache = blocks.MaxBy(e => e.height)?.height ?? 0;
            int BlocksBehind = height - LastHeightInCache;
            if (BlocksBehind > 0)
            {
                //fetch new blocks only
                List<GQLBlockTimestampOnly>? newBlocks = await GraphQLInterface.GetLatestBlockTimestamps(Globals.MaxBlocksToStoreForStatsInLocalStorage, 0, LastHeightInCache + 1, height);
                blocks.AddRange(newBlocks);
                blocks = blocks.OrderByDescending(e => Convert.ToDouble(e.timestamp)).ToList();
                IsUpdated = true;
            }
            else if (BlocksBehind < 0)
            {
                // fork ? do nothing.
            }
            else
            {
                //in sync
            }

            if (blocks.Count > Globals.MaxBlocksToStoreForStatsInLocalStorage)
            {
                blocks = blocks.Take(Globals.MaxBlocksToStoreForStatsInLocalStorage).ToList();
            }

            if (IsUpdated)
            {
                await localStorage.SetItemAsync("ErgoStats_LastBlocks_" + Tools.GetHashSHA1String(Globals.GraphQLEndpoint), blocks);
                Globals.LastBlocksForStats.Clear();
                Globals.LastBlocksForStats = blocks.Clone();//use clone here
            }

            
            blocks = blocks.Take(RequestedAmount).ToList();
        }

        semaphoreBlocksForStatistics.Release();
        return blocks;
    }

    public static double CalculateNetworkHashrate(List<GQLBlockTimestampOnly> blocks)
    {
        double timestampMsNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        
        List<double> secondsPerBlock = new List<double>();
        List<double> difficultyPerBlock = new List<double>();
        double previous = 0;

        blocks = blocks.OrderBy(e => Convert.ToDouble(e.timestamp)).ToList();

        foreach (GQLBlockTimestampOnly block in blocks)
        {
            if (previous != 0)
            {
                double current = Convert.ToDouble(block.timestamp);
                secondsPerBlock.Add((current - previous) / 1000.0);
                difficultyPerBlock.Add(Convert.ToDouble(block.difficulty));
            }
            previous = Convert.ToDouble(block.timestamp);
        }

        return Math.Round(difficultyPerBlock.Sum() / secondsPerBlock.Sum(), 0);
    }

    public static async Task<double> GetCurrentNetworkHashrate(ILocalStorageService localStorage)
    {
        List<GQLBlockTimestampOnly> LastBlocks = await BlockTools.GetLastBlocksForStatistics(localStorage, Globals.MaxBlocksToStoreForStatsInLocalStorage);
        double timestampMsNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        double millisIn1Day = (24 * 60.0 * 60.0 * 1000.0);
        List<GQLBlockTimestampOnly> blocksPeriod = LastBlocks.Where(e => (timestampMsNow - Convert.ToDouble(e.timestamp)) < millisIn1Day).ToList();
        var hashrate = CalculateNetworkHashrate(blocksPeriod);

        return hashrate;
    }

    public static async Task<TimeSpan> GetTimespanUntillNextEpoch(ILocalStorageService localStorage)
    {
        var blocks = await GetLastBlocksForStatistics(localStorage, 32);

        if (blocks == null || blocks.Count == 0) return TimeSpan.MinValue;

        double Last16BlocksAverageBlockTimeMs = BlockTools.GetAverageBlockTime(blocks);
        int currentHeight = await GetCurrentHeight();
        var ErgoEpochLength = await GetErgoEpochLength();
        int blocksUntilNextEpoch = ErgoEpochLength - (currentHeight % ErgoEpochLength);
        double millisUntillNextEpoch = Last16BlocksAverageBlockTimeMs * blocksUntilNextEpoch;

        TimeSpan timespanUntilNextEpoch = TimeSpan.FromMilliseconds(millisUntillNextEpoch);

        //need to subtract the current time passed since last block
        TimeSpan timespanSinceLastBlock = await GetTimespanSinceLastBlock();
        timespanUntilNextEpoch = timespanUntilNextEpoch - timespanSinceLastBlock;

        return timespanUntilNextEpoch;
    }

    public static async Task<int?> ConvertBlockHeaderIdToHeight(string? headerId)
    {
        if (headerId == null) return null;
        GQLBlockDetail? block = await GraphQLInterface.GetBlock(headerId, null);
        if (block == null) return null;
        return block.height;
    }

    public static async Task<string?> ConvertBlockHeightToHeaderId(int? height)
    {
        if (height == null) return null;
        GQLBlockDetail? block = await GraphQLInterface.GetBlock(null, height);
        if (block == null) return null;
        return block.header.headerId;
    }

    public static async Task<List<HashrateInfo>> GetHashrateInfoStatistics(ILocalStorageService localStorage)
    {
        var blocks = await GetLastBlocksForStatistics(localStorage, Globals.MaxBlocksToStoreForStatsInLocalStorage);
        blocks = blocks.OrderBy(e => e.height).ToList();

        var ErgoEpochLength = await GetErgoEpochLength();
        int step = 8;

        List<HashrateInfo> data = new List<HashrateInfo>(blocks.Count / step);
        for (var i = ErgoEpochLength; i < blocks.Count; i += step)
        {
            var hashrate = CalculateNetworkHashrate(blocks.Skip(i - ErgoEpochLength).Take(ErgoEpochLength).ToList());
            var hashrateTHs = Math.Round(hashrate / 1000 / 1000 / 1000 / 1000,2);
            var difficulty = Convert.ToDouble(blocks[i].difficulty);
            var difficultyPH = Math.Round(difficulty / 1000 / 1000 / 1000 / 1000 / 1000,2);
            var dt = Tools.UnixTimeStampMsToDateTime(Convert.ToDouble(blocks[i].timestamp));

            data.Add(new HashrateInfo { height = blocks[i].height, difficulty = difficulty, difficultyPH = difficultyPH , hashrate = hashrate, hashrateTHs = hashrateTHs, timestamp = dt });
        }
        return data;
    }
}
