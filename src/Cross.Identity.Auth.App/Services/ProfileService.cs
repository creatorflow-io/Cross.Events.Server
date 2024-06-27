using Cross.Identity;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Cross.OAuthServer.Services
{
	/// <summary>
	/// We provide the role claims for the user, so that the role claims can be included in the access token.
	/// <para>It is useful for the SPA client to know the roles of the user.</para>
	/// <para>For backend, we have option to use <see cref="IClaimsTransformation"/> and access to Identity DB to get user roles</para>
	/// <para>NOTE: implement inheritence claims from roles of user for optional</para>
	/// </summary>
	internal class ProfileService : IProfileService
	{
		private UserManager<ApplicationUser> _userManager;
		private ILogger _logger;

		public ProfileService(UserManager<ApplicationUser> userManager, 
			ILogger<ProfileService> logger)
		{
			_userManager = userManager;
			_logger = logger;
		}

		public async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			context.LogProfileRequest(_logger);
			if(context.RequestedClaimTypes.Any())
			{
				_logger.LogDebug("Including role claims for user: {user}", context.Subject.GetSubjectId());
				var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());
				if(user!= null)
				{
					var roles = await _userManager.GetRolesAsync(user);
					_logger.LogDebug("User {user} has roles {roles}", user.UserName, roles);
					var roleClaims = roles.Select(r => new Claim("role", r)).ToList();
					context.IssuedClaims.AddRange(roleClaims);

					context.IssuedClaims.Add(new Claim("prefered_username", user.UserName));
					context.IssuedClaims.AddRange(user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)));
				}
				else
				{
					_logger.LogInformation("User not found: {user}", context.Subject.GetSubjectId());
				}
			}
			context.LogIssuedClaims(_logger);
		}

		public Task IsActiveAsync(IsActiveContext context)
		{
			return Task.CompletedTask;
		}
	}
}
