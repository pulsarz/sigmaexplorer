using SigmaExplorer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Globals
{
    public static string DonationAddress = "9fGat1BwWoW9kKuSZtpCtX8vs4Yk1Awbj9BQJG2oyojZfB92vpL";

    public static string IPFSGateway = "https://cloudflare-ipfs.com/ipfs";

    //public static string DefaultGraphQLEndpoint = "https://gql.ergoplatform.com";

    //public static string GraphQLEndpoint = "https://gql.ergoplatform.com";
    //public static string GraphQLEndpoint = "https://gql.mempoolnode.live";//testnet
    //public static string GraphQLEndpoint = "https://ergo-explorer.getblok.io/graphql/";
    //public static string GraphQLEndpoint = "https://explore.sigmaspace.io/api/graphql";
    public static string GraphQLEndpoint = "https://explorer-graphql.ergohost.io/";

    //public static string TestnetGraphQLEndpoint = "https://gql-testnet.ergoplatform.com";
    public static string TestnetGraphQLEndpoint = "https://tn-ergo-explorer.anetabtc.io/graphql";

    public static int GraphQLBatchSize = 50;
    public static int UIRefreshInterval = 1000 * 1;
    public static int UIRefreshIntervalMempool = 1000 * 30;

    
    public static int MaxBlocksToStoreForStatsInLocalStorage = 2048;
    public static List<GQLBlockTimestampOnly> LastBlocksForStats = new List<GQLBlockTimestampOnly>();

    //public static GQLBlockTimestampOnly? LastBlockTimestampOnly = null;
    public static DateTime LastNetworkStateUpdate = DateTime.MinValue;
    public static GQLStateWrapper? LastNetworkState = null;


    //Newly discovered itens get added into this list (resets on refresh though).
    //We already provide some often used ones here to save on api calls.
    public static Dictionary<string,GQLToken> tokenCache = new Dictionary<string,GQLToken> {
        {"1fd6e032e8476c4aa54c18c1a308dce83940e8f4a28f576440513ed7326ad489", new GQLToken { tokenId = "1fd6e032e8476c4aa54c18c1a308dce83940e8f4a28f576440513ed7326ad489", decimals = 4, name = "Paideia" } },
        {"00b1e236b60b95c2c6f8007a9d89bc460fc9e78f98b09faec9449007b40bccf3", new GQLToken { tokenId = "00b1e236b60b95c2c6f8007a9d89bc460fc9e78f98b09faec9449007b40bccf3", decimals = 4, name = "EGIO" } },
        {"d71693c49a84fbbecd4908c94813b46514b18b67a99952dc1e6e4791556de413", new GQLToken { tokenId = "d71693c49a84fbbecd4908c94813b46514b18b67a99952dc1e6e4791556de413", decimals = 2, name = "ergopad" } },
        {"00bd762484086cf560d3127eb53f0769d76244d9737636b2699d55c56cd470bf", new GQLToken { tokenId = "00bd762484086cf560d3127eb53f0769d76244d9737636b2699d55c56cd470bf", decimals = 4, name = "EPOS" } },
        {"007fd64d1ee54d78dd269c8930a38286caa28d3f29d27cadcb796418ab15c283", new GQLToken { tokenId = "007fd64d1ee54d78dd269c8930a38286caa28d3f29d27cadcb796418ab15c283", decimals = 4, name = "EXLE" } },
        {"02f31739e2e4937bb9afb552943753d1e3e9cdd1a5e5661949cb0cef93f907ea", new GQLToken { tokenId = "02f31739e2e4937bb9afb552943753d1e3e9cdd1a5e5661949cb0cef93f907ea", decimals = 4, name = "Terahertz" } },
        {"60a3b2e917fe6772d65c5d253eb6e4936f1a2174d62b3569ad193a2bf6989298", new GQLToken { tokenId = "60a3b2e917fe6772d65c5d253eb6e4936f1a2174d62b3569ad193a2bf6989298", decimals = 9, name = "GBGT GetBlok.io Governance Token" } },
        {"e8b20745ee9d18817305f32eb21015831a48f02d40980de6e849f886dca7f807", new GQLToken { tokenId = "e8b20745ee9d18817305f32eb21015831a48f02d40980de6e849f886dca7f807", decimals = 8, name = "Flux" } },
        {"0779ec04f2fae64e87418a1ad917639d4668f78484f45df962b0dec14a2591d2", new GQLToken { tokenId = "0779ec04f2fae64e87418a1ad917639d4668f78484f45df962b0dec14a2591d2", decimals = 0, name = "Mi Goreng" } },
        {"0cd8c9f416e5b1ca9f986a7f10a84191dfb85941619e49e53c0dc30ebf83324b", new GQLToken { tokenId = "0cd8c9f416e5b1ca9f986a7f10a84191dfb85941619e49e53c0dc30ebf83324b", decimals = 0, name = "COMET" } },
        {"30974274078845f263b4f21787e33cc99e9ec19a17ad85a5bc6da2cca91c5a2e", new GQLToken { tokenId = "30974274078845f263b4f21787e33cc99e9ec19a17ad85a5bc6da2cca91c5a2e", decimals = 8, name = "WT_ADA" } },
        {"ef802b475c06189fdbf844153cdc1d449a5ba87cce13d11bb47b5a539f27f12b", new GQLToken { tokenId = "ef802b475c06189fdbf844153cdc1d449a5ba87cce13d11bb47b5a539f27f12b", decimals = 9, name = "WT_ERG" } },
        {"03faf2cb329f2e90d6d23b58d91bbb6c046aa143261cc21f52fbe2824bfcbf04", new GQLToken { tokenId = "03faf2cb329f2e90d6d23b58d91bbb6c046aa143261cc21f52fbe2824bfcbf04", decimals = 2, name = "SigUSD" } },
        {"003bd19d0187117f130b62e1bcab0939929ff5c7709f843c5c4dd158949285d0", new GQLToken { tokenId = "003bd19d0187117f130b62e1bcab0939929ff5c7709f843c5c4dd158949285d0", decimals = 0, name = "SigRSV" } },
        {"7d672d1def471720ca5782fd6473e47e796d9ac0c138d9911346f118b2f6d9d9", new GQLToken { tokenId = "7d672d1def471720ca5782fd6473e47e796d9ac0c138d9911346f118b2f6d9d9", decimals = 0, name = "SUSD Bank V2 NFT" } },
        {"01e6498911823f4d36deaf49a964e883b2c4ae2a4530926f18b9c1411ab2a2c2", new GQLToken { tokenId = "01e6498911823f4d36deaf49a964e883b2c4ae2a4530926f18b9c1411ab2a2c2", decimals = 0, name = "ORACLE" } },
        {"8c27dd9d8a35aac1e3167d58858c0a8b4059b277da790552e37eba22df9b9035", new GQLToken { tokenId = "8c27dd9d8a35aac1e3167d58858c0a8b4059b277da790552e37eba22df9b9035", decimals = 0, name = "ERGUSD-PT" } },
        {"20fa2bf23962cdf51b07722d6237c0c7b8a44f78856c0f7ec308dc1ef1a92a51", new GQLToken { tokenId = "20fa2bf23962cdf51b07722d6237c0c7b8a44f78856c0f7ec308dc1ef1a92a51", decimals = 0, name = "Emission Contract NFT" } },
        {"d9a2cc8a09abfaed87afacfbb7daee79a6b26f10c6613fc13d3f3953e5521d1a", new GQLToken { tokenId = "d9a2cc8a09abfaed87afacfbb7daee79a6b26f10c6613fc13d3f3953e5521d1a", decimals = 0, name = "Reemission Token" } },
        {"f7cf16e6eed0d11ffd3f55186e00085748e78f487cb6e517b2f610e0045509fe", new GQLToken { tokenId = "f7cf16e6eed0d11ffd3f55186e00085748e78f487cb6e517b2f610e0045509fe", decimals = 0, name = "ERG_Ergopad_LP" } },
        {"e249780a22e14279357103749102d0a7033e0459d10b7f277356522ae9df779c", new GQLToken { tokenId = "e249780a22e14279357103749102d0a7033e0459d10b7f277356522ae9df779c", decimals = 0, name = "ERG_NETA_LP" } },
        {"e7021bda9872a7eb2aa69dd704e6a997dae9d1b40d1ff7a50e426ef78c6f6f87", new GQLToken { tokenId = "e7021bda9872a7eb2aa69dd704e6a997dae9d1b40d1ff7a50e426ef78c6f6f87 ", decimals = 0, name = "ERG_ErgoPOS_LP" } },
        {"0f037f9e869774b63ee1e8f0b90d83f0ed6d92e25c298854e4acd74db06cd250", new GQLToken { tokenId = "0f037f9e869774b63ee1e8f0b90d83f0ed6d92e25c298854e4acd74db06cd250", decimals = 0, name = "COMET WL" } },
        {"e91cbc48016eb390f8f872aa2962772863e2e840708517d1ab85e57451f91bed", new GQLToken { tokenId = "e91cbc48016eb390f8f872aa2962772863e2e840708517d1ab85e57451f91bed", decimals = 0, name = "Ergold" } },
        {"36aba4b4a97b65be491cf9f5ca57b5408b0da8d0194f30ec8330d1e8946161c1", new GQLToken { tokenId = "36aba4b4a97b65be491cf9f5ca57b5408b0da8d0194f30ec8330d1e8946161c1", decimals = 0, name = "Erdoge" } },
        {"fbbaac7337d051c10fc3da0ccb864f4d32d40027551e1c3ea3ce361f39b91e40", new GQLToken { tokenId = "fbbaac7337d051c10fc3da0ccb864f4d32d40027551e1c3ea3ce361f39b91e40", decimals = 0, name = "kushti" } }
    };

    public static Dictionary<string,MinerInfo> KnownMiners = new Dictionary<string,MinerInfo>
    {
        {"88dhgzEuTXaSuf5QC1TJDgdxqJMQEQAM6YaTTRqmUDrmPoVky1b16WAK5zMrq3p2mYqpUNKCyi5CLS9V", new MinerInfo { address = "88dhgzEuTXaSuf5QC1TJDgdxqJMQEQAM6YaTTRqmUDrmPoVky1b16WAK5zMrq3p2mYqpUNKCyi5CLS9V", name = "herominers" } },
        {"88dhgzEuTXaRQTX5KNdnaWTTX7fEZVEQRn6qP4MJotPuRnS3QpoJxYpSaXoU1y7SHp8ZXMp92TH22DBY", new MinerInfo { address = "88dhgzEuTXaRQTX5KNdnaWTTX7fEZVEQRn6qP4MJotPuRnS3QpoJxYpSaXoU1y7SHp8ZXMp92TH22DBY", name = "2miners" } },
        {"88dhgzEuTXaRiLRSYpvTCXWoN3A86gnWs3Z8BWkJGkGMXsR3WzUUyqbB47YAzhhsB6HJdJ4tC5AFYfSc", new MinerInfo { address = "88dhgzEuTXaRiLRSYpvTCXWoN3A86gnWs3Z8BWkJGkGMXsR3WzUUyqbB47YAzhhsB6HJdJ4tC5AFYfSc", name = "2miners solo" } },
        {"88dhgzEuTXaTj2AZkM2vwnemCYyAUJymaFf8iJPUYmgLkJqQmPd3DTubYS5UfL75MhQbEjmuhBMbdspA", new MinerInfo { address = "88dhgzEuTXaTj2AZkM2vwnemCYyAUJymaFf8iJPUYmgLkJqQmPd3DTubYS5UfL75MhQbEjmuhBMbdspA", name = "k1pool" } },
        {"88dhgzEuTXaSzQf8GiC73N6Y4FgB7e7hEPauEZnNjasg2wKHb87dJuemhoXTjo2XnZ327GCGJZuCQoNs", new MinerInfo { address = "88dhgzEuTXaSzQf8GiC73N6Y4FgB7e7hEPauEZnNjasg2wKHb87dJuemhoXTjo2XnZ327GCGJZuCQoNs", name = "flypool" } },
        {"88dhgzEuTXaQ2HPUskY3hvgMA5uCbQWwZNPbMC1Hem9zM2V9U7KMah7LYWS4Hm4WECGuc22nofdQbHbY", new MinerInfo { address = "88dhgzEuTXaQ2HPUskY3hvgMA5uCbQWwZNPbMC1Hem9zM2V9U7KMah7LYWS4Hm4WECGuc22nofdQbHbY", name = "woolypooly" } },
        {"88dhgzEuTXaUddjuoQ974fRGZtb6298oZ3oGBP32ZDYuTAtP3bEQgcYR2C1xKPGEhqsMVZBiFGiPf6QE", new MinerInfo { address = "88dhgzEuTXaUddjuoQ974fRGZtb6298oZ3oGBP32ZDYuTAtP3bEQgcYR2C1xKPGEhqsMVZBiFGiPf6QE", name = "ergominers" } },
        {"88dhgzEuTXaRp6WD5jWZSnXzBbA44g1xSMk6Xv2r6Cey8snSH78S6ZbWjP24yyPTDCCZByLpNXXe6NnN", new MinerInfo { address = "88dhgzEuTXaRp6WD5jWZSnXzBbA44g1xSMk6Xv2r6Cey8snSH78S6ZbWjP24yyPTDCCZByLpNXXe6NnN", name = "nanopool" } },
        {"88dhgzEuTXaTnTZomXPfuJ67oYJPbrv17yNkLjN6Nj8HxZEUf2iAdiv9gTqmnKKa2i75zmUtDnPQovBb", new MinerInfo { address = "88dhgzEuTXaTnTZomXPfuJ67oYJPbrv17yNkLjN6Nj8HxZEUf2iAdiv9gTqmnKKa2i75zmUtDnPQovBb", name = "kryptex" } },
        {"88dhgzEuTXaVfva5U9pvg84LryFq6umpt3ZpaUt63yDLcHydKsEHaXbebCbnKsprU5PW3G2GqX8ZdmUM", new MinerInfo { address = "88dhgzEuTXaVfva5U9pvg84LryFq6umpt3ZpaUt63yDLcHydKsEHaXbebCbnKsprU5PW3G2GqX8ZdmUM", name = "666pool" } },
        {"88dhgzEuTXaQ9HG2rDCmirsSGWEva3yZTXfjAB4ZBb3D1o3XH66qpndNUYqXzXiYnUUb2h9qZrzdZ6UA", new MinerInfo { address = "88dhgzEuTXaQ9HG2rDCmirsSGWEva3yZTXfjAB4ZBb3D1o3XH66qpndNUYqXzXiYnUUb2h9qZrzdZ6UA", name = "getblok" } }
    };
}
