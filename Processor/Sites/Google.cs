using System.Threading.Tasks;
using PuppeteerSharp;
using SharedModels.Models;
using Processor.Helper;

namespace Processor.Plugins
{
    class Google : Plugin
    {
        private Page _page;

        public Google(Page page, Query _transactionMessage) : base(page, _transactionMessage)
        {
            _page = page;
        }

        public override async Task Navigate()
        {
            await SendTextToElement("Search Field", By.Name("q"), "gobbledy group");
            await ClickElement("Google Search", By.Type("#tsf > div:nth-child(2) > div > div.FPdoLc.VlcLAe > center > input[type=\"submit\"]:nth-child(1)"));

            await SavePage();
        }
    }
}
