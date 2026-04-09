using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayHub.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddContractImageUrlToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContractImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractImageUrl",
                table: "AspNetUsers");
        }
    }
}
