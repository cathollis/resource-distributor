using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hollis.ResourceDistributor.Functions.Migrations
{
    /// <inheritdoc />
    public partial class AddTableUserAndResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ResourceDistributor");

            migrationBuilder.CreateTable(
                name: "Resources",
                schema: "ResourceDistributor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    AllowAnymouse = table.Column<bool>(type: "bit", nullable: false),
                    TargetUrl = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    RequestCopyHeaderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseCopyHeaderName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "ResourceDistributor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClearTextKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Disabled = table.Column<bool>(type: "bit", nullable: false),
                    IdentificationName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceUser",
                schema: "ResourceDistributor",
                columns: table => new
                {
                    AllowUsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourcesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceUser", x => new { x.AllowUsersId, x.ResourcesId });
                    table.ForeignKey(
                        name: "FK_ResourceUser_Resources_ResourcesId",
                        column: x => x.ResourcesId,
                        principalSchema: "ResourceDistributor",
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceUser_Users_AllowUsersId",
                        column: x => x.AllowUsersId,
                        principalSchema: "ResourceDistributor",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceUser_ResourcesId",
                schema: "ResourceDistributor",
                table: "ResourceUser",
                column: "ResourcesId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ClearTextKey",
                schema: "ResourceDistributor",
                table: "Users",
                column: "ClearTextKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceUser",
                schema: "ResourceDistributor");

            migrationBuilder.DropTable(
                name: "Resources",
                schema: "ResourceDistributor");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "ResourceDistributor");
        }
    }
}
