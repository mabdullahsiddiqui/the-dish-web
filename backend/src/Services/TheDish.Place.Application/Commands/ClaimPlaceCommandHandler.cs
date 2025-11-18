using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;
using TheDish.Place.Application.Interfaces;
using PlaceEntity = TheDish.Place.Domain.Entities.Place;

namespace TheDish.Place.Application.Commands;

public class ClaimPlaceCommandHandler : IRequestHandler<ClaimPlaceCommand, Response<PlaceDto>>
{
    private readonly IPlaceRepository _placeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClaimPlaceCommandHandler> _logger;

    public ClaimPlaceCommandHandler(
        IPlaceRepository placeRepository,
        IUnitOfWork unitOfWork,
        ILogger<ClaimPlaceCommandHandler> logger)
    {
        _placeRepository = placeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Response<PlaceDto>> Handle(ClaimPlaceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var place = await _placeRepository.GetByIdAsync(request.PlaceId, cancellationToken);
            if (place == null)
            {
                return Response<PlaceDto>.FailureResult("Place not found");
            }

            if (place.ClaimedBy.HasValue)
            {
                return Response<PlaceDto>.FailureResult("Place is already claimed");
            }

            place.Claim(request.UserId);
            await _placeRepository.UpdateAsync(place, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var placeDto = await MapToDtoAsync(place, cancellationToken);

            _logger.LogInformation("Place claimed successfully: {PlaceId} by {UserId}", request.PlaceId, request.UserId);

            return Response<PlaceDto>.SuccessResult(placeDto, "Place claimed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error claiming place: {PlaceId}", request.PlaceId);
            return Response<PlaceDto>.FailureResult("An error occurred while claiming the place");
        }
    }

    private async Task<PlaceDto> MapToDtoAsync(PlaceEntity place, CancellationToken cancellationToken)
    {
        var placeWithDetails = await _placeRepository.GetByIdWithDetailsAsync(place.Id, cancellationToken);
        if (placeWithDetails == null)
        {
            throw new InvalidOperationException("Place not found after claim");
        }

        return new PlaceDto
        {
            Id = placeWithDetails.Id,
            Name = placeWithDetails.Name,
            Address = placeWithDetails.Address,
            Latitude = placeWithDetails.Location.Y,
            Longitude = placeWithDetails.Location.X,
            Phone = placeWithDetails.Phone,
            Website = placeWithDetails.Website,
            Email = placeWithDetails.Email,
            CuisineTypes = placeWithDetails.CuisineTypes,
            PriceRange = placeWithDetails.PriceRange,
            DietaryTags = placeWithDetails.DietaryTags,
            TrustScores = placeWithDetails.TrustScores,
            AverageRating = placeWithDetails.AverageRating,
            ReviewCount = placeWithDetails.ReviewCount,
            ClaimedBy = placeWithDetails.ClaimedBy,
            IsVerified = placeWithDetails.IsVerified,
            Status = placeWithDetails.Status.ToString(),
            Photos = placeWithDetails.Photos.Select(p => new PlacePhotoDto
            {
                Id = p.Id,
                PlaceId = p.PlaceId,
                Url = p.Url,
                ThumbnailUrl = p.ThumbnailUrl,
                Caption = p.Caption,
                UploadedBy = p.UploadedBy,
                IsFeatured = p.IsFeatured,
                DisplayOrder = p.DisplayOrder,
                UploadedAt = p.UploadedAt
            }).ToList(),
            MenuItems = placeWithDetails.MenuItems.Select(m => new MenuItemDto
            {
                Id = m.Id,
                PlaceId = m.PlaceId,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                Category = m.Category,
                DietaryTags = m.DietaryTags,
                AllergenWarnings = m.AllergenWarnings,
                SpiceLevel = m.SpiceLevel,
                IsPopular = m.IsPopular,
                IsAvailable = m.IsAvailable,
                PhotoUrl = m.PhotoUrl
            }).ToList(),
            CreatedAt = placeWithDetails.CreatedAt,
            UpdatedAt = placeWithDetails.UpdatedAt
        };
    }
}

