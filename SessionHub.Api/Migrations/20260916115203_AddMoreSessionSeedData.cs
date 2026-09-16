using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SessionHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreSessionSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "Description", "EndTimeUtc", "Level", "Room", "SpeakerId", "StartTimeUtc", "Summary", "Title", "Track" },
                values: new object[,]
                {
                    { 16, "This practical session shows how rapid feedback loops help cross-functional teams ship confidently, reduce rework, and keep stakeholders aligned.", new DateTimeOffset(new DateTime(2026, 9, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "Lakeside 3", 5, new DateTimeOffset(new DateTime(2026, 9, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use smaller release rhythms and clearer signals to improve delivery quality.", "Shipping Faster with Better Feedback Loops", "Product" },
                    { 17, "Learn the patterns that make AI-enabled workflows useful in real product teams, including guardrails, observability, and user trust.", new DateTimeOffset(new DateTime(2026, 9, 15, 11, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, "Atlas Hall", 6, new DateTimeOffset(new DateTime(2026, 9, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Design AI-assisted experiences that feel helpful, safe, and predictable.", "Patterns for Agentic Workflows", "AI" },
                    { 18, "This session distills the common hard-won lessons from taking a small application from an early demo into a real operational system.", new DateTimeOffset(new DateTime(2026, 9, 15, 12, 45, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "Grand Ballroom", 1, new DateTimeOffset(new DateTime(2026, 9, 15, 11, 45, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Turn a promising prototype into something a team can rely on long term.", "From Prototype to Production: Lessons Learned", "Architecture" },
                    { 19, "This session explores how disciplined styling choices can improve readability, responsiveness, and overall product polish without overbuilding the CSS layer.", new DateTimeOffset(new DateTime(2026, 9, 15, 14, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "Innovation Lab", 8, new DateTimeOffset(new DateTime(2026, 9, 15, 13, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use simple styling techniques to improve clarity and visual confidence.", "Practical CSS for Better Product Experiences", "Frontend" },
                    { 20, "Discover the practical habits that help small teams keep a system healthy as it grows without adding unnecessary ceremony.", new DateTimeOffset(new DateTime(2026, 9, 15, 15, 45, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "Harbor 1", 7, new DateTimeOffset(new DateTime(2026, 9, 15, 14, 45, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Build a sustainable rhythm around deployment, ownership, and reliability.", "Operational Excellence for Small Teams", "Platform" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 20);
        }
    }
}
