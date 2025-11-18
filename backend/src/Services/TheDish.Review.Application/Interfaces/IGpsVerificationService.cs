namespace TheDish.Review.Application.Interfaces;

public interface IGpsVerificationService
{
    Task<bool> VerifyProximityAsync(
        double placeLatitude,
        double placeLongitude,
        double checkInLatitude,
        double checkInLongitude,
        double maxDistanceMeters = 200,
        CancellationToken cancellationToken = default);
    
    Task<double> CalculateDistanceAsync(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2,
        CancellationToken cancellationToken = default);
}








