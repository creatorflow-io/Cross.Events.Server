using Cross.Events.Api.Authorization;
using Cross.Events.Domain.AggregateModels.ClientAggregate;
using Juice.AspNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cross.Events.MongoDB;
using MongoDB.Driver.Linq;
using Cross.AspNetCore;
using MediatR;
using Juice;
using MongoDB.Driver;
using Cross.Events.Api.Controllers.Clients.Models;
using Juice.Domain;

namespace Cross.Events.Api.Controllers.Clients
{
    [ApiController]
    [Route("api/clients")]
    [ApiExplorerSettings(GroupName = "tcpevents")]
    [IgnoreAntiforgeryToken]
    [Authorize(Policies.ClientContributePolicy)]
    public class ClientApiController : ControllerBase
    {
        private MongoRepository<TcpClient, string> _repository;
        public ClientApiController(MongoRepository<TcpClient, string> repository)
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
        public async Task<ActionResult<DatasourceResult<TcpClient>>> QueryAsync(
           [FromBody] ClientDatasourceRequest request
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
                    query = query.Where(x => x.IpAddress.ToLower().Contains(request.Query.ToLower()));
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
        public async Task<ActionResult<TcpClient>> GetAsync([FromRoute] string id)
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
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policies.EventContributePolicy)]
        public async Task<ActionResult<IOperationResult>> AcceptAsync([FromRoute] string id)
        {
            try
            {
                var tcpClient = await _repository.GetByIdAsync(id);
                if(tcpClient == null)
                {
					return NotFound();
				}
                tcpClient.Accept();
                try { 
					tcpClient.ThrowIfHasErrors();
				}
				catch (Exception ex)
                {
					return BadRequest(ex.Message);
				}
				await _repository.UpdateAsync(tcpClient);
                return Ok(OperationResult.Success);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
			}
        }

        [HttpPost("{id}/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IOperationResult>> BanAsync([FromRoute] string id)
		{
			try
			{
				var tcpClient = await _repository.GetByIdAsync(id);
				if (tcpClient == null)
				{
					return NotFound();
				}
				tcpClient.Ban();
				try
				{
					tcpClient.ThrowIfHasErrors();
				}
				catch (Exception ex)
				{
					return BadRequest(ex.Message);
				}
				await _repository.UpdateAsync(tcpClient);
				return Ok(OperationResult.Success);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
    }
}
