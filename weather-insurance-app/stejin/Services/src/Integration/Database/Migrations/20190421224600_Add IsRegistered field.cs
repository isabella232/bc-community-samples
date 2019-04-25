using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherInsurance.Integration.Database.Migrations
{
    public partial class AddIsRegisteredfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRegistered",
                table: "DeployedContracts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRegistered",
                table: "DeployedContracts");
        }
    }
}
