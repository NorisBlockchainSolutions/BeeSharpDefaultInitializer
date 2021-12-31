using System.Threading.Tasks;
using Autofac;
using BeeSharp.ApiCall;
using BeeSharp.ApiComponents.ApiModels.CondenserApi.get_chain_properties;
using BeeSharp.ApiComponents.Condenser;
using BeeSharp.ApiComponents.Condenser.Serialization;
using BeeSharp.Auth;
using BeeSharp.Auth.ECKeyManagement.KeyProcessing;
using BeeSharp.Auth.Provider;
using BeeSharp.Auth.Signatures;


namespace BeeSharpDefaultInitializer.ContainerConfig
{
    public static class AdvancedConfigurationContainerConfig
    {
        private static void RegisterApiCallManager(ContainerBuilder builder, ApiCallContext apiCallContext,
            ushort containerCreationConnectionDelay)
        {
            builder.RegisterType<ApiCallManager>()
                .As<IApiCallManager>()
                .WithParameter(new TypedParameter(typeof(ApiCallContext), apiCallContext))
                .SingleInstance();
            // Since ApiCallManager needs to be initialized for further container builds, it needs an
            // asynchronous registration (Task<IApiCallManager>)
            builder.Register(async context =>
                {
                    var instance = context!.Resolve<IApiCallManager>();
                    await instance.InitializeAsync();
                    await Task.Delay(containerCreationConnectionDelay);
                    return instance;
                })
                .SingleInstance();
            // This also requires all components depending on ApiCallManager to create their own asynchronous
            // registration
        }

        public static void RegisterAdvancedConfigurationComponents(ContainerBuilder builder,
            ApiCallContext apiCallContext, BlockChainParametersContext blockChainParametersContext,
            CondenserCallContext condenserCallContext, ushort containerCreationConnectionDelay)
        {
            RegisterApiCallManager(builder, apiCallContext, containerCreationConnectionDelay);

            // Register Task<ICondenserCall>
            builder.Register(async context =>
                {
                    var componentContext = context!.Resolve<IComponentContext>();
                    var apiCallManager = await componentContext.Resolve<Task<IApiCallManager>>();
                    var condenserResponseVerifier = componentContext.Resolve<ICondenserResponseVerifier>();
                    var result = new CondenserCall(apiCallManager, condenserResponseVerifier,
                        blockChainParametersContext,
                        condenserCallContext);
                    return (ICondenserCall)result;
                })
                .SingleInstance();

            // Register Task<IRefBlockParameterProvider>
            builder.Register(async context =>
                {
                    var condenser = await context!.Resolve<Task<ICondenserCall>>();
                    var result = new RefBlockParameterProvider(condenser);
                    await Task.Delay(containerCreationConnectionDelay);
                    return (IRefBlockParameterProvider)result;
                })
                .SingleInstance();

            // Register Task<ITransactionBuilder>
            builder.Register(async context =>
                {
                    var componentContext = context!.Resolve<IComponentContext>();
                    var signatureCreator = componentContext.Resolve<ISignatureCreator>();
                    var transactionSerializer = componentContext.Resolve<ITransactionSerializer>();
                    var refBlockParameterProvider = await componentContext.Resolve<Task<IRefBlockParameterProvider>>();
                    var result = new TransactionBuilder(signatureCreator, transactionSerializer,
                        refBlockParameterProvider);
                    return (ITransactionBuilder)result;
                })
                .SingleInstance();

            // Register Task<BroadcastTransaction>
            builder.Register(async context =>
                {
                    var componentContext = context!.Resolve<IComponentContext>();
                    var transactionBuilder = await componentContext.Resolve<Task<ITransactionBuilder>>();
                    var condenser = await componentContext.Resolve<Task<ICondenserCall>>();
                    var result = new BroadcastTransaction(condenser, transactionBuilder);
                    return (IBroadcastTransaction)result;
                })
                .SingleInstance();

            // Register Task<ChainParameterModel>
            builder.Register(async context =>
                {
                    var condenserCall = await context!.Resolve<Task<ICondenserCall>>();
                    var chainInfo = await condenserCall.GetCondenserApiCallResultAsync(
                        new CondenserApiGetChainProperties())!;

                    var chainParameters = ChainParameterProvider.RegisterNewIfUnknown(
                        chainInfo.AccountCreationFee, blockChainParametersContext);
                    await Task.Delay(containerCreationConnectionDelay);
                    return chainParameters;
                })
                .SingleInstance();

            // Register Task<IPublicKeyTransformer>
            builder.Register(async context =>
                {
                    var componentContext = context!.Resolve<IComponentContext>();
                    var chainParameterModel = await componentContext.Resolve<Task<ChainParameterModel>>();
                    var domainParametersProvider = componentContext.Resolve<IDomainParametersProvider>();
                    var result = new PublicKeyTransformer(domainParametersProvider, chainParameterModel);
                    return (IPublicKeyTransformer)result;
                })
                .SingleInstance();

            // Register Task<IPrivateKeyTransformer>
            builder.Register(async context =>
                {
                    var componentContext = context!.Resolve<IComponentContext>();
                    var chainParameterModel = await componentContext.Resolve<Task<ChainParameterModel>>();
                    var domainParametersProvider = componentContext.Resolve<IDomainParametersProvider>();
                    var result = new PrivateKeyTransformer(domainParametersProvider, chainParameterModel);
                    return (IPrivateKeyTransformer)result;
                })
                .SingleInstance();

            // Register Task<IEcdsaPrivateKeyFactory>
            builder.Register(async context =>
                {
                    var componentContext = context!.Resolve<IComponentContext>();
                    var privateKeyTransformer = await componentContext.Resolve<Task<IPrivateKeyTransformer>>();
                    var publicKeyTransformer = await componentContext.Resolve<Task<IPublicKeyTransformer>>();
                    var ecKeyCreator = componentContext.Resolve<IEcKeyCreator>();
                    var result = new EcdsaPrivateKeyFactory(privateKeyTransformer, publicKeyTransformer, ecKeyCreator);
                    return (IEcdsaPrivateKeyFactory)result;
                })
                .SingleInstance();

            // Register Task<IEcdsaPublicKeyFactory>
            builder.Register(async context =>
                {
                    var componentContext = context!.Resolve<IComponentContext>();
                    var publicKeyTransformer = await componentContext.Resolve<Task<IPublicKeyTransformer>>();
                    var result = new EcdsaPublicKeyFactory(publicKeyTransformer);
                    return (IEcdsaPublicKeyFactory)result;
                })
                .SingleInstance();
        }
    }
}