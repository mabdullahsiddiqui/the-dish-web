using TheDish.Common.Domain.Entities;
using TheDish.User.Domain.Enums;

namespace TheDish.User.Domain.Entities;

public class User : AggregateRoot
{
    public string Email { get; private set; } = string.Empty;
    public string? PasswordHash { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    
    // Social login fields
    public ExternalProvider ExternalProvider { get; private set; } = ExternalProvider.Email;
    public string? ExternalProviderId { get; private set; }
    public string? ExternalProviderEmail { get; private set; }
    
    // User profile
    public int Reputation { get; private set; } = 0;
    public int ReviewCount { get; private set; } = 0;
    public bool IsVerified { get; private set; } = false;
    public DateTime JoinDate { get; private set; } = DateTime.UtcNow;
    
    // Password reset
    public string? PasswordResetCode { get; private set; }
    public DateTime? PasswordResetCodeExpiry { get; private set; }
    
    private User() { }

    public User(
        string email,
        string firstName,
        string lastName,
        string? passwordHash = null,
        ExternalProvider externalProvider = ExternalProvider.Email,
        string? externalProviderId = null)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        ExternalProvider = externalProvider;
        ExternalProviderId = externalProviderId;
        ExternalProviderEmail = externalProvider == ExternalProvider.Email ? null : email;
        JoinDate = DateTime.UtcNow;
    }

    public static User CreateWithEmail(
        string email,
        string firstName,
        string lastName,
        string passwordHash)
    {
        return new User(email, firstName, lastName, passwordHash, ExternalProvider.Email);
    }

    public static User CreateWithSocialLogin(
        string email,
        string firstName,
        string lastName,
        ExternalProvider provider,
        string providerId)
    {
        return new User(email, firstName, lastName, null, provider, providerId)
        {
            ExternalProviderEmail = email
        };
    }

    public void LinkSocialAccount(ExternalProvider provider, string providerId, string? email = null)
    {
        ExternalProvider = provider;
        ExternalProviderId = providerId;
        if (email != null)
        {
            ExternalProviderEmail = email;
        }
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public void IncrementReviewCount()
    {
        ReviewCount++;
    }

    public void UpdateReputation(int points)
    {
        Reputation = Math.Max(0, Reputation + points);
    }

    public ReputationLevel GetReputationLevel()
    {
        return Reputation switch
        {
            >= 500 => ReputationLevel.Diamond,
            >= 300 => ReputationLevel.Platinum,
            >= 150 => ReputationLevel.Gold,
            >= 50 => ReputationLevel.Silver,
            _ => ReputationLevel.Bronze
        };
    }

    public void SetPasswordResetCode(string code, DateTime expiry)
    {
        PasswordResetCode = code;
        PasswordResetCodeExpiry = expiry;
    }

    public void ClearPasswordResetCode()
    {
        PasswordResetCode = null;
        PasswordResetCodeExpiry = null;
    }

    public bool IsPasswordResetCodeValid(string code)
    {
        if (string.IsNullOrEmpty(PasswordResetCode) || PasswordResetCodeExpiry == null)
        {
            return false;
        }

        if (DateTime.UtcNow > PasswordResetCodeExpiry.Value)
        {
            return false;
        }

        return PasswordResetCode == code;
    }

    public void UpdatePasswordHash(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }
}

