using Cross.Events.MongoDB;
using Cross.Identity.Api.Authorization;
using Cross.Identity.Api.Contracts.Models.Users;
using Cross.Identity.Api.Services;
using Cross.MongoDB.Extensions;
using Juice.AspNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Security.Claims;

namespace Cross.Identity.Api.Controllers.Users
{
    [ApiController]
    [Route("api/user")]
    [ApiExplorerSettings(GroupName = "identity")]
    [IgnoreAntiforgeryToken]
    [Authorize(Policy = Policies.IdentityAdmin)]
    public class UserController : ControllerBase
    {
        private MongoRepository<ApplicationUser, Guid> _repository;
        public UserController(MongoRepository<ApplicationUser, Guid> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Query Users
        /// </summary>
        /// <param name="request"></param>
        /// <param name="viewService"></param>
        /// <returns></returns>
        [HttpPost("Query")]
        //[HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DatasourceResult<UserRecordModel>>> QueryAsync(
           [FromBody] DatasourceRequest request,
           [FromServices] UserViewService viewService
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

                var rs = await viewService.GetDatasourceResultAsync(request, HttpContext.RequestAborted);

                return Ok(rs);
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
        /// <param name="viewService"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ActionName("Get")]
        public async Task<ActionResult<UserRecordModel>> GetAsync([FromRoute] Guid id,
            [FromServices] UserViewService viewService)
        {
            var model = await viewService.GetUserAsync(id);
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
        /// <param name="userManager"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateAsync([FromBody] UserCreateModel user,
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await userManager.FindByNameAsync(user.UserName);
            if (existing != null)
            {
                return BadRequest("The user name is already taken.");
            }

            var model = new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                TwoFactorEnabled = user.TwoFactorEnabled
            };
            try
            {
                var rs = await userManager.CreateAsync(model, user.Password);
                if (!rs.Succeeded)
                {
                    return BadRequest();
                }
                var claims = new List<Claim>();
                if (!string.IsNullOrEmpty(user.Name))
                {
                    claims.Add(new Claim("name", user.Name));
                }
                if (!string.IsNullOrEmpty(user.FirstName))
                {
                    claims.Add(new Claim("given_name", user.FirstName));
                }
                if (!string.IsNullOrEmpty(user.Surname))
                {
                    claims.Add(new Claim("family_name", user.Surname));
                }
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    claims.Add(new Claim("picture", user.Avatar));
                }
                if (claims.Any())
                {
                    rs = await userManager.AddClaimsAsync(model, claims);
                    if (!rs.Succeeded)
                    {
                        return BadRequest(rs);
                    }
                }

                if(user.Roles!=null && user.Roles.Any())
                {
                    var rs1 = await userManager.AddToRolesAsync(model, user.Roles);
                    if (!rs1.Succeeded)
                    {
                        return BadRequest(rs1);
                    }
                }

                return CreatedAtAction("Get", new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] UserEditModel user,
            Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = await _repository.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            model.EmailConfirmed = user.EmailConfirmed;
            model.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            model.LockoutEnabled = user.LockoutEnabled;
            model.TwoFactorEnabled = user.TwoFactorEnabled;

            try
            {
                if (!model.Claims.Any(c => c.ClaimType == "name" && c.ClaimValue == user.Name))
                {
                    model.Claims.RemoveAll(x => x.ClaimType == "name");
                    if (!string.IsNullOrEmpty(user.Name))
                    {
                        model.Claims.Add(new UserClaim { ClaimType = "name", ClaimValue = user.Name });
                    }
                }

                if (!model.Claims.Any(c => c.ClaimValue == "given_name" && c.ClaimValue == user.FirstName))
                {
                    model.Claims.RemoveAll(x => x.ClaimType == "given_name");
                    if (!string.IsNullOrEmpty(user.Surname))
                    {
                        model.Claims.Add(new UserClaim { ClaimType = "given_name", ClaimValue = user.Surname });
                    }
                }

                if (!model.Claims.Any(c => c.ClaimValue == "family_name" && c.ClaimValue == user.Surname))
                {
                    model.Claims.RemoveAll(x => x.ClaimType == "family_name");
                    if (!string.IsNullOrEmpty(user.Surname))
                    {
                        model.Claims.Add(new UserClaim { ClaimType = "family_name", ClaimValue = user.Surname });
                    }
                }

                await _repository.UpdateAsync(model);

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
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            var model = await _repository.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            try
            {
                await userManager.DeleteAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}/password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangePasswordAsync(Guid id,
            [FromBody] ChangePasswordModel passwordModel,
            [FromServices] UserManager<ApplicationUser> userManager)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = await _repository.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            try
            {
                var rs = await userManager.RemovePasswordAsync(model);
                if (!rs.Succeeded)
                {
                    return BadRequest(rs);
                }
                rs = await userManager.AddPasswordAsync(model, passwordModel.Password);
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


        [HttpPut("{id}/roles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateRolesAsync(Guid id,
            [FromBody] List<string> roles,
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            var model = await _repository.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            try
            {
                var rs = await userManager.AddToRolesAsync(model, roles);
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
    }
}
