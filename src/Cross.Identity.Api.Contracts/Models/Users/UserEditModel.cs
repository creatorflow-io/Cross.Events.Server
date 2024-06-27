using System.ComponentModel.DataAnnotations;

namespace Cross.Identity.Api.Contracts.Models.Users
{
	public class UserEditModel
	{
		public string? Name { get; set; }
		public string? FirstName { get; set; }
		public string? Surname { get; set; }

		[DataType(DataType.EmailAddress)]
		[EmailAddress]
		public string? Email { get; set; }

		[DataType(DataType.PhoneNumber)]
		[Phone]
		public string? PhoneNumber { get; set; }

		public bool TwoFactorEnabled { get; set; }

		public bool LockoutEnabled { get; set; }
		public bool PhoneNumberConfirmed { get; set; }
		public bool EmailConfirmed { get; set; }
	}
}
