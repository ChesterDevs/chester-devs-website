using System.Linq;
using System.Reflection;
using Autofac;
using LazyCache;
using Module = Autofac.Module;

namespace ChesterDevs.Core.Aspnet
{
    public class AutofacModule : Module
    {
        private static readonly string[] AssembliesNamesToScan =
        {
            "ChesterDevs.Core.Aspnet"
        };

        protected override void Load(ContainerBuilder builder)
        {
            ScanAssemblies(builder);
            RegisterOddBalls(builder);
        }

        private void RegisterOddBalls(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<CachingService>().As<IAppCache>().SingleInstance();
        }

        private void ScanAssemblies(ContainerBuilder containerBuilder)
        {
            var assembliesToScan = AssembliesNamesToScan
                .Select(Assembly.Load)
                .ToArray();

            containerBuilder
                .RegisterAssemblyTypes(assembliesToScan)
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}