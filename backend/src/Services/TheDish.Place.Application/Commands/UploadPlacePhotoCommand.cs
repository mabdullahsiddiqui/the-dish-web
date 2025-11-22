using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;

namespace TheDish.Place.Application.Commands;

public class UploadPlacePhotoCommand : IRequest<Response<PlacePhotoDto>>
{
    public Guid PlaceId { get; set; }
    public Guid UserId { get; set; }
    public Stream PhotoStream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public bool IsFeatured { get; set; } = false;
}











