namespace BeeSharpDefaultInitializer
{
    public class CondenserFactoryOptionsModel
    {
        /// <summary>
        /// This parameter controls, how many nodes are tried on a network connection timeout, until giving up.
        /// </summary>
        public ushort MaxConnectionRetries { get; set; }

        /// <summary>
        /// This parameter controls, how long each nodes has to respond until a timeout occurs.
        /// Unlike NodeRankingTimeout, this timeout is used for regular web requests.
        /// </summary>
        public ushort WebRequestTimeout { get; set; }

        /// <summary>
        /// This parameter controls, how many nodes are tried to get a successful response (non-error-code) from a
        /// request, until giving up.
        /// </summary>
        public ushort MaxRequestRetries { get; set; }

        /// <summary>
        /// List of all trusted ApiNode urls.
        /// </summary>
        public string[] ApiNodeUrls { get; set; }

        /// <summary>
        /// How long has each node to respond until a timeout occurs.
        /// Unlike WebRequestTimeout, this timeout is specifically for NodeRanking requests.
        /// </summary>
        public ushort NodeRankingTimeout { get; set; }

        /// <summary>
        /// This parameter controls the length of the timeframe analyzed in Api Node selection.
        /// For more information, consult the project Wiki.
        /// </summary>
        public ushort SingleRequestMeasuringLimit { get; set; }

        /// <summary>
        /// This parameter controls the maximum latency allowed increase per single request in Api Node selection.
        /// For more information, consult the project Wiki
        /// </summary>
        public ushort SingleRequestMaxLatencyIncrease { get; set; }

        /// <summary>
        /// This parameter controls the prefix for private keys in Wif representation on the blockchain.
        /// </summary>
        public string WifPrefix { get; set; }

        /// <summary>
        /// This parameter controls the prefix for public keys in base58-encoded representation on the blockchain.
        /// </summary>
        public string ChainPrefix { get; set; }

        /// <summary>
        /// This parameter identifies the blockchain used. It has a length of 256 characters.
        /// </summary>
        public string ChainId { get; set; }

        /// <summary>
        /// This parameter controls which additional assemblies are included by BeeSharp. Used for extending BeeSharp.
        /// </summary>
        public string[] Assemblies { get; set; }

        /// <summary>
        /// This parameter controls the delay between different operations upon instance creation that connect to an
        /// Api Node.
        /// </summary>
        public ushort ContainerCreationConnectionDelay { get; set; }

        public CondenserFactoryOptionsModel(ushort maxConnectionRetries, ushort webRequestTimeout,
            ushort maxRequestRetries, string[] apiNodeUrls,
            ushort nodeRankingTimeout, ushort singleRequestMeasuringLimit, ushort singleRequestMaxLatencyIncrease,
            string wifPrefix, string chainPrefix, string chainId, string[] assemblies,
            ushort containerCreationConnectionDelay)
        {
            MaxConnectionRetries = maxConnectionRetries;
            WebRequestTimeout = webRequestTimeout;
            MaxRequestRetries = maxRequestRetries;
            ApiNodeUrls = apiNodeUrls;
            NodeRankingTimeout = nodeRankingTimeout;
            SingleRequestMeasuringLimit = singleRequestMeasuringLimit;
            SingleRequestMaxLatencyIncrease = singleRequestMaxLatencyIncrease;
            WifPrefix = wifPrefix;
            ChainPrefix = chainPrefix;
            ChainId = chainId;
            Assemblies = assemblies;
            ContainerCreationConnectionDelay = containerCreationConnectionDelay;
        }
    }
}