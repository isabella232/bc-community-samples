using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherInsurance.Integration.Database.Migrations
{
    public partial class AddPurchases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    PurchaseId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractAddress = table.Column<string>(maxLength: 42, nullable: true),
                    NetworkId = table.Column<long>(nullable: false),
                    UserAddress = table.Column<string>(maxLength: 42, nullable: true),
                    Notional = table.Column<decimal>(nullable: false),
                    Premium = table.Column<byte[]>(maxLength: 32, nullable: false),
                    PurchaseTimestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.PurchaseId);
                    table.ForeignKey(
                        name: "FK_Purchases_DeployedContracts_ContractAddress_NetworkId",
                        columns: x => new { x.ContractAddress, x.NetworkId },
                        principalTable: "DeployedContracts",
                        principalColumns: new[] { "ContractAddress", "NetworkId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ContractAddress_NetworkId",
                table: "Purchases",
                columns: new[] { "ContractAddress", "NetworkId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchases");
        }
    }
}
