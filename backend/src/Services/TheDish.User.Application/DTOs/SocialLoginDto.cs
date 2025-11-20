namespace TheDish.User.Application.DTOs;

public class SocialLoginDto
{
    public string Provider { get; set; } = string.Empty; // "Google" or "Facebook"
    public string Token { get; set; } = string.Empty; // ID token for Google, Access token for Facebook
}

