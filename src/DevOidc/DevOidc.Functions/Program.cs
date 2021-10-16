using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DevOidc.Functions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Debugger.Launch();

            Startup? startup = null;

            var host = new HostBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddCommandLine(args);
                    config.AddEnvironmentVariables();
                })
                .ConfigureFunctionsWorkerDefaults((context, builder) =>
                {
                    startup ??= new Startup(context.Configuration);
                    startup.ConfigureWorker(builder);
                })
                .ConfigureServices((context, services) =>
                {
                    startup ??= new Startup(context.Configuration);
                    startup.ConfigureServices(services);
                })
                .Build();

            await host.RunAsync();
        }
    }
}
