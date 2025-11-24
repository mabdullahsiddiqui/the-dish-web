-- Update Place Ratings and Review Counts from Reviews
-- This script calculates the average rating and review count for each place
-- based on active, non-deleted reviews

UPDATE places."Places" p
SET 
    "AverageRating" = COALESCE((
        SELECT AVG(r."Rating"::numeric)
        FROM reviews."Reviews" r
        WHERE r."PlaceId" = p."Id"
          AND r."IsDeleted" = false
          AND r."Status" = 0  -- Active status
    ), 0),
    "ReviewCount" = COALESCE((
        SELECT COUNT(*)
        FROM reviews."Reviews" r
        WHERE r."PlaceId" = p."Id"
          AND r."IsDeleted" = false
          AND r."Status" = 0  -- Active status
    ), 0);

-- Show the results
SELECT 
    "Id",
    "Name",
    "AverageRating",
    "ReviewCount"
FROM places."Places"
ORDER BY "ReviewCount" DESC, "AverageRating" DESC;



