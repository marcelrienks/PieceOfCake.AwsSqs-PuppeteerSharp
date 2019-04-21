using System.Threading.Tasks;
using SharedModels.Models;

namespace Processor.Interfaces
{
    public interface IQueueHandler
    {
        Task<Query> SubscribeToQueue();
    }
}