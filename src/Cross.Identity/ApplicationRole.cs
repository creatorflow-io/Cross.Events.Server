using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Cross.Attibutes;

namespace Cross.Identity
{
	[MongoCollection("IdentityRoles")]
	public class ApplicationRole : IdentityRole<Guid>, IIdentifiable<Guid>
	{
		[BsonGuidRepresentation(GuidRepresentation.Standard)]
		override public Guid Id { get; set; }

		public List<RoleClaim> Claims { get; set; }

		public ApplicationRole()
		{
			Claims = new List<RoleClaim>();
		}

		public ApplicationRole(string roleName) : base(roleName)
		{
			Claims = new List<RoleClaim>();
		}
	}
}
