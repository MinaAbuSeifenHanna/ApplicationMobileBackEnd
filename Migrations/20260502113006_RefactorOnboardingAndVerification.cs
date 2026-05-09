using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayHub.Backend.Migrations
{
    /// <inheritdoc />
    public partial class RefactorOnboardingAndVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                newName: "UnitContractDocumentUrl");

            migrationBuilder.RenameColumn(
                name: "FaceIdImageUrl",
                table: "AspNetUsers",
                newName: "SelfieImageUrl");

            migrationBuilder.RenameColumn(
                name: "ContractImageUrl",
                table: "AspNetUsers",
                newName: "ProfileImageUrl");

            migrationBuilder.RenameColumn(
                name: "BankIban",
                table: "AspNetUsers",
                newName: "BankAccountIban");

            migrationBuilder.RenameColumn(
                name: "BackIdImageUrl",
                table: "AspNetUsers",
                newName: "IdFrontImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "IdBackImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsIdentityVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdBackImageUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsIdentityVerified",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UnitContractDocumentUrl",
                table: "AspNetUsers",
                newName: "ProfilePictureUrl");

            migrationBuilder.RenameColumn(
                name: "SelfieImageUrl",
                table: "AspNetUsers",
                newName: "FaceIdImageUrl");

            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "AspNetUsers",
                newName: "ContractImageUrl");

            migrationBuilder.RenameColumn(
                name: "IdFrontImageUrl",
                table: "AspNetUsers",
                newName: "BackIdImageUrl");

            migrationBuilder.RenameColumn(
                name: "BankAccountIban",
                table: "AspNetUsers",
                newName: "BankIban");
        }
    }
}
