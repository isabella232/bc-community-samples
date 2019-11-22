using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherInsurance.Integration.Database.Migrations
{
    public partial class AddDeployedContractscompositekey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeployedContracts",
                table: "DeployedContracts");

            migrationBuilder.RenameColumn(
                name: "ContractType",
                table: "ContractFiles",
                newName: "ContractFileType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeployedContracts",
                table: "DeployedContracts",
                columns: new[] { "ContractAddress", "NetworkId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeployedContracts",
                table: "DeployedContracts");

            migrationBuilder.RenameColumn(
                name: "ContractFileType",
                table: "ContractFiles",
                newName: "ContractType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeployedContracts",
                table: "DeployedContracts",
                column: "ContractAddress");
        }
    }
}
