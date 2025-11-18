using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace TheDish.Place.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "places");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Places",
                schema: "places",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<Point>(type: "geography(Point, 4326)", nullable: false),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CuisineTypes = table.Column<string>(type: "text", nullable: false),
                    PriceRange = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    DietaryTags = table.Column<string>(type: "jsonb", nullable: false),
                    TrustScores = table.Column<string>(type: "jsonb", nullable: false),
                    AverageRating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false, defaultValue: 0m),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ClaimedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    HoursOfOperation = table.Column<string>(type: "jsonb", nullable: true),
                    Amenities = table.Column<string>(type: "text", nullable: false),
                    ParkingInfo = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DietaryCertifications",
                schema: "places",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    DietaryType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CertificationLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CertificatePhotoUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CertificateNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CertifyingAuthority = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationStatus = table.Column<int>(type: "integer", nullable: false),
                    VerifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OfficialCertScore = table.Column<int>(type: "integer", nullable: false),
                    CommunityScore = table.Column<int>(type: "integer", nullable: false),
                    MenuScore = table.Column<int>(type: "integer", nullable: false),
                    VisitScore = table.Column<int>(type: "integer", nullable: false),
                    TrustScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    AddedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastScoreUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietaryCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DietaryCertifications_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalSchema: "places",
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                schema: "places",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DietaryTags = table.Column<string>(type: "text", nullable: false),
                    AllergenWarnings = table.Column<string>(type: "text", nullable: false),
                    SpiceLevel = table.Column<int>(type: "integer", nullable: true),
                    IsPopular = table.Column<bool>(type: "boolean", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    PhotoUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalSchema: "places",
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlacePhotos",
                schema: "places",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacePhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlacePhotos_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalSchema: "places",
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DietaryCertifications_ExpiryDate",
                schema: "places",
                table: "DietaryCertifications",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_DietaryCertifications_PlaceId_DietaryType",
                schema: "places",
                table: "DietaryCertifications",
                columns: new[] { "PlaceId", "DietaryType" });

            migrationBuilder.CreateIndex(
                name: "IX_DietaryCertifications_TrustScore",
                schema: "places",
                table: "DietaryCertifications",
                column: "TrustScore");

            migrationBuilder.CreateIndex(
                name: "IX_DietaryCertifications_VerificationStatus",
                schema: "places",
                table: "DietaryCertifications",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_Category",
                schema: "places",
                table: "MenuItems",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_PlaceId",
                schema: "places",
                table: "MenuItems",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_PlacePhotos_PlaceId",
                schema: "places",
                table: "PlacePhotos",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_PlacePhotos_PlaceId_IsFeatured",
                schema: "places",
                table: "PlacePhotos",
                columns: new[] { "PlaceId", "IsFeatured" });

            migrationBuilder.CreateIndex(
                name: "IX_Places_AverageRating_ReviewCount",
                schema: "places",
                table: "Places",
                columns: new[] { "AverageRating", "ReviewCount" });

            migrationBuilder.CreateIndex(
                name: "IX_Places_ClaimedBy",
                schema: "places",
                table: "Places",
                column: "ClaimedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Places_Location",
                schema: "places",
                table: "Places",
                column: "Location")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_Places_Name",
                schema: "places",
                table: "Places",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Places_Status",
                schema: "places",
                table: "Places",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DietaryCertifications",
                schema: "places");

            migrationBuilder.DropTable(
                name: "MenuItems",
                schema: "places");

            migrationBuilder.DropTable(
                name: "PlacePhotos",
                schema: "places");

            migrationBuilder.DropTable(
                name: "Places",
                schema: "places");
        }
    }
}
