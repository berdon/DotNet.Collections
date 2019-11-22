using DotNet.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SimpleUsage
{
    public class Program
    {
        public static void Main()
        {
            var services = new ServiceCollection();

            var myServiceInstance = new MyService();
            services.AddMutable(myServiceInstance);
            var serviceProvider = services.BuildServiceProvider();

            var injectedClass = ActivatorUtilities.CreateInstance<InjectedClass>(serviceProvider);

            injectedClass.DisplayMessage();

            var serviceMutator = serviceProvider.GetRequiredService<IServiceMutator<MyService>>();
            serviceMutator.Update(instance => instance.Message = "Some different message");

            injectedClass.DisplayMessage();
        }
    }
}
