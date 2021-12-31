using Autofac;
using BeeSharp.ApiComponents.ApiModels.JsonConverter.BroadcastOp;
using BeeSharp.ApiComponents.ApiModels.JsonConverter.BroadcastOp.CustomJson;
using BeeSharp.ApiComponents.ApiModels.JsonConverter.DirectorComponents;

namespace BeeSharpDefaultInitializer.ContainerConfig
{
    public static class DirectorManager
    {
        public static void RegisterDirectors(ILifetimeScope containerScope)
        {
            var customJsonOpIdDirector = containerScope.Resolve<CustomJsonOpIdDirector>();
            var customJsonOpListOpDirector = containerScope.Resolve<CustomJsonOpListOpDirector>();
            var broadcastOpDirector = containerScope.Resolve<BroadcastOpDirector>();
            
            // Register all directors
            DirectorRegistry.AddDirector(typeof(CustomJsonOpIdDirector), customJsonOpIdDirector);
            DirectorRegistry.AddDirector(typeof(CustomJsonOpListOpDirector), customJsonOpListOpDirector);
            DirectorRegistry.AddDirector(typeof(BroadcastOpDirector), broadcastOpDirector);
        }
    }
}