using Cross.MongoDB.Extensions;
using Juice.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;

namespace Cross.Identity.Stores
{
	internal class UserStore : UserStoreBase<ApplicationUser, ApplicationRole, Guid, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>
	{
		private IMongoDatabase _database;
		public UserStore([FromKeyedServices(Constants.DbKey)]IMongoDatabase database, IdentityErrorDescriber? errorDescriber = null)
			: base(errorDescriber?? new IdentityErrorDescriber())
		{
			_database = database;
		}

		private IMongoCollection<ApplicationUser> _users => _database.GetCollection<ApplicationUser>();
		private IMongoCollection<ApplicationRole> _roles => _database.GetCollection<ApplicationRole>();

		public override IQueryable<ApplicationUser> Users => _users.AsQueryable();

		public override async Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			var newClaims = claims
				.Where(c1 => !user.Claims.Any(c => c1.Type == c.ClaimType && c1.Value == c.ClaimValue))
				.Select(c => new UserClaim
				{
					UserId = user.Id,
					ClaimType = c.Type,
					ClaimValue = c.Value
				});

			user.Claims.AddRange(newClaims);

			var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, user.Id);
			var claimsUpdate = Builders<ApplicationUser>.Update.AddToSetEach(u => u.Claims, newClaims);

			await _users.UpdateOneAsync(filter, claimsUpdate, cancellationToken: cancellationToken);

		}

		public override async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			var userLogin = new UserLogin
			{
				UserId = user.Id,
				LoginProvider = login.LoginProvider,
				ProviderKey = login.ProviderKey,
				ProviderDisplayName = login.ProviderDisplayName
			};
			await _users.UpdateOneAsync(u => u.Id == user.Id, 
				Builders<ApplicationUser>.Update.AddToSet(u => u.Logins, userLogin), 
				cancellationToken: cancellationToken);
		}

		public override async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(user);
			await _users.InsertOneAsync(user, cancellationToken: cancellationToken);
			return IdentityResult.Success;
		}

		public override async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(user);
			try
			{
				await _users.DeleteOneAsync(u => u.Id == user.Id, cancellationToken);
				return IdentityResult.Success;
			}catch(Exception ex)
			{
				return IdentityResult.Failed(new IdentityError { Code = ex.GetType().Name, Description = ex.Message });
			}
		}

		public override async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
		{
			return await _users.Find(u => u.NormalizedEmail == normalizedEmail).FirstOrDefaultAsync(cancellationToken);
		}

		public override async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
		{
			return await _users.Find(u => u.Id == Guid.Parse(userId)).FirstOrDefaultAsync(cancellationToken);
		}

		public override async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
		{
			return await _users.Find(u => u.NormalizedUserName == normalizedUserName).FirstOrDefaultAsync(cancellationToken);
		}

		public override async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
		{
			await Task.Yield();
			return user.Claims.Select(c => new Claim(c.ClaimType!, c.ClaimValue!)).ToList();
		}

		public override async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
		{
			await Task.Yield();
			return user.Logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName)).ToList();
		}

		public override async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
		{
			return await _users.Find(u => u.Claims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value)).ToListAsync(cancellationToken);
		}

		public override async Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(user);
			ArgumentNullException.ThrowIfNull(claims);
			
			var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, user.Id);

			var claimsUpdate = Builders<ApplicationUser>.Update.PullFilter(u => u.Claims, c => claims.Any(c1 => c1.Type == c.ClaimType && c1.Value == c.ClaimValue));

			await _users.UpdateOneAsync(filter, 
						claimsUpdate, 
						cancellationToken: cancellationToken);
		}

		public override async Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(user);
			
			var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, user.Id);

			var loginUpdate = Builders<ApplicationUser>.Update.PullFilter(u => u.Logins, l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);

			await _users.UpdateOneAsync(filter, loginUpdate, cancellationToken: cancellationToken);
		}

		public override async Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
		{
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(user);
			ArgumentNullException.ThrowIfNull(claim);
			ArgumentNullException.ThrowIfNull(newClaim);

			var claimIndex = user.Claims.FindIndex(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
			if(claimIndex < 0)
			{
                return;
            }

			user.Claims[claimIndex].ClaimType = newClaim.Type;
			user.Claims[claimIndex].ClaimValue = newClaim.Value;
			var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, user.Id);
			var update = Builders<ApplicationUser>.Update.Set("Claims.$[c].ClaimType", newClaim.Type)
												.Set("Claims.$[c].ClaimValue", newClaim.Value);

			var options = new UpdateOptions { ArrayFilters = new List<ArrayFilterDefinition> { 
				new BsonDocumentArrayFilterDefinition<BsonDocument>(
					new BsonDocument("c.ClaimType", claim.Type).Add("c.ClaimValue", claim.Value)) } };

			await _users.UpdateOneAsync(filter, update, options, cancellationToken: cancellationToken);
		}

		public override async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(user);

			var oldStamp = user.ConcurrencyStamp;
			user.ConcurrencyStamp = Guid.NewGuid().ToString();
			try
			{
				var rs = await _users.ReplaceOneAsync(u => u.Id == user.Id && (u.ConcurrencyStamp == null || u.ConcurrencyStamp.Equals(oldStamp)),
					user, cancellationToken: cancellationToken);
				if (rs.IsAcknowledged && rs.ModifiedCount == 1)
				{
					return IdentityResult.Success;
				}
				else
				{
					return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
				}
			}
			catch (Exception ex)
			{
				return IdentityResult.Failed(new IdentityError { Code = ex.GetType().Name, Description = ex.Message});
			}
		}

		protected override async Task AddUserTokenAsync(UserToken token)
		{
			var user = await _users.Find(u => u.Id == token.UserId).FirstOrDefaultAsync();
			if (user != null)
			{
				user.Tokens.Add(token);
				await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
			}
		}

		protected override async Task<UserToken?> FindTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
		{
			await Task.Yield();
			ArgumentNullException.ThrowIfNull(user);
			return user.Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);
		}

		protected override async Task<ApplicationUser?> FindUserAsync(Guid userId, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(userId);
			return await _users.Find(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);
		}

		protected override async Task<UserLogin?> FindUserLoginAsync(Guid userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(userId);

			return await _users.Find(u => u.Id == userId && u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
				.Project(u => u.Logins.Where(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey).FirstOrDefault())
				.FirstOrDefaultAsync(cancellationToken);
		}

		protected override async Task<UserLogin?> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
		{
			return await _users.Find(u => u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
				.Project(u => u.Logins.Where(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey).FirstOrDefault())
				.FirstOrDefaultAsync(cancellationToken);
		}

		protected override async Task RemoveUserTokenAsync(UserToken token)
		{
			var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, token.UserId);

			var loginUpdate = Builders<ApplicationUser>.Update.PullFilter(u => u.Tokens,
					l => l.LoginProvider == token.LoginProvider && l.Name == token.Name);

			await _users.UpdateOneAsync(filter, loginUpdate);

		}

		public override async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
		{
			var role = await _roles.Find(r => r.NormalizedName == normalizedRoleName).FirstOrDefaultAsync(cancellationToken);
			if(role == null)
			{
				return new List<ApplicationUser>();
			}

			return await _users.Find(u => u.Roles.Any(r => r == role.Id)).ToListAsync(cancellationToken);
		}

		public override async Task AddToRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(user);
			ArgumentNullException.ThrowIfNull(normalizedRoleName);

			var role = await _roles.Find(r => r.NormalizedName == normalizedRoleName).FirstOrDefaultAsync(cancellationToken);
			if(role != null && !user.Roles.Any(r => r == role.Id))
			{
				user.Roles.Add(role.Id);
				var rs = await _users.UpdateOneAsync(u => u.Id == user.Id, 
					Builders<ApplicationUser>.Update.AddToSet(u => u.Roles, role.Id),
					cancellationToken: cancellationToken);
				if(rs.IsModifiedCountAvailable && rs.ModifiedCount == 0)
				{
					throw new InvalidOperationException("Failed to add user to role");
				}
			}
		}

		public override async Task RemoveFromRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(user);
			ArgumentNullException.ThrowIfNull(normalizedRoleName);

			var role = await _roles.Find(r => r.NormalizedName == normalizedRoleName).FirstOrDefaultAsync(cancellationToken);
			if (role != null)
			{
				user.Roles.Remove(role.Id);
				await _users.UpdateOneAsync(u => u.Id == user.Id, 
					Builders<ApplicationUser>.Update.Pull(u => u.Roles, role.Id), 
					cancellationToken: cancellationToken);
			}
		}

		public override async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(user);

			return await _roles.Find(r => user.Roles.Any(ur => ur == r.Id))
                .Project(r => r.Name)
                .ToListAsync(cancellationToken);
		}

		public override async Task<bool> IsInRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(user);

			var role = await _roles.Find(r => r.NormalizedName == normalizedRoleName).FirstOrDefaultAsync(cancellationToken);
			if(role == null)
			{
				return false;
			}

			return user.Roles.Any(r => r == role.Id);
		}

		protected override async Task<ApplicationRole?> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			return await _roles.Find(r => r.NormalizedName == normalizedRoleName).FirstOrDefaultAsync(cancellationToken);
		}

		protected override async Task<UserRole?> FindUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
		{
			return await _users.Find(u => u.Id == userId && u.Roles.Any(r => r == roleId))
                .Project(u => new UserRole { UserId = userId, RoleId = roleId })
                .FirstOrDefaultAsync(cancellationToken);
		}
	}

}
