using NetTopologySuite.Geometries;
using TheDish.Review.Application.Interfaces;

namespace TheDish.Review.Infrastructure.Services;

public class GpsVerificationService : IGpsVerificationService
{
    public Task<bool> VerifyProximityAsync(
        double placeLatitude,
        double placeLongitude,
        double checkInLatitude,
        double checkInLongitude,
        double maxDistanceMeters = 200,
        CancellationToken cancellationToken = default)
    {
        var distance = CalculateDistanceInMeters(
            placeLatitude,
            placeLongitude,
            checkInLatitude,
            checkInLongitude);

        return Task.FromResult(distance <= maxDistanceMeters);
    }

    public Task<double> CalculateDistanceAsync(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2,
        CancellationToken cancellationToken = default)
    {
        var distance = CalculateDistanceInMeters(latitude1, longitude1, latitude2, longitude2);
        return Task.FromResult(distance);
    }

    private static double CalculateDistanceInMeters(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2)
    {
        // Haversine formula for accurate distance calculation
        const double earthRadiusMeters = 6371000; // Earth radius in meters
        
        var lat1Rad = latitude1 * Math.PI / 180.0;
        var lat2Rad = latitude2 * Math.PI / 180.0;
        var deltaLatRad = (latitude2 - latitude1) * Math.PI / 180.0;
        var deltaLonRad = (longitude2 - longitude1) * Math.PI / 180.0;

        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return earthRadiusMeters * c;
    }
}

