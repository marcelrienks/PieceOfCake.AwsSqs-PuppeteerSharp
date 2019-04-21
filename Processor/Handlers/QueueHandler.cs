using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using SharedModels.Models;
using Processor.Interfaces;
using static SharedModels.Models.Enums;
using static Processor.Handlers.LoggingHandler;

namespace Processor.Handlers
{
    public class QueueHandler : IQueueHandler
    {
        private readonly IAmazonSQS _sqsClient;

        /// <summary>
        /// Instantiate the QueueHandler, using the dependancy injected concrete IAmazonSQS object.
        /// Note: this IAmazonSQS object is created in startup, and will use the config settings under appsettings.Development.json/AWS.
        /// </summary>
        /// <param name="queueClient">The dependancy injected IAmazonSQS object created at startup</param>
        public QueueHandler(IAmazonSQS sqsClient)
        {
            _sqsClient = sqsClient;
        }

        public async Task<Query> SubscribeToQueue()
        {
            Query transaction = null;

            // **********************
            // I don't like the idea of using a while loop, but there does not seem to be any other option
            // **********************
            while (transaction == null)
            {
                LogEvent(LoggingLevel.Info, $"...Polling...");
                transaction = await ReadTransaction();
                Thread.Sleep(100);
            }

            return transaction;
        }

        private async Task<Query> ReadTransaction()
        {
            Query transaction = null;

            // Receive message from the Queue
            var receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest());
            if (receiveMessageResponse.Messages.Any())
            {
                var message = receiveMessageResponse.Messages.FirstOrDefault();

                // Attempt to deserialise message
                transaction = JsonConvert.DeserializeObject<Query>(message.Body);

                // If we were anble to deserialise, remove message from Queue
                await DeleteTransaction(message.ReceiptHandle);
            }

            return transaction;
        }

        private async Task DeleteTransaction(string recieptHandle)
        {
            var deleteMessageRequest = new DeleteMessageRequest { ReceiptHandle = recieptHandle };
            var response = await _sqsClient.DeleteMessageAsync(deleteMessageRequest);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new System.Exception($"Failed to delete message: {recieptHandle}");
        }
    }
}