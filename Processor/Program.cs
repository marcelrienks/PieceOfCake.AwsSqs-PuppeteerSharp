using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using SharedModels.Models;
using Processor.Handlers;
using Processor.Helper;
using Processor.Plugins;
using System;
using System.Linq;
using System.Threading.Tasks;
using static SharedModels.Models.Enums;
using static Processor.Handlers.LoggingHandler;

namespace Processor
{
    class Program
    {
        private static QueueHandler _queueHandler;
        private static Query _query;
        private static Plugin _plugin;

        static void Main(string[] args)
        {
            Initialise().Wait();
        }

        private static async Task Initialise()
        {
            try
            {
                LogEvent(LoggingLevel.Info, $"Starting...");

                await BrowserPages.InitialiseBrowser();
#if PRELOAD || RELEASE
                await BrowserPages.PreLoadPages();
                while (true)
                {
                    await InitialiseQueueAsync();
                    InitialisePlugin();

                    await _plugin.Execute();
                }
#else
                await BrowserPages.LoadCurrentPage(Sites.Google);
                await _plugin.Execute();
#endif
            }
            catch (Exception ex)
            {
                LogEvent(LoggingLevel.Exception, $"Global exception was caught.", ex);
                _plugin?.DisposePlugin();
            }

            Console.ReadLine();
        }

        private async static Task InitialiseQueueAsync()
        {
            LogEvent(LoggingLevel.Info, $"Initialising Queue.");

            var awsOptions = ConfigurationHandler.Configuration.GetAWSOptions();
            var sqsClient = awsOptions.CreateServiceClient<IAmazonSQS>();
            _queueHandler = new QueueHandler(sqsClient);

            _query = await _queueHandler.SubscribeToQueue();
        }

        private static void InitialisePlugin()
        {
            LogEvent(LoggingLevel.Info, $"Initialising {_query.Site} site.");

            // Get page, and load Plugin
            var page = BrowserPages.Pages.Single(tab => tab.Key == _query.Site).Value;

            // Initialise Plugin
            switch (_query.Site)
            {
                case Sites.Bing:
                    _plugin = new Bing(page, _query);
                    break;

                case Sites.Google:
                    _plugin = new Google(page, _query);
                    break;
            }
        }
    }
}
