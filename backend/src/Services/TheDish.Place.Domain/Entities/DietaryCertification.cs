using TheDish.Common.Domain.Entities;
using TheDish.Place.Domain.Enums;

namespace TheDish.Place.Domain.Entities;

public class DietaryCertification : BaseEntity
{
    public Guid PlaceId { get; private set; }
    public string DietaryType { get; private set; } = string.Empty; // halal, kosher, vegan, etc.
    public string? CertificationLevel { get; private set; }
    
    // Official certificate details
    public string? CertificatePhotoUrl { get; private set; }
    public string? CertificateNumber { get; private set; }
    public string? CertifyingAuthority { get; private set; }
    public DateTime? IssueDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    
    // Verification status
    public CertificationStatus VerificationStatus { get; private set; } = CertificationStatus.Pending;
    public Guid? VerifiedBy { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    
    // Trust score components
    public int OfficialCertScore { get; private set; } = 0; // 0-100
    public int CommunityScore { get; private set; } = 0; // 0-100
    public int MenuScore { get; private set; } = 0; // 0-100
    public int VisitScore { get; private set; } = 0; // 0-100
    
    // Final trust score
    public decimal TrustScore { get; private set; } = 0; // 0-100
    
    // Metadata
    public Guid AddedBy { get; private set; }
    public DateTime? LastScoreUpdate { get; private set; }
    
    // Navigation property
    public virtual Place Place { get; private set; } = null!;

    private DietaryCertification() { }

    public DietaryCertification(
        Guid placeId,
        string dietaryType,
        Guid addedBy,
        string? certificationLevel = null)
    {
        if (placeId == Guid.Empty)
            throw new ArgumentException("Place ID is required", nameof(placeId));
        if (string.IsNullOrWhiteSpace(dietaryType))
            throw new ArgumentException("Dietary type is required", nameof(dietaryType));
        if (addedBy == Guid.Empty)
            throw new ArgumentException("Added by user ID is required", nameof(addedBy));

        PlaceId = placeId;
        DietaryType = dietaryType;
        AddedBy = addedBy;
        CertificationLevel = certificationLevel;
        VerificationStatus = CertificationStatus.Pending;
    }

    public void SetCertificateDetails(
        string? certificatePhotoUrl,
        string? certificateNumber,
        string? certifyingAuthority,
        DateTime? issueDate,
        DateTime? expiryDate)
    {
        CertificatePhotoUrl = certificatePhotoUrl;
        CertificateNumber = certificateNumber;
        CertifyingAuthority = certifyingAuthority;
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        UpdateTimestamp();
    }

    public void Verify(Guid verifiedBy)
    {
        VerificationStatus = CertificationStatus.Verified;
        VerifiedBy = verifiedBy;
        VerifiedAt = DateTime.UtcNow;
        RejectionReason = null;
        UpdateTimestamp();
    }

    public void Reject(Guid rejectedBy, string reason)
    {
        VerificationStatus = CertificationStatus.Rejected;
        VerifiedBy = rejectedBy;
        VerifiedAt = DateTime.UtcNow;
        RejectionReason = reason;
        UpdateTimestamp();
    }

    public void MarkExpired()
    {
        if (ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow)
        {
            VerificationStatus = CertificationStatus.Expired;
            UpdateTimestamp();
        }
    }

    public void UpdateTrustScores(
        int officialCertScore,
        int communityScore,
        int menuScore,
        int visitScore)
    {
        OfficialCertScore = Math.Clamp(officialCertScore, 0, 100);
        CommunityScore = Math.Clamp(communityScore, 0, 100);
        MenuScore = Math.Clamp(menuScore, 0, 100);
        VisitScore = Math.Clamp(visitScore, 0, 100);

        // Calculate final trust score (weighted average)
        TrustScore = (decimal)(
            (OfficialCertScore * 0.50) +
            (CommunityScore * 0.30) +
            (MenuScore * 0.10) +
            (VisitScore * 0.10)
        );

        LastScoreUpdate = DateTime.UtcNow;
        UpdateTimestamp();
    }
}









