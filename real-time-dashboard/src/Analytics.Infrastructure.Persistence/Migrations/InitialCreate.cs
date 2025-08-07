using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Analytics.Infrastructure.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PixelEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PlayerId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BannerTag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    SourceIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PixelEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastEventAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FirstDepositDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventDate = table.Column<DateTime>(type: "date", nullable: false),
                    EventType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BannerTag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSummaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    EventType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VisitCount = table.Column<int>(type: "integer", nullable: false),
                    RegistrationCount = table.Column<int>(type: "integer", nullable: false),
                    DepositCount = table.Column<int>(type: "integer", nullable: false),
                    ConversionRate = table.Column<decimal>(type: "numeric(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyMetrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dashboards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboards", x => x.Id);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_PixelEvents_BannerTag",
                table: "PixelEvents",
                column: "BannerTag");

            migrationBuilder.CreateIndex(
                name: "IX_PixelEvents_EventType",
                table: "PixelEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_PixelEvents_PlayerId",
                table: "PixelEvents",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PixelEvents_Timestamp",
                table: "PixelEvents",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Players_FirstDepositDate",
                table: "Players",
                column: "FirstDepositDate");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlayerId",
                table: "Players",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_RegistrationDate",
                table: "Players",
                column: "RegistrationDate");

            migrationBuilder.CreateIndex(
                name: "IX_EventSummaries_EventDate_EventType_BannerTag",
                table: "EventSummaries",
                columns: new[] { "EventDate", "EventType", "BannerTag" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyMetrics_Date_EventType",
                table: "DailyMetrics",
                columns: new[] { "Date", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_Name",
                table: "Dashboards",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PixelEvents");
            migrationBuilder.DropTable(name: "Players");
            migrationBuilder.DropTable(name: "EventSummaries");
            migrationBuilder.DropTable(name: "DailyMetrics");
            migrationBuilder.DropTable(name: "Dashboards");
        }
    }
}
