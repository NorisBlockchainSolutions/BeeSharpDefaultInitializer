using System.Threading.Tasks;
using BeeSharp.ApiComponents.Condenser;
using BeeSharp.Auth.ECKeyManagement.KeyProcessing;

namespace BeeSharpDefaultInitializer
{
    public interface ICondenserAdapter
    {
        /// <summary>
        /// Get an instance of condenserCall to read blockchain data from the condenser api.
        /// For writing to the blockchain, use broadcastTransactionAsync instead.
        /// </summary>
        /// <returns>The condenser instance.</returns>
        Task<ICondenserCall> GetCondenserAsync();

        /// <summary>
        /// Get an instance of broadcast transaction to broadcast transactions to the blockchain.
        /// For reading data from the blockchain, use GetCondenserAsync instead.
        /// </summary>
        /// <returns>The instance.</returns>
        Task<IBroadcastTransaction> GetBroadcastTransactionAsync();

        /// <summary>
        /// Get an instance of private key factory. This factory is used to create private key objects.
        /// </summary>
        /// <returns>The private key factory.</returns>
        Task<IEcdsaPrivateKeyFactory> GetPrivateKeyFactoryAsync();

        /// <summary>
        /// Create an instance of public key factory. This factory is used to create public key objects.
        /// </summary>
        /// <returns>The factory.</returns>
        Task<IEcdsaPublicKeyFactory> GetPublicKeyFactoryAsync();

        /// <summary>
        /// Remove an api node url from the current node ranking.
        /// Useful, if an api node malfunctions.
        /// </summary>
        /// <param name="apiNodeUrl">The url of the apiNode to remove.</param>
        public Task RemoveApiNodeUrlFromCurrentRanking(string apiNodeUrl);

        /// <summary>
        /// Update the current api node ranking by re-ranking all known nodes.
        /// </summary>
        public Task UpdateApiNodeRankingAsync();

        /// <summary>
        /// Update the current api node ranking by re-initializing the whole ranking.
        /// </summary>
        public Task RunInitialApiNodeRankingAsync();

        /// <summary>
        /// Resolve a component from the container scope.
        /// </summary>
        /// <returns></returns>
        public T ResolveContainerComponent<T>() where T : notnull;
    }
}