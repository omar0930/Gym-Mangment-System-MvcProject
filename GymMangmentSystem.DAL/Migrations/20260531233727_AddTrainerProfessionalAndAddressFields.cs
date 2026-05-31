using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMangmentSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerProfessionalAndAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Specialty",
                table: "Trainers",
                type: "varchar(20)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "Trainers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Trainers",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "Trainers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Trainers",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Members",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Members",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Members");

            migrationBuilder.AlterColumn<int>(
                name: "Specialty",
                table: "Trainers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)");
        }
    }
}
