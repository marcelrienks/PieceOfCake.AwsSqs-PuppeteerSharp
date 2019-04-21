using Microsoft.Extensions.Configuration;
using System.IO;

namespace Processor.Handlers
{
    public static class ConfigurationHandler
    {
        public static IConfigurationRoot Configuration;

        static ConfigurationHandler()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();

            Configuration = builder.Build();
        }
    }
}
