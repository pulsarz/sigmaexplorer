using SigmaExplorer.Shared;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

public class OracleV1Data
{
    public string title { get; set; }
    public int current_block_height { get; set; }
    public double latest_price { get; set; }
    public int posting_schedule_minutes { get; set; }
    public int epoch_ends_in_minutes { get; set; }
    public string current_pool_stage { get; set; }
    public long pool_funded_percentage { get; set; }
    public long minimum_pool_box_value { get; set; }
    public int posting_schedule_blocks { get; set; }
    public int number_of_oracles { get; set; }
    public string current_epoch_id { get; set; }
    public long latest_datapoint { get; set; }
    public string live_epoch_address { get; set; }
    public long oracle_payout_price { get; set; }
    public int live_epoch_length { get; set; }
    public int epoch_prep_length { get; set; }
    public int deviation_range { get; set; }
    public int consensus_num { get; set; }
    public string oracle_pool_nft_id { get; set; }
    public string oracle_pool_participant_token_id { get; set; }
    public int epoch_end_height { get; set; }
}

public static class OracleTools
{
    private static readonly HttpClient _httpClient = new HttpClient();
    public static async Task<OracleV1Data> GetOracleV1Info(string url)
    {
        OracleV1Data data = null;

        var result = await _httpClient.GetFromJsonAsync<string>(url);
        if (result != null && result != "")
        {
            data = JsonSerializer.Deserialize<OracleV1Data>(result);
         }
        return data;
    }

}
