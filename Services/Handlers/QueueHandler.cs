using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using Services.Interfaces;
using SharedModels.Models;
using System.Threading.Tasks;

namespace Services.Handlers
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

        /// <summary>
        /// Send a message to the queue asynchronously
        /// </summary>
        /// <param name="transaction">The Query to be created on the queue</param>
        /// <returns></returns>
        public async Task<SendMessageResponse> SendMessageAsync(Query transaction)
        {
            // Create message queue request
            var message = new SendMessageRequest { MessageBody = JsonConvert.SerializeObject(transaction) };

            // Send message queue request
            return await _sqsClient.SendMessageAsync(message);
        }
    }
}
