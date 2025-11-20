using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using TheDish.User.Application.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace TheDish.User.Infrastructure.Services;

public class OAuthService : IOAuthService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public OAuthService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<OAuthUserInfo?> ValidateGoogleTokenAsync(string token)
    {
        try
        {
            var clientId = _configuration["OAuth:Google:ClientId"];
            if (string.IsNullOrEmpty(clientId))
            {
                throw new InvalidOperationException("Google Client ID not configured");
            }

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

            return new OAuthUserInfo
            {
                Id = payload.Subject,
                Email = payload.Email,
                FirstName = payload.GivenName ?? "",
                LastName = payload.FamilyName ?? "",
                Picture = payload.Picture
            };
        }
        catch (Exception ex)
        {
            // Log all errors with details
            throw new InvalidOperationException($"Error validating Google token: {ex.GetType().Name} - {ex.Message}", ex);
        }
    }

    public async Task<OAuthUserInfo?> ValidateFacebookTokenAsync(string token)
    {
        try
        {
            var appId = _configuration["OAuth:Facebook:AppId"];
            var appSecret = _configuration["OAuth:Facebook:AppSecret"];

            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appSecret))
            {
                throw new InvalidOperationException("Facebook App ID or Secret not configured");
            }

            // First, verify the token
            var verifyUrl = $"https://graph.facebook.com/debug_token?input_token={token}&access_token={appId}|{appSecret}";
            var verifyResponse = await _httpClient.GetAsync(verifyUrl);
            
            if (!verifyResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var verifyContent = await verifyResponse.Content.ReadAsStringAsync();
            var verifyData = JsonSerializer.Deserialize<JsonElement>(verifyContent);

            if (!verifyData.GetProperty("data").GetProperty("is_valid").GetBoolean())
            {
                return null;
            }

            var userId = verifyData.GetProperty("data").GetProperty("user_id").GetString();
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            // Get user info
            var userInfoUrl = $"https://graph.facebook.com/v18.0/{userId}?fields=id,email,first_name,last_name,picture&access_token={token}";
            var userInfoResponse = await _httpClient.GetAsync(userInfoUrl);

            if (!userInfoResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var userInfoContent = await userInfoResponse.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<JsonElement>(userInfoContent);

            return new OAuthUserInfo
            {
                Id = userInfo.GetProperty("id").GetString() ?? userId,
                Email = userInfo.TryGetProperty("email", out var email) ? email.GetString() ?? "" : "",
                FirstName = userInfo.TryGetProperty("first_name", out var firstName) ? firstName.GetString() ?? "" : "",
                LastName = userInfo.TryGetProperty("last_name", out var lastName) ? lastName.GetString() ?? "" : "",
                Picture = userInfo.TryGetProperty("picture", out var picture) && 
                         picture.TryGetProperty("data", out var data) &&
                         data.TryGetProperty("url", out var url) ? url.GetString() : null
            };
        }
        catch
        {
            return null;
        }
    }
}

