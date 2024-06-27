using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System.Security.Claims;
using Cross.MongoDB.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Cross.Identity.Stores
{
	internal class RoleStore : RoleStoreBase<ApplicationRole, Guid, UserRole, RoleClaim>
	{
		private IMongoDatabase _database;

		public RoleStore([FromKeyedServices(Constants.DbKey)] IMongoDatabase database, IdentityErrorDescriber? errorDescriber = null)
			: base(errorDescriber ?? new IdentityErrorDescriber())
		{
			_database = database;
		}

		private IMongoCollection<ApplicationRole> _roles => _database.GetCollection<ApplicationRole>();

		public override IQueryable<ApplicationRole> Roles => _roles.AsQueryable();

		public override async Task AddClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ArgumentNullException.ThrowIfNull(role);

			var filter = Builders<ApplicationRole>.Filter.Eq(x => x.Id, role.Id);
			var update = Builders<ApplicationRole>.Update.Push(x => x.Claims, new RoleClaim { ClaimType = claim.Type, ClaimValue = claim.Value });

			await _roles.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
		}

		public override async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(role);

			await _roles.InsertOneAsync(role, cancellationToken: cancellationToken);
			return IdentityResult.Success;
		}

		public override async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(role);

			try
			{
				await _roles.DeleteOneAsync(x => x.Id == role.Id, cancellationToken);
				return IdentityResult.Success;
			}
			catch (Exception ex)
			{
				return IdentityResult.Failed(new IdentityError { Code = ex.GetType().Name, Description = ex.Message });
			}
		}

		public override async Task<ApplicationRole?> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
		{
			return await _roles.Find(x => x.NormalizedName == normalizedName).FirstOrDefaultAsync(cancellationToken);
		}

		public override Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default)
		{
			return Task.FromResult<IList<Claim>>(role.Claims.Select(x => new Claim(x.ClaimType!, x.ClaimValue!)).ToList());
		}

		public override async Task RemoveClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(role);
			ArgumentNullException.ThrowIfNull(claim);
			var filter = Builders<ApplicationRole>.Filter.Eq(x => x.Id, role.Id);
			var update = Builders<ApplicationRole>.Update.PullFilter(x => x.Claims, x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);

			await _roles.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
		}

		public override async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			ArgumentNullException.ThrowIfNull(role);

			var oldStamp = role.ConcurrencyStamp;
			role.ConcurrencyStamp = Guid.NewGuid().ToString();

			var filter = Builders<ApplicationRole>.Filter.Eq(x => x.Id, role.Id) & Builders<ApplicationRole>.Filter.Eq(x => x.ConcurrencyStamp, oldStamp);
			try
			{
				var rs = await _roles.ReplaceOneAsync(filter, role, cancellationToken: cancellationToken);

				if (rs.IsAcknowledged && rs.ModifiedCount == 1)
				{
					return IdentityResult.Success;
				}
				else
				{
					return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
				}
			}
			catch (MongoWriteException ex)
			{
				return IdentityResult.Failed(new IdentityError { Code = ex.GetType().Name, Description = ex.Message });
			}
			catch (Exception ex)
			{
				return IdentityResult.Failed(new IdentityError { Code = ex.GetType().Name, Description = ex.Message });
			}
		}

		public override async Task<ApplicationRole?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
		{
			return await _roles.Find(x => x.Id == Guid.Parse(id)).FirstOrDefaultAsync(cancellationToken);
		}
	}
}
