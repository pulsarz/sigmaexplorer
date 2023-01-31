using Blazored.LocalStorage;
using SigmaExplorer.Shared;

public static class DifficultyTools
{
    private static async Task<double> CalculateBitcoinDifficulty(GQLBlockTimestampOnly start, GQLBlockTimestampOnly end)
    {
        var ErgoEpochLength = await BlockTools.GetErgoEpochLength();
        var ErgoDesiredBlockTimeMs = await BlockTools.GetErgoDesiredBlockTimeMs();
        var diff = (Convert.ToDouble(end.difficulty) * ErgoDesiredBlockTimeMs * ErgoEpochLength / ((Convert.ToDouble(end.timestamp) - Convert.ToDouble(start.timestamp))));
        return diff;
    }
    private static async Task<double> InterpolateErgoDiff(Dictionary<double, double> data)
    {
        var ErgoEpochLength = await BlockTools.GetErgoEpochLength();

        var size = data.Count();
        var xy = data.Select(e => e.Key*e.Value);
        var x = data.Select(e => e.Key);
        var x2 = data.Select(e => e.Key* e.Key);
        var y = data.Select(e => e.Value);
        var y2 = data.Select(e => e.Value * e.Value);

        var xySum = xy.Aggregate(0D, (partialSum, a) => partialSum + a);
        var x2Sum = x2.Aggregate(0D, (partialSum, a) => partialSum + a);
        var ySum = y.Aggregate(0D, (partialSum, a) => partialSum + a);
        var xSum = x.Aggregate(0D, (partialSum, a) => partialSum + a);

        var b = (xySum * size - xSum * ySum) / (x2Sum * size - xSum * xSum);
        var a = (ySum - b * xSum) / size;

        var point = data.Last().Key + ErgoEpochLength;
        return (a + b * point);
    }

    private static async Task<double> CalculateErgoDiff(List<GQLBlockTimestampOnly> blocks)
    {
        Dictionary<double, double> data = new Dictionary<double, double>();

        var currentDifficultyStr = await BlockTools.GetCurrentDifficulty();
        var currentDifficulty = Convert.ToDouble(currentDifficultyStr);
        var ErgoEpochLength = await BlockTools.GetErgoEpochLength();
        var ErgoDesiredBlockTimeMs = await BlockTools.GetErgoDesiredBlockTimeMs();

        for (var i = 1; i < blocks.Count; i++)
        {
            var diff = (Convert.ToDouble(blocks[i].difficulty) * ErgoDesiredBlockTimeMs * ErgoEpochLength) / ((Convert.ToDouble(blocks[i].timestamp) - Convert.ToDouble(blocks[i-1].timestamp)));
            data.Add(blocks[i].height,diff);
        }

        var rawPredictedDiff = await InterpolateErgoDiff(data);
        return Math.Max(Math.Min(rawPredictedDiff, currentDifficulty * 1.5), currentDifficulty * 0.5);
    }

    public static async Task<double> CalculateNextDifficulty(ILocalStorageService localStorage)
    {
        //Inspired by https://github.com/Luivatra/ergo_diff/blob/5878f25bec395242a5456d76d3d7026e58d30567/scripts/ergoDiffTools.js#L204
        int height = await BlockTools.GetCurrentHeight();
        var currentDifficultyStr = await BlockTools.GetCurrentDifficulty();
        var currentDifficulty = Convert.ToDouble(currentDifficultyStr);
        var ErgoEpochLength = await BlockTools.GetErgoEpochLength();

        List<GQLBlockTimestampOnly> Blocks = await BlockTools.GetLastBlocksForStatistics(localStorage, ErgoEpochLength*8);//1024

        var blocksThisEpoch = height % ErgoEpochLength;
        var remainingBlocksThisEpoch = ErgoEpochLength - blocksThisEpoch;     // diff adjusts every epoch
        var nextEpochStart = height + remainingBlocksThisEpoch;
        var start = ErgoEpochLength - (nextEpochStart - height);


        var epoch8 = Blocks.Skip(0).Take(1).First();
        var epoch7 = Blocks.Skip(start).Take(1).First();
        var epoch6 = Blocks.Skip(start+ ErgoEpochLength).Take(1).First();
        var epoch5 = Blocks.Skip(start + ErgoEpochLength*2).Take(1).First();
        var epoch4 = Blocks.Skip(start + ErgoEpochLength*3).Take(1).First();
        var epoch3 = Blocks.Skip(start + ErgoEpochLength*4).Take(1).First();
        var epoch2 = Blocks.Skip(start + ErgoEpochLength*5).Take(1).First();
        var epoch1 = Blocks.Skip(start + ErgoEpochLength*6).Take(1).First();
        var epoch0 = Blocks.Skip(start + ErgoEpochLength*7).Take(1).First();

        var headers = new List<GQLBlockTimestampOnly>();
        headers.Add(epoch0);
        headers.Add(epoch1);
        headers.Add(epoch2);
        headers.Add(epoch3);
        headers.Add(epoch4);
        headers.Add(epoch5);
        headers.Add(epoch6);
        headers.Add(epoch7);

        var newX = epoch8.Clone();
        newX.height += remainingBlocksThisEpoch;
        newX.timestamp = Convert.ToString(Convert.ToUInt64(epoch7.timestamp) + Math.Floor((Convert.ToDouble(epoch8.timestamp) - Convert.ToDouble(epoch7.timestamp)) / (ErgoEpochLength - remainingBlocksThisEpoch) * ErgoEpochLength));
        headers.Add(newX);


        var diff = await CalculateErgoDiff(headers);
        var bitcoinDiff = await CalculateBitcoinDifficulty(epoch7, newX);

        diff = (diff+bitcoinDiff)/2;
        diff = Math.Max(Math.Min(diff, currentDifficulty * 1.5), currentDifficulty * 0.5);

        return diff;
    }
}
