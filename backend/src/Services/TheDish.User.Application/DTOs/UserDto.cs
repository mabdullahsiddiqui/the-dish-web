using TheDish.User.Domain.Enums;

namespace TheDish.User.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ExternalProvider { get; set; } = "Email";
    public int Reputation { get; set; }
    public string ReputationLevel { get; set; } = string.Empty;
    public int ReviewCount { get; set; }
    public bool IsVerified { get; set; }
    public DateTime JoinDate { get; set; }
}

