using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using SharedModels.Models;
using System;
using System.Threading.Tasks;

namespace Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        private readonly IQueueHandler _queueHandler;

        /// <summary>
        /// Instantiate the TransactionsController, using the dependancy injected concrete IQueueHandler object.
        /// Note: this IQueueHandler object is created in startup.
        /// </summary>
        /// <param name="queueHandler">The dependancy injected IQueueHandler object created at startup</param>
        public QueriesController(IQueueHandler queueHandler)
        {
            _queueHandler = queueHandler;
        }

        /// <summary>
        /// This is just a dummy method, which allows the post method to return using 'CreatedAtAction'
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<Query> GetQuery(System.Guid id)
        {
            return new Query() { Id = id, Site = Enums.Sites.Google };
        }

        /// <summary>
        /// Handle the Post method for creating a query
        /// </summary>
        /// <param name="query">The transaction to be created</param>
        [HttpPost]
        public async Task<ActionResult<Query>> PostQuery([FromBody] Query query)
        {
            query.Id = Guid.NewGuid();

            // Create a transaction message on the AWS queue
            await _queueHandler.SendMessageAsync(query);

            // Return the created transaction
            return CreatedAtAction(nameof(GetQuery), new { id = query.Id }, query);
        }

        /// <summary>
        /// Handle the post method for creating a number of queries with random sites
        /// This is used for bulk testing
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpPost("{count}")]
        public async Task<ActionResult<Query>> PostQueries(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Create a transaction message on the AWS queue
                await _queueHandler.SendMessageAsync(new Query() { Id = Guid.NewGuid(), Site = (Enums.Sites)new Random().Next(0, 2) });
            }

            // Return the created transaction
            return StatusCode(201);
        }
    }
}