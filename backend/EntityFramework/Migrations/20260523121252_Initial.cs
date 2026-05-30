using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: true),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberType = table.Column<int>(type: "int", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Otp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GearItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToughTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfPurchase = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ManufacturerExpiry = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastInspection = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NextInspection = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    InspectedByUserId = table.Column<int>(type: "int", nullable: true),
                    LentToUserId = table.Column<int>(type: "int", nullable: true),
                    LentByUserId = table.Column<int>(type: "int", nullable: true),
                    LentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReturnedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ExpectedReturnDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    StorageLocation = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: true),
                    Sex = table.Column<int>(type: "int", nullable: true),
                    Length = table.Column<int>(type: "int", nullable: true),
                    GearCategory = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GearItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GearItems_Users_InspectedByUserId",
                        column: x => x.InspectedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GearItems_Users_LentByUserId",
                        column: x => x.LentByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GearItems_Users_LentToUserId",
                        column: x => x.LentToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logbook",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GearItemId = table.Column<int>(type: "int", nullable: false),
                    InspectedByUserId = table.Column<int>(type: "int", nullable: true),
                    LentToUserId = table.Column<int>(type: "int", nullable: true),
                    LentByUserId = table.Column<int>(type: "int", nullable: true),
                    LentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReturnedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logbook", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logbook_GearItems_GearItemId",
                        column: x => x.GearItemId,
                        principalTable: "GearItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Logbook_Users_InspectedByUserId",
                        column: x => x.InspectedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logbook_Users_LentByUserId",
                        column: x => x.LentByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logbook_Users_LentToUserId",
                        column: x => x.LentToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GearItems_InspectedByUserId",
                table: "GearItems",
                column: "InspectedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GearItems_LentByUserId",
                table: "GearItems",
                column: "LentByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GearItems_LentToUserId",
                table: "GearItems",
                column: "LentToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Logbook_GearItemId",
                table: "Logbook",
                column: "GearItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Logbook_InspectedByUserId",
                table: "Logbook",
                column: "InspectedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Logbook_LentByUserId",
                table: "Logbook",
                column: "LentByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Logbook_LentToUserId",
                table: "Logbook",
                column: "LentToUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logbook");

            migrationBuilder.DropTable(
                name: "GearItems");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
