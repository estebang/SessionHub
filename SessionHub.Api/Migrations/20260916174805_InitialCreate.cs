using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SessionHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Speakers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Company = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speakers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Track = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Room = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SpeakerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Speakers_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "Speakers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Speakers",
                columns: new[] { "Id", "Bio", "Company", "FirstName", "LastName", "PhotoUrl", "Title" },
                values: new object[,]
                {
                    { 1, "Maira helps teams grow modern cloud architectures and developer productivity across large organizations.", "Northwind Labs", "Maira", "Lopez", "https://images.unsplash.com/...", "Principal Engineer" },
                    { 2, "Daniel connects engineering decisions with product strategy and measurable outcomes for customer experience.", "Brightlane", "Daniel", "Kemp", "", "Director of Product" },
                    { 3, "Priya specializes in resilient distributed systems, event-driven platforms, and operational excellence.", "Contoso Cloud", "Priya", "Nair", "", "Cloud Architect" },
                    { 4, "Lance teaches developers how to build better user experiences with practical, production-friendly guidance.", "BluePeak", "Lance", "Brooks", "", "Developer Advocate" },
                    { 5, "Alicia enjoys turning complex systems into approachable patterns that teams can trust and evolve.", "Fabrikam", "Alicia", "Stone", "", "Staff Software Engineer" },
                    { 6, "Marco helps organizations turn AI ideas into useful workflows that deliver real business value.", "DataWorks", "Marco", "Davis", "", "AI Solutions Lead" },
                    { 7, "Harper focuses on developer platforms, automation, and reliable engineering practices for scaling teams.", "Quanta Systems", "Harper", "Nguyen", "", "Platform Engineer" },
                    { 8, "Ethan bridges design and implementation to create intuitive, accessible digital products.", "Pixel Harbor", "Ethan", "Rossi", "", "UX Engineer" },
                    { 9, "Sofia helps teams design secure systems without slowing delivery or sacrificing usability.", "Redline Security", "Sofia", "Petrov", "", "Security Architect" },
                    { 10, "Jordan works with teams to modernize legacy systems and improve delivery through better architecture.", "Northwind Consulting", "Jordan", "Mills", "", "Senior Consultant" }
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "Description", "EndTimeUtc", "Level", "Room", "SpeakerId", "StartTimeUtc", "Summary", "Title", "Track" },
                values: new object[,]
                {
                    { 1, "This session walks through creating a small but realistic application from the ground up, covering architecture, APIs, UI, and demo-friendly decisions that make a conference talk easier to follow.", new DateTimeOffset(new DateTime(2026, 9, 14, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "Grand Ballroom", 1, new DateTimeOffset(new DateTime(2026, 9, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Learn how a focused product idea becomes a polished app with clear architecture and pragmatic decisions.", "From Idea to Ship: Building a Modern Conference Planner", "Architecture" },
                    { 2, "This walkthrough covers contract design, resource modeling, and DTO choices that help teams move quickly without creating brittle surfaces.", new DateTimeOffset(new DateTime(2026, 9, 14, 10, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "Harbor 1", 2, new DateTimeOffset(new DateTime(2026, 9, 14, 9, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "A simple API design approach that keeps developers productive and services easy to evolve.", "Designing HTTP APIs That Stay Friendly to Teams", "Backend" },
                    { 3, "This session highlights the practical patterns for using Entity Framework Core in a clean, readable app that feels realistic without becoming over-engineered.", new DateTimeOffset(new DateTime(2026, 9, 14, 11, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "Cedar Hall", 3, new DateTimeOffset(new DateTime(2026, 9, 14, 10, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use EF Core confidently for a small application with migrations, SQLite, and seed data.", "Entity Framework Core for Demo-Ready Apps", "Data" },
                    { 4, "Explore how a simple React view can effectively surface session data, speaker profiles, and browsing flows without extra complexity.", new DateTimeOffset(new DateTime(2026, 9, 14, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "Innovation Lab", 4, new DateTimeOffset(new DateTime(2026, 9, 14, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Build an interface that is easy to explain, fast to code, and pleasant to use.", "React Patterns for Conference Dashboards", "Frontend" },
                    { 5, "See why a focused service layer can keep logic clean and make a demo app easier to reason about in front of an audience.", new DateTimeOffset(new DateTime(2026, 9, 14, 14, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "Skyline 2", 5, new DateTimeOffset(new DateTime(2026, 9, 14, 13, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Keep business logic readable and testable in a small application architecture.", "Service Layers Without the Drama", "Architecture" },
                    { 6, "This session demonstrates how thoughtful API responses and user-friendly messaging improve every stage of an app.", new DateTimeOffset(new DateTime(2026, 9, 14, 14, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "Harbor 2", 6, new DateTimeOffset(new DateTime(2026, 9, 14, 13, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Create API feedback that keeps users informed and developers confident.", "Building Trust with Better Error Handling", "Backend" },
                    { 7, "Learn how small improvements in the app stack create a more responsive experience for users who are browsing a busy conference schedule.", new DateTimeOffset(new DateTime(2026, 9, 14, 15, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, "North Hall", 7, new DateTimeOffset(new DateTime(2026, 9, 14, 14, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Get fast wins with caching, response shaping, and layout choices.", "Performance Tuning for Real-World Web Apps", "Performance" },
                    { 8, "Use the SessionHub example to understand how sessions and speakers relate and how a small domain model supports a larger experience.", new DateTimeOffset(new DateTime(2026, 9, 14, 16, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "Cedar Hall", 8, new DateTimeOffset(new DateTime(2026, 9, 14, 15, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Think through the essential entities behind a conference planner.", "Designing a Data Model for Event Discovery", "Data" },
                    { 9, "Learn how request and response models help frontend and backend teams collaborate effectively when building user-visible features.", new DateTimeOffset(new DateTime(2026, 9, 15, 10, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "Harbor 1", 2, new DateTimeOffset(new DateTime(2026, 9, 15, 9, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Good contracts are documentation, not just implementation details.", "API Contracts That Help Teams Communicate", "Backend" },
                    { 10, "This talk explores how clean layout decisions, strong hierarchy, and clear content produce more usable software with less code.", new DateTimeOffset(new DateTime(2026, 9, 15, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "Innovation Lab", 4, new DateTimeOffset(new DateTime(2026, 9, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Build interfaces that tell a story without distracting from the main action.", "Minimal UI, Maximum Clarity", "Frontend" },
                    { 11, "This session covers the shared security design choices that matter most when shipping software quickly to customers.", new DateTimeOffset(new DateTime(2026, 9, 15, 12, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "Skyline 2", 9, new DateTimeOffset(new DateTime(2026, 9, 15, 11, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Look at practical patterns that help keep applications safer without heavy process overhead.", "Secure by Default: Common Design Patterns", "Security" },
                    { 12, "This talk shows how to evolve aging applications through clear boundaries, gradual changes, and careful modernization decisions.", new DateTimeOffset(new DateTime(2026, 9, 15, 13, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, "Grand Ballroom", 10, new DateTimeOffset(new DateTime(2026, 9, 15, 12, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use incremental patterns to improve existing systems with less risk.", "Modernizing Legacy Systems Without a Rewrite", "Architecture" },
                    { 13, "Learn tactics for making teams more effective through good tooling, straightforward automation, and smarter standards.", new DateTimeOffset(new DateTime(2026, 9, 15, 14, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "North Hall", 7, new DateTimeOffset(new DateTime(2026, 9, 15, 13, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Small process improvements can create a huge impact for teams and outcomes.", "Developer Experience That Scales", "Platform" },
                    { 14, "Explore how product teams can interpret signals and behaviors into a simple experience that helps users discover what matters.", new DateTimeOffset(new DateTime(2026, 9, 15, 15, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "Harbor 2", 2, new DateTimeOffset(new DateTime(2026, 9, 15, 14, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Turn scattered information into a practical product story that users understand.", "From Data to Decision: Building Useful Insights", "Product" },
                    { 15, "This session demonstrates how accessibility, usability, and maintainability all improve when we adopt simple design practices.", new DateTimeOffset(new DateTime(2026, 9, 15, 16, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "Innovation Lab", 8, new DateTimeOffset(new DateTime(2026, 9, 15, 15, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Build inclusive experiences without making the work more complicated for the team.", "The Power of Accessible Interfaces", "UX" },
                    { 16, "This practical session shows how rapid feedback loops help cross-functional teams ship confidently, reduce rework, and keep stakeholders aligned.", new DateTimeOffset(new DateTime(2026, 9, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "Lakeside 3", 5, new DateTimeOffset(new DateTime(2026, 9, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use smaller release rhythms and clearer signals to improve delivery quality.", "Shipping Faster with Better Feedback Loops", "Product" },
                    { 17, "Learn the patterns that make AI-enabled workflows useful in real product teams, including guardrails, observability, and user trust.", new DateTimeOffset(new DateTime(2026, 9, 15, 11, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, "Atlas Hall", 6, new DateTimeOffset(new DateTime(2026, 9, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Design AI-assisted experiences that feel helpful, safe, and predictable.", "Patterns for Agentic Workflows", "AI" },
                    { 18, "This session distills the common hard-won lessons from taking a small application from an early demo into a real operational system.", new DateTimeOffset(new DateTime(2026, 9, 15, 12, 45, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "Grand Ballroom", 1, new DateTimeOffset(new DateTime(2026, 9, 15, 11, 45, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Turn a promising prototype into something a team can rely on long term.", "From Prototype to Production: Lessons Learned", "Architecture" },
                    { 19, "This session explores how disciplined styling choices can improve readability, responsiveness, and overall product polish without overbuilding the CSS layer.", new DateTimeOffset(new DateTime(2026, 9, 15, 14, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "Innovation Lab", 8, new DateTimeOffset(new DateTime(2026, 9, 15, 13, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use simple styling techniques to improve clarity and visual confidence.", "Practical CSS for Better Product Experiences", "Frontend" },
                    { 20, "Discover the practical habits that help small teams keep a system healthy as it grows without adding unnecessary ceremony.", new DateTimeOffset(new DateTime(2026, 9, 15, 15, 45, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "Harbor 1", 7, new DateTimeOffset(new DateTime(2026, 9, 15, 14, 45, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Build a sustainable rhythm around deployment, ownership, and reliability.", "Operational Excellence for Small Teams", "Platform" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_SessionId",
                table: "Favorites",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SpeakerId",
                table: "Sessions",
                column: "SpeakerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Speakers");
        }
    }
}
