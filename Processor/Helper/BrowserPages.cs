using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;
using Processor.Handlers;
using static SharedModels.Models.Enums;
using static Processor.Handlers.LoggingHandler;

namespace Processor.Helper
{
    public static class BrowserPages
    {
        public static Browser Browser;
        public static Dictionary<Sites, Page> Pages;

        static BrowserPages()
        {
            LogEvent(LoggingLevel.Info, $"Initialising Browser.");
            Pages = new Dictionary<Sites, Page>();
        }

        public static async Task InitialiseBrowser()
        {
            // Download brower if needed, and instantiate
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                DefaultViewport = new ViewPortOptions() { Width = 1920, Height = 1080 }
            });
        }

        public static async Task LoadCurrentPage(Sites site)
        {
            LogEvent(LoggingLevel.Info, $"Loading first default page.");
            var page = (await Browser.PagesAsync()).FirstOrDefault();
            await page.GoToAsync(ConfigurationHandler.Configuration.GetSection("Sites")[$"{site}Url"]);
            await page.SetViewportAsync(new ViewPortOptions() { Width = 1920, Height = 1080 });
            Pages.Add(site, page);
        }

        public static async Task CreateAndLoadNewPage(Sites site)
        {
            LogEvent(LoggingLevel.Info, $"Create And Load New Page.");
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(ConfigurationHandler.Configuration.GetSection("Sites")[$"{site}Url"]);
            await page.SetViewportAsync(new ViewPortOptions() { Width = 1920, Height = 1080 });
            Pages.Add(site, page);
        }

        public static async Task PreLoadPages()
        {
            LogEvent(LoggingLevel.Info, $"PreLoad Pages.");
            var isFirstRun = true;

            // Loop through all banks available
            foreach (Sites bank in Enum.GetValues(typeof(Sites)))
            {
                // If this is the first itteration, get a handle on the default page
                if (isFirstRun)
                {
                    await LoadCurrentPage(bank);
                    isFirstRun = false;
                }

                // else open a new tab for the subsequint banks
                else
                    await CreateAndLoadNewPage(bank);
            }
        }

        public static Task ResetPage(Sites bank)
        {
            LogEvent(LoggingLevel.Info, $"Resetting Bank Page.");
            Pages.Remove(bank);
            return CreateAndLoadNewPage(bank);
        }
    }
}
