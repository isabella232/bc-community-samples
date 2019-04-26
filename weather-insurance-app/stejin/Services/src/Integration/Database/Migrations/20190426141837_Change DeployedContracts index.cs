using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherInsurance.Integration.Database.Migrations
{
    public partial class ChangeDeployedContractsindex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeployedContracts_ContractName",
                table: "DeployedContracts");

            migrationBuilder.CreateIndex(
                name: "IX_DeployedContracts_ContractName_NetworkId",
                table: "DeployedContracts",
                columns: new[] { "ContractName", "NetworkId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeployedContracts_ContractName_NetworkId",
                table: "DeployedContracts");

            migrationBuilder.CreateIndex(
                name: "IX_DeployedContracts_ContractName",
                table: "DeployedContracts",
                column: "ContractName",
                unique: true);
        }
    }
}
