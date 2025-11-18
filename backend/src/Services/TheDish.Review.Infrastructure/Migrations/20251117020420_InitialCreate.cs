using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace TheDish.Review.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "reviews");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Text = table.Column<string>(type: "text", nullable: false),
                    PhotoUrls = table.Column<string>(type: "text", nullable: false),
                    DietaryAccuracy = table.Column<string>(type: "jsonb", nullable: false),
                    GpsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    CheckInLocation = table.Column<Point>(type: "geography(Point, 4326)", nullable: true),
                    HelpfulCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NotHelpfulCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewHelpfulness",
                schema: "reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsHelpful = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewHelpfulness", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewHelpfulness_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "reviews",
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewPhotos",
                schema: "reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewPhotos_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "reviews",
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewHelpfulness_ReviewId",
                schema: "reviews",
                table: "ReviewHelpfulness",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewHelpfulness_ReviewId_UserId",
                schema: "reviews",
                table: "ReviewHelpfulness",
                columns: new[] { "ReviewId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewHelpfulness_UserId",
                schema: "reviews",
                table: "ReviewHelpfulness",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPhotos_ReviewId",
                schema: "reviews",
                table: "ReviewPhotos",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_GpsVerified",
                schema: "reviews",
                table: "Reviews",
                column: "GpsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PlaceId",
                schema: "reviews",
                table: "Reviews",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PlaceId_CreatedAt",
                schema: "reviews",
                table: "Reviews",
                columns: new[] { "PlaceId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Rating",
                schema: "reviews",
                table: "Reviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Status",
                schema: "reviews",
                table: "Reviews",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                schema: "reviews",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId_CreatedAt",
                schema: "reviews",
                table: "Reviews",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId_PlaceId",
                schema: "reviews",
                table: "Reviews",
                columns: new[] { "UserId", "PlaceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewHelpfulness",
                schema: "reviews");

            migrationBuilder.DropTable(
                name: "ReviewPhotos",
                schema: "reviews");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "reviews");
        }
    }
}
