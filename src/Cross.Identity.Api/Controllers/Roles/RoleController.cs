using Cross.Events.MongoDB;
using Cross.Identity.Api.Authorization;
using Cross.Identity.Api.Contracts.Models.Roles;
using Cross.Identity.Api.Services;
using Juice.AspNetCore.Models;
using Juice.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Cross.Identity.Api.Controllers.Users
{
	[ApiController]
	[Route("api/role")]
	[ApiExplorerSettings(GroupName = "identity")]
	[IgnoreAntiforgeryToken]
	[Authorize(Policy = Policies.IdentityAdmin)]
	public class RoleController : ControllerBase
	{
		private MongoRepository<ApplicationRole, Guid> _repository;
		public RoleController(MongoRepository<ApplicationRole, Guid> repository)
		{
			_repository = repository;
		}

        /// <summary>
        /// Query Roles
        /// </summary>
        /// <param name="request"></param>
        /// <param name="viewService"></param>
        /// <returns></returns>
        [HttpPost("Query")]
		//[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<DatasourceResult<ApplicationRole>>> QueryAsync(
		   [FromBody] DatasourceRequest request,
		   [FromServices] RoleViewService viewService
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
				
				var result = await viewService.GetDatasourceResultAsync(request, HttpContext.RequestAborted);
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
		[ActionName("Get")]
		public async Task<ActionResult<ApplicationRole>> GetAsync([FromRoute] Guid id)
		{
			var model = await _repository.GetByIdAsync(id);
			if (model == null)
			{
				return NotFound();
			}

			return Ok(model);
		}

		/// <summary>
		/// Add new user
		/// </summary>
		/// <param name="user"></param>
		/// <param name="roleManager"></param>
		/// <returns></returns>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> CreateAsync([FromBody] RoleCreateModel role,
			[FromServices] RoleManager<ApplicationRole> roleManager)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var existing = await roleManager.FindByNameAsync(role.Name);
			if (existing != null)
			{
                return BadRequest("Role name already exists.");
            }

			var model = new ApplicationRole(role.Name);
			try
			{
				var rs = await roleManager.CreateAsync(model);
				if (!rs.Succeeded)
				{
					return BadRequest(rs);
				}
				
				return CreatedAtAction("Get", new { model.Id }, model);
			}catch(Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> UpdateAsync([FromBody] RoleEditModel role,
			[FromServices] RoleManager<ApplicationRole> roleManager,
			Guid id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

            var existing = await roleManager.FindByNameAsync(role.Name);
            if (existing != null && existing.Id!=id)
            {
                return BadRequest("Role name already exists.");
            }

			try
			{
				var model = await _repository.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound();
                }

                if ("admin".Equals(model.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Cannot edit Admin role.");
                }
                var rs = await roleManager.SetRoleNameAsync(model, role.Name);

				if (!rs.Succeeded)
				{
					return BadRequest(rs);
				}
				rs = await roleManager.UpdateAsync(model);

                if (!rs.Succeeded)
                {
                    return BadRequest(rs);
                }

                return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> DeleteAsync(Guid id,
			[FromServices] RoleManager<ApplicationRole> roleManager)
		{
			var model = await _repository.GetByIdAsync(id);
			if (model == null)
			{
				return NotFound();
			}
			if("admin".Equals(model.Name, StringComparison.OrdinalIgnoreCase))
			{
                return BadRequest("Cannot delete Admin role.");
            }
			try
			{
				await roleManager.DeleteAsync(model);
				return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

	}
}
