using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;
using TheDish.Place.Application.Interfaces;
using PlaceEntity = TheDish.Place.Domain.Entities.Place;

namespace TheDish.Place.Application.Commands;

public class UpdatePlaceRatingCommandHandler : IRequestHandler<UpdatePlaceRatingCommand, Response<PlaceDto>>
{
    private readonly IPlaceRepository _placeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePlaceRatingCommandHandler> _logger;

    public UpdatePlaceRatingCommandHandler(
        IPlaceRepository placeRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePlaceRatingCommandHandler> logger)
    {
        _placeRepository = placeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Response<PlaceDto>> Handle(UpdatePlaceRatingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var place = await _placeRepository.GetByIdAsync(request.PlaceId, cancellationToken);
            if (place == null)
            {
                return Response<PlaceDto>.FailureResult("Place not found");
            }

            // Update rating and review count
            place.UpdateRating(request.AverageRating, request.ReviewCount);

            await _placeRepository.UpdateAsync(place, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var placeDto = await MapToDtoAsync(place, cancellationToken);

            _logger.LogInformation("Place rating updated successfully: {PlaceId}, Rating: {Rating}, Count: {Count}", 
                request.PlaceId, request.AverageRating, request.ReviewCount);

            return Response<PlaceDto>.SuccessResult(placeDto, "Place rating updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating place rating: {PlaceId}", request.PlaceId);
            return Response<PlaceDto>.FailureResult("An error occurred while updating the place rating");
        }
    }

    private async Task<PlaceDto> MapToDtoAsync(PlaceEntity place, CancellationToken cancellationToken)
    {
        var placeWithDetails = await _placeRepository.GetByIdWithDetailsAsync(place.Id, cancellationToken);
        if (placeWithDetails == null)
        {
            throw new InvalidOperationException("Place not found after update");
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


