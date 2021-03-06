#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using BeeSharp.ApiCall;
using BeeSharp.ApiCall.ApiNodeRanking;
using BeeSharp.ApiCall.ApiUrlForSingleRequest.MaximumAllowedLatency;
using BeeSharp.ApiComponents.Condenser;
using BeeSharp.Auth.ECKeyManagement.KeyProcessing;
using BeeSharp.Auth.Signatures;
using BeeSharpDefaultInitializer.ContainerConfig;

namespace BeeSharpDefaultInitializer
{
    public class CondenserAdapter : ICondenserAdapter
    {
        private readonly ILifetimeScope _containerScope;

        private CondenserAdapter(ushort maxConnectionRetries, ushort webRequestTimeout,
            ushort maxRequestRetries, string[] apiNodeUrls, ushort nodeRankingTimeout,
            ushort singleRequestMeasuringLimit, ushort singleRequestMaxLatencyIncrease,
            string wifPrefix, string chainPrefix, string chainId, string[] assemblies,
            ushort containerCreationConnectionDelay)
        {
            var apiCallContext = new ApiCallContext(maxConnectionRetries, webRequestTimeout);
            var condenserCallContext = new CondenserCallContext(maxRequestRetries);
            var nodeRankingManagerContext = new ApiNodeRankingManagerContext(apiNodeUrls, nodeRankingTimeout);
            var maxApiNodeLatencyContext = new MaxApiNodeLatencyContext(singleRequestMeasuringLimit,
                singleRequestMaxLatencyIncrease);
            var localBlockChainParametersContext = new BlockChainParametersContext(wifPrefix, chainPrefix);
            var signatureCreationContext = new SignatureCreationContext(chainId);
            var assembliesAll = new List<string> { nameof(BeeSharp) };
            assembliesAll.AddRange(assemblies);
            
            var container = CondenserContainerConfig.Configure(nodeRankingManagerContext,
                maxApiNodeLatencyContext, signatureCreationContext, apiCallContext, localBlockChainParametersContext,
                condenserCallContext, assembliesAll.ToArray(), containerCreationConnectionDelay);
            
            _containerScope = container.BeginLifetimeScope();
            DirectorManager.RegisterDirectors(_containerScope);
        }

        public CondenserAdapter(CondenserFactoryOptionsModel factoryOptions) : this(
            factoryOptions.MaxConnectionRetries,
            factoryOptions.WebRequestTimeout,
            factoryOptions.MaxRequestRetries,
            factoryOptions.ApiNodeUrls, factoryOptions.NodeRankingTimeout,
            factoryOptions.SingleRequestMeasuringLimit, factoryOptions.SingleRequestMaxLatencyIncrease,
            factoryOptions.WifPrefix, factoryOptions.ChainPrefix, factoryOptions.ChainId,
            factoryOptions.Assemblies,
            factoryOptions.ContainerCreationConnectionDelay
        ) {}
        
        /// <summary>
        /// Get an instance of condenserCall to read blockchain data from the condenser api.
        /// For writing to the blockchain, use broadcastTransactionAsync instead.
        /// </summary>
        /// <returns>The condenser instance.</returns>
        public async Task<ICondenserCall> GetCondenserAsync()
        {
            return await _containerScope.Resolve<Task<ICondenserCall>>();
        }
        
        /// <summary>
        /// Get an instance of broadcast transaction to broadcast transactions to the blockchain.
        /// For reading data from the blockchain, use GetCondenserAsync instead.
        /// </summary>
        /// <returns>The instance.</returns>
        public async Task<IBroadcastTransaction> GetBroadcastTransactionAsync()
        {
            return await _containerScope.Resolve<Task<IBroadcastTransaction>>();
        }

        /// <summary>
        /// Get an instance of private key factory. This factory is used to create private key objects.
        /// </summary>
        /// <returns>The private key factory.</returns>
        public async Task<IEcdsaPrivateKeyFactory> GetPrivateKeyFactoryAsync()
        {
            return await _containerScope.Resolve<Task<IEcdsaPrivateKeyFactory>>();
        }

        /// <summary>
        /// Create an instance of public key factory. This factory is used to create public key objects.
        /// </summary>
        /// <returns>The factory.</returns>
        public async Task<IEcdsaPublicKeyFactory> GetPublicKeyFactoryAsync()
        {
            return await _containerScope.Resolve<Task<IEcdsaPublicKeyFactory>>();
        }
        
        /// <summary>
        /// Remove an api node url from the current node ranking.
        /// Useful, if an api node malfunctions.
        /// </summary>
        /// <param name="apiNodeUrl">The url of the apiNode to remove.</param>
        public async Task RemoveApiNodeUrlFromCurrentRanking(string apiNodeUrl)
        {
            var apiNodeRankingManager = _containerScope.Resolve<IApiNodeRankingManager>();

            await apiNodeRankingManager.RemoveUrlFromCurrentRankingAsync(apiNodeUrl);
        }

        /// <summary>
        /// Update the current api node ranking by re-ranking all known nodes.
        /// </summary>
        public async Task UpdateApiNodeRankingAsync()
        {
            var apiNodeRankingManager = _containerScope.Resolve<IApiNodeRankingManager>();

            await apiNodeRankingManager.UpdateApiNodeRankingAsync();
        }

        /// <summary>
        /// Update the current api node ranking by re-initializing the whole ranking.
        /// </summary>
        public async Task RunInitialApiNodeRankingAsync()
        {
            var apiNodeRankingManager = _containerScope.Resolve<IApiNodeRankingManager>();

            await apiNodeRankingManager.RunInitialApiNodeRankingAsync();
        }

        /// <summary>
        /// Resolve a component from the container scope.
        /// </summary>
        /// <returns></returns>
        public T ResolveContainerComponent<T>() where T : notnull
        {
            return _containerScope.Resolve<T>();
        }
    }
}