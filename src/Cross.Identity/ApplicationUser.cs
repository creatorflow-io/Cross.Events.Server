using Cross.Attibutes;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cross.Identity
{
	[MongoCollection("IdentityUsers")]
	public class ApplicationUser : IdentityUser<Guid>, IIdentifiable<Guid>
    {
		[BsonGuidRepresentation(GuidRepresentation.Standard)]
		override public Guid Id { get; set; }

		public List<UserClaim> Claims { get; set; }
		public List<UserLogin> Logins { get; set; }
		public List<UserToken> Tokens { get; set; }

		public List<Guid> Roles { get; set; }

		public ApplicationUser()
		{
			Claims = new List<UserClaim>();
			Logins = new List<UserLogin>();
			Tokens = new List<UserToken>();
			Roles = new List<Guid>();
		}
    }
}
