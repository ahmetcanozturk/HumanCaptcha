using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HumanCaptchaBackend.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Captchas",
                columns: table => new
                {
                    ID = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    Expires = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Captchas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Exceptions",
                columns: table => new
                {
                    ID = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExceptionTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    TypeName = table.Column<string>(unicode: false, nullable: false),
                    Message = table.Column<string>(unicode: false, nullable: false),
                    StackTrace = table.Column<string>(unicode: false, nullable: true),
                    InnerStack = table.Column<string>(unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exceptions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    Used = table.Column<bool>(nullable: false),
                    Expires = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Captchas");

            migrationBuilder.DropTable(
                name: "Exceptions");

            migrationBuilder.DropTable(
                name: "Tokens");
        }
    }
}
