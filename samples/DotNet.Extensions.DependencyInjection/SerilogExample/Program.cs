using DotNet.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SerilogExample
{
    public class Program
    {
        public static void Main()
        {
            // Overwrite any existing changes with the debug version
            File.Copy("appsettings.debug.json", "appsettings.json", true);

            // Load the configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var services = new ServiceCollection();
            services.AddMutable<ILogger, Logger>(
                () => new LoggerConfiguration()
                    .ReadFrom
                    .Configuration(config)
                    .CreateLogger(),
                _ => config.GetReloadToken());

            var provider = services.BuildServiceProvider();

            // Create our foo instance
            var foo = ActivatorUtilities.CreateInstance<FooClass>(provider);

            // Write out a couple things
            foo.Debug("Debug level");
            foo.Verbose("Verbose level, normally squelched");

            // Override the appsettings with a verbose version
            File.Copy("appsettings.verbose.json", "appsettings.json", true);

            // IConfiguration's file watcher isn't super fast - so let's just force it to reload
            config.Reload();

            foo.Debug("Debug level");
            foo.Verbose("Verbose level, normally squelched");
        }
    }
}
