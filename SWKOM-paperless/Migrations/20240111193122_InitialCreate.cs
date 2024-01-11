using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Org.OpenAPITools.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Correspondents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Match = table.Column<string>(type: "text", nullable: false),
                    MatchingAlgorithm = table.Column<long>(type: "bigint", nullable: false),
                    IsInsensitive = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentCount = table.Column<long>(type: "bigint", nullable: false),
                    LastCorrespondence = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Correspondents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Match = table.Column<string>(type: "text", nullable: false),
                    MatchingAlgorithm = table.Column<long>(type: "bigint", nullable: false),
                    IsInsensitive = table.Column<bool>(type: "boolean", nullable: false),
                    IsInboxTag = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Correspondent = table.Column<int>(type: "integer", nullable: true),
                    DocumentType = table.Column<int>(type: "integer", nullable: true),
                    StoragePath = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<List<int>>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Match = table.Column<string>(type: "text", nullable: false),
                    MatchingAlgorithm = table.Column<long>(type: "bigint", nullable: false),
                    IsInsensitive = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewCorrespondents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Match = table.Column<string>(type: "text", nullable: false),
                    MatchingAlgorithm = table.Column<long>(type: "bigint", nullable: false),
                    IsInsensitive = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentCount = table.Column<long>(type: "bigint", nullable: false),
                    LastCorrespondence = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewCorrespondents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewDocumentTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Match = table.Column<string>(type: "text", nullable: false),
                    MatchingAlgorithm = table.Column<long>(type: "bigint", nullable: false),
                    IsInsensitive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewDocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Match = table.Column<string>(type: "text", nullable: false),
                    MatchingAlgorithm = table.Column<long>(type: "bigint", nullable: false),
                    IsInsensitive = table.Column<bool>(type: "boolean", nullable: false),
                    IsInboxTag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Correspondents");

            migrationBuilder.DropTable(
                name: "DocTags");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "NewCorrespondents");

            migrationBuilder.DropTable(
                name: "NewDocumentTypes");

            migrationBuilder.DropTable(
                name: "NewTags");

            migrationBuilder.DropTable(
                name: "UserInfos");
        }
    }
}
