namespace TheDish.User.Application.Interfaces;

public interface IOAuthService
{
    Task<OAuthUserInfo?> ValidateGoogleTokenAsync(string token);
    Task<OAuthUserInfo?> ValidateFacebookTokenAsync(string token);
}

public class OAuthUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Picture { get; set; }
}

