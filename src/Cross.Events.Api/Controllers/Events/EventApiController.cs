using Cross.Events.Api.Authorization;
using Cross.Events.Domain.AggregateModels.EventAggregate;
using Juice.AspNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cross.Events.MongoDB;
using MongoDB.Driver.Linq;
using Cross.AspNetCore;
using MediatR;
using Cross.Events.Domain.Commands.Events;
using Juice;
using MongoDB.Driver;
using Cross.Events.Api.Controllers.Events.Models;

namespace Cross.Events.Api.Controllers.Events
{
    [ApiController]
    [Route("api/events")]
    [ApiExplorerSettings(GroupName = "tcpevents")]
    [IgnoreAntiforgeryToken]
    public class EventApiController : ControllerBase
    {
        private MongoRepository<TcpEvent, string> _repository;
        public EventApiController(MongoRepository<TcpEvent, string> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Query TcpEvents
        /// NOTE: SwaggerUI currently does not build query params correctly for array of objects, so I have to use POST to test easily.
        /// I expected a query string like this for Sorts array: sorts[0].Property=Name
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Query")]
        //[HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policies.EventReadPolicy)]
        public async Task<ActionResult<DatasourceResult<TcpEvent>>> QueryAsync(
           [FromBody] EventDatasourceRequest request
        )
        {
            try
            {
                request.Standardizing();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            try
            {
                var query = _repository.GetCollection().AsQueryable();
                if (!string.IsNullOrEmpty(request.Query))
                {
                    query = query.Where(x => x.Message.ToLower().Contains(request.Query.ToLower()));
                }
                if (request.Status.HasValue)
                {
                    query = query.Where(x => x.Status == request.Status.Value);
                }

                var result = await query.ToDatasourceResultAsync(request, HttpContext.RequestAborted);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Return TcpEvent by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policies.EventReadPolicy)]

        public async Task<ActionResult<TcpEvent>> GetAsync([FromRoute] string id)
        {
            var model = await _repository.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }


        [HttpPost("{id}/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policies.EventContributePolicy)]
        public async Task<ActionResult<IOperationResult>> ProcessAsync([FromRoute] string id, [FromServices] IMediator mediator)
        {
            try
            {
                var command = new ProcessTcpEventCommand(id);
                var result = await mediator.Send(command);
                if (result.Succeeded)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("{id}/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policies.EventAdminPolicy)]
        public async Task<ActionResult<IOperationResult>> AbandonAsync([FromRoute] string id, [FromServices] IMediator mediator)
        {
            try
            {
                var command = new AbandonTcpEventCommand(id);
                var result = await mediator.Send(command);
                if (result.Succeeded)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
