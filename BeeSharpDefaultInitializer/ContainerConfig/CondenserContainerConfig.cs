using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Autofac;
using BeeSharp.ApiCall;
using BeeSharp.ApiCall.ApiNodeRanking;
using BeeSharp.ApiCall.ApiNodeRanking.BenchmarkSingleApiNode.ThreadSafeSortedDictionary;
using BeeSharp.ApiCall.ApiUrlForSingleRequest;
using BeeSharp.ApiCall.ApiUrlForSingleRequest.MaximumAllowedLatency;
using BeeSharp.ApiCall.ApiWebCalls.WebClient;
using BeeSharp.ApiComponents.Condenser;
using BeeSharp.Auth.Signatures;
using BeeSharp.root;
using BeeSharp.root.HttpClient;

namespace BeeSharpDefaultInitializer.ContainerConfig
{
    public static class CondenserContainerConfig
    {
        public static IContainer Configure(ApiNodeRankingManagerContext apiNodeRankingManagerContext,
            MaxApiNodeLatencyContext maxApiNodeLatencyContext, SignatureCreationContext signatureCreationContext,
            ApiCallContext apiCallContext, BlockChainParametersContext blockChainParametersContext,
            CondenserCallContext condenserCallContext,
            string[] assemblies, ushort containerCreationConnectionDelay)
        {
            var builder = new ContainerBuilder();

            // To create the container, we need to register all components needed

            // Register all components with an according interface (Component with IComponent as IComponent)
            builder.RegisterAssemblyTypes(Assembly.Load(nameof(BeeSharp)))
                .Where(t => t.GetInterfaces().FirstOrDefault(i => i.Name == $"I{t.Name}") != null)
                .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == $"I{t.Name}")!)
                .SingleInstance();

            // Register all components that require additional context/information
            builder.RegisterType<SingleApiUrlHandler>().As<ISingleApiUrlHandler>()
                .WithParameter(new TypedParameter(typeof(MaxApiNodeLatencyContext), maxApiNodeLatencyContext))
                .SingleInstance();
            builder.RegisterType<ApiNodeRankingManager>().As<IApiNodeRankingManager>()
                .WithParameter(new TypedParameter(typeof(ApiNodeRankingManagerContext), apiNodeRankingManagerContext))
                .SingleInstance();
            builder.RegisterType<MaxApiNodeLatencyManager>().As<IMaxApiNodeLatencyManager>()
                .WithParameter(new TypedParameter(typeof(MaxApiNodeLatencyContext), maxApiNodeLatencyContext))
                .SingleInstance();
            builder.RegisterType<SignatureCreator>().As<ISignatureCreator>()
                .WithParameter(new TypedParameter(typeof(SignatureCreationContext), signatureCreationContext))
                .SingleInstance();

            // Register factories
            AdvancedConfigurationContainerConfig.RegisterAdvancedConfigurationComponents(builder,
                apiCallContext, blockChainParametersContext, condenserCallContext, containerCreationConnectionDelay);

            // Register dynamic factories (factories that require a type to use)
            var threadSafeExecutionType = typeof(ThreadSafeExecution);
            builder.RegisterType<ThreadSafeExecutionFactory>().As<IThreadSafeExecutionFactory>()
                .WithParameter(new TypedParameter(typeof(Type), threadSafeExecutionType))
                .SingleInstance();

            // Register Directors
            DirectorContainerConfig.RegisterDirectors(builder, assemblies);

            // Register additional types that are needed, but have no interface with similar name associated
            builder.RegisterType<ThreadSafeExecution>().SingleInstance();
            builder.RegisterType<HttpClientFactory>().As<IWebClientFactory>().SingleInstance();

            // Register generic types
            GenericsContainerConfig.RegisterGenerics(builder);

            return builder.Build();
        }
    }
}