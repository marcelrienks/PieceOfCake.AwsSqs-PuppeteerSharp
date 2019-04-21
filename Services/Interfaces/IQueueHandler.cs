using System.Threading.Tasks;
using Amazon.SQS.Model;
using SharedModels.Models;

namespace Services.Interfaces
{
    public interface IQueueHandler
    {
        Task<SendMessageResponse> SendMessageAsync(Query transaction);
    }
}