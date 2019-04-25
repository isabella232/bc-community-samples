using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherInsurance.Integration.Database.Migrations
{
    public partial class AddFees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fees",
                columns: table => new
                {
                    FeeId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Sender = table.Column<string>(maxLength: 42, nullable: false),
                    Recipient = table.Column<string>(maxLength: 42, nullable: false),
                    ContractAddress = table.Column<string>(nullable: false),
                    NetworkId = table.Column<long>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<long>(nullable: false),
                    IsConfirmed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fees", x => x.FeeId);
                    table.ForeignKey(
                        name: "FK_Fees_DeployedContracts_ContractAddress_NetworkId",
                        columns: x => new { x.ContractAddress, x.NetworkId },
                        principalTable: "DeployedContracts",
                        principalColumns: new[] { "ContractAddress", "NetworkId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fees_ContractAddress_NetworkId",
                table: "Fees",
                columns: new[] { "ContractAddress", "NetworkId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fees");
        }
    }
}
