using Autofac;
using BeeSharp.ApiCall.ApiNodeRanking.BenchmarkSingleApiNode.ThreadSafeSortedDictionary;
using BeeSharp.ApiCall.ApiNodeRanking.BenchmarkSingleApiNode.ThreadSafeSortedDictionary.CollectionManagement;
using BeeSharp.ApiCall.ApiNodeRanking.RankingCreators;
using BeeSharp.root;

namespace BeeSharpDefaultInitializer.ContainerConfig
{
    public static class GenericsContainerConfig
    {
        public static void RegisterGenerics(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(SortedIDictionaryFactory<,>))
                .As(typeof(ISortedIDictionaryFactory<,>))
                .SingleInstance();
            builder.RegisterGeneric(typeof(CollisionFreeEntryAdder<>))
                .As(typeof(ICollisionFreeEntryAdder<>))
                .SingleInstance();
            builder.RegisterGeneric(typeof(ConflictingKeyAdder<>))
                .As(typeof(IConflictingKeyAdder<>))
                .SingleInstance();
            builder.RegisterGeneric(typeof(ListFactory<>))
                .As(typeof(IListFactory<>))
                .SingleInstance();
        }
    }
}