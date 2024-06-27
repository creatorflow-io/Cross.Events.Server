
namespace Cross.Identity.Api.Contracts.Models.Users
{
    public record UserRecordModel
    {
        public Guid Id { get; init; }
        public string UserName { get; init; }
        public string? Name { get; init; }
        public string? FirstName { get; init; }
        public string? Surname { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string[] Roles { get; init; }
    }
}
