using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace Cross.Identity.Api.Contracts.Models.Roles
{
    public class RoleEditModel
    {
        [Required]
        [Description("The role name must be unique.")]
        [RegularExpression("[a-z0-9A-Z_]+", ErrorMessage = "The {0} must contain only letters, numbers, underscore character (a-z, A-Z, 0-9 _)")]
        public string Name { get; set; }
    }
}
