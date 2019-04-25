using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherInsurance.Integration.Database.Migrations
{
    public partial class Addcontractdescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ContractName",
                table: "DeployedContracts",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DeployedContracts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "DeployedContracts");

            migrationBuilder.AlterColumn<string>(
                name: "ContractName",
                table: "DeployedContracts",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }
    }
}
