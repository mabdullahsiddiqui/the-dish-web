CREATE SCHEMA IF NOT EXISTS reviews;
CREATE EXTENSION IF NOT EXISTS postgis;

CREATE TABLE IF NOT EXISTS reviews."Reviews" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "PlaceId" uuid NOT NULL,
    "Rating" integer NOT NULL DEFAULT 1,
    "Text" text NOT NULL,
    "PhotoUrls" text NOT NULL,
    "DietaryAccuracy" jsonb NOT NULL,
    "GpsVerified" boolean NOT NULL,
    "CheckInLocation" geography(Point, 4326),
    "HelpfulCount" integer NOT NULL DEFAULT 0,
    "NotHelpfulCount" integer NOT NULL DEFAULT 0,
    "Status" integer NOT NULL DEFAULT 0,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "IsDeleted" boolean NOT NULL,
    CONSTRAINT "PK_Reviews" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS reviews."ReviewHelpfulness" (
    "Id" uuid NOT NULL,
    "ReviewId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "IsHelpful" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "IsDeleted" boolean NOT NULL,
    CONSTRAINT "PK_ReviewHelpfulness" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ReviewHelpfulness_Reviews_ReviewId" FOREIGN KEY ("ReviewId") REFERENCES reviews."Reviews"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS reviews."ReviewPhotos" (
    "Id" uuid NOT NULL,
    "ReviewId" uuid NOT NULL,
    "Url" character varying(1000) NOT NULL,
    "ThumbnailUrl" character varying(1000),
    "Caption" character varying(500),
    "UploadedBy" uuid NOT NULL,
    "UploadedAt" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "IsDeleted" boolean NOT NULL,
    CONSTRAINT "PK_ReviewPhotos" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ReviewPhotos_Reviews_ReviewId" FOREIGN KEY ("ReviewId") REFERENCES reviews."Reviews"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_ReviewHelpfulness_ReviewId" ON reviews."ReviewHelpfulness"("ReviewId");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_ReviewHelpfulness_ReviewId_UserId" ON reviews."ReviewHelpfulness"("ReviewId", "UserId");
CREATE INDEX IF NOT EXISTS "IX_ReviewHelpfulness_UserId" ON reviews."ReviewHelpfulness"("UserId");
CREATE INDEX IF NOT EXISTS "IX_ReviewPhotos_ReviewId" ON reviews."ReviewPhotos"("ReviewId");
CREATE INDEX IF NOT EXISTS "IX_Reviews_GpsVerified" ON reviews."Reviews"("GpsVerified");
CREATE INDEX IF NOT EXISTS "IX_Reviews_PlaceId" ON reviews."Reviews"("PlaceId");
CREATE INDEX IF NOT EXISTS "IX_Reviews_PlaceId_CreatedAt" ON reviews."Reviews"("PlaceId", "CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Reviews_Rating" ON reviews."Reviews"("Rating");
CREATE INDEX IF NOT EXISTS "IX_Reviews_Status" ON reviews."Reviews"("Status");
CREATE INDEX IF NOT EXISTS "IX_Reviews_UserId" ON reviews."Reviews"("UserId");
CREATE INDEX IF NOT EXISTS "IX_Reviews_UserId_CreatedAt" ON reviews."Reviews"("UserId", "CreatedAt");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Reviews_UserId_PlaceId" ON reviews."Reviews"("UserId", "PlaceId");




