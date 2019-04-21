using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp;
using SharedModels.Models;
using Processor.Helper;
using static SharedModels.Models.Enums;
using static Processor.Handlers.LoggingHandler;

namespace Processor.Plugins
{
    abstract class Plugin
    {
        private readonly Page _page;
        readonly Query _transactionMessage;

        abstract public Task Navigate();

        public Plugin(Page page, Query transactionMessage)
        {
            _page = page;
            _transactionMessage = transactionMessage;
        }

        public virtual async Task Execute()
        {
            await Navigate();
            await DisposePlugin();
        }

        public async Task<bool> DoesTextExist(string text)
        {
            try
            {
                await _page.WaitForXPathAsync($"//*[contains(text(),'{text}')]", new WaitForSelectorOptions { Visible = true, Timeout = 20 });
                return true;
            }
            catch (Exception ex) when (ex.GetType() == typeof(WaitTaskTimeoutException))
            {
                return false;
            }
        }

        public async Task SendTextToElement(string elementDescription, By by, string text)
        {
            await _page.TypeAsync(by.Selector, text);
            LogEvent(LoggingLevel.Debug, $"Text '{text}' sent to elements '{elementDescription}'.");
        }

        public async Task ClickElement(string elementDescription, By by)
        {
            await _page.ClickAsync(by.Selector);
            LogEvent(LoggingLevel.Debug, $"Element '{elementDescription}' clicked.");
        }

        public bool WaitForResult(Func<bool> successCheck, Func<bool> failedCheck, string logMessage = "")
        {
            var timestamp = DateTime.Now;

            while (!successCheck())
            {
                if (failedCheck != null && failedCheck())
                    return false;

                LogEvent(LoggingLevel.Info, $"...Waiting for result on '{logMessage}'");

                if ((DateTime.Now - timestamp).Duration() > new TimeSpan(0, (Int16)3000, 0))
                {
                    LogEvent(LoggingLevel.Debug, $"Timeout period exceeded waiting for result");
                    return false;
                }

                Thread.Sleep(Convert.ToInt16(50));
            }

            return true;
        }

        public async Task<bool> WaitForResultAsync(Func<Task<bool>> successCheck, Func<Task<bool>> failedCheck, string logMessage = "")
        {
            var timestamp = DateTime.Now;

            while (!await successCheck())
            {
                if (failedCheck != null && await failedCheck())
                    return false;

                LogEvent(LoggingLevel.Info, $"...Waiting for result on '{logMessage}'");

                if ((DateTime.Now - timestamp).Duration() > new TimeSpan(0, (Int16)3000, 0))
                {
                    LogEvent(LoggingLevel.Debug, $"Timeout period exceeded waiting for result");
                    return false;
                }

                await Task.Delay(Convert.ToInt16(50));
            }

            return true;
        }

        public async Task SavePage()
        {
            var screenshot = await _page.ScreenshotBase64Async();
            File.WriteAllBytes($"./{_transactionMessage.Id}.bmp", Convert.FromBase64String(screenshot.ToString()));
        }

        public async Task DisposePlugin()
        {
            LogEvent(LoggingLevel.Info, $"Disposing Driver.");

            await _page.CloseAsync();
            _page.Dispose();

            await BrowserPages.ResetPage(_transactionMessage.Site);
        }
    }
}
