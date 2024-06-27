using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Cross.Identity.Api.Contracts.Models.Users
{
	public class UserCreateModel
	{
		[Required]
		[Description("The user name must be unique.")]
		[RegularExpression("[a-z0-9A-Z@._]+", ErrorMessage = "The {0} must contain only letters, numbers, underscore, @ or dot character (a-z, A-Z, 0-9, @ . _)")]
		public string UserName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

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

		/// <summary>
		/// Avatar url
		/// </summary>
		[DataType(DataType.Url)]
		public string? Avatar { get; set; }

		public string[] Roles { get; set; }

	}
}
