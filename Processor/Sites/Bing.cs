using System.Threading.Tasks;
using PuppeteerSharp;
using SharedModels.Models;
using Processor.Helper;

namespace Processor.Plugins
{
    class Bing : Plugin
    {
        private Page _page;

        public Bing(Page page, Query _transactionMessage) : base(page, _transactionMessage)
        {
            _page = page;
        }

        public override async Task Navigate()
        {
            await SendTextToElement("Search Field", By.Id("sb_form_q"), "gobbledy group");
            await ClickElement("Bing Search", By.Id("sb_form_go"));

            await SavePage();
        }
    }
}
