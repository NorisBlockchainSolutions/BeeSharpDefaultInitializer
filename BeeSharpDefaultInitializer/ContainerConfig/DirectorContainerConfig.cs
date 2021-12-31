using Autofac;
using BeeSharp.ApiComponents.ApiModels.JsonConverter.BroadcastOp;
using BeeSharp.ApiComponents.ApiModels.JsonConverter.BroadcastOp.CustomJson;

namespace BeeSharpDefaultInitializer.ContainerConfig
{
    public static class DirectorContainerConfig
    {
        public static void RegisterDirectors(ContainerBuilder builder, string[] assemblies)
        {
            builder.RegisterType<CustomJsonOpIdDirector>()
                .WithParameter(new TypedParameter(typeof(string[]), assemblies))
                .SingleInstance();
            builder.RegisterType<CustomJsonOpListOpDirector>()
                .WithParameter(new TypedParameter(typeof(string[]), assemblies))
                .SingleInstance();
            builder.RegisterType<BroadcastOpDirector>()
                .WithParameter(new TypedParameter(typeof(string[]), assemblies))
                .SingleInstance();
        }
    }
}