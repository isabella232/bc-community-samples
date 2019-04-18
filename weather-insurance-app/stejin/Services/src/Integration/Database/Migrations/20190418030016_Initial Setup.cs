using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherInsurance.Integration.Database.Migrations
{
    public partial class InitialSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractFiles",
                columns: table => new
                {
                    ContractFileId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractFileName = table.Column<string>(maxLength: 50, nullable: false),
                    IncludesJson = table.Column<bool>(nullable: false),
                    IncludesSol = table.Column<bool>(nullable: false),
                    OwnerAddress = table.Column<string>(maxLength: 42, nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    ContractType = table.Column<string>(maxLength: 25, nullable: true),
                    ApiVersion = table.Column<string>(maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractFiles", x => x.ContractFileId);
                });

            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    NetworkId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NetworkName = table.Column<string>(maxLength: 50, nullable: false),
                    Platform = table.Column<int>(nullable: false),
                    Url = table.Column<string>(maxLength: 255, nullable: false),
                    ReferenceContractAddress = table.Column<string>(maxLength: 42, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => x.NetworkId);
                });

            migrationBuilder.CreateTable(
                name: "DeployedContracts",
                columns: table => new
                {
                    ContractAddress = table.Column<string>(maxLength: 42, nullable: false),
                    ContractFileId = table.Column<long>(nullable: false),
                    NetworkId = table.Column<long>(nullable: false),
                    ContractName = table.Column<string>(maxLength: 20, nullable: false),
                    OwnerAddress = table.Column<string>(maxLength: 42, nullable: false),
                    ContractType = table.Column<int>(nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(nullable: false),
                    ConstructorArguments = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeployedContracts", x => x.ContractAddress);
                    table.ForeignKey(
                        name: "FK_DeployedContracts_ContractFiles_ContractFileId",
                        column: x => x.ContractFileId,
                        principalTable: "ContractFiles",
                        principalColumn: "ContractFileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeployedContracts_Networks_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Networks",
                        principalColumn: "NetworkId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractFiles_ContractFileName_OwnerAddress",
                table: "ContractFiles",
                columns: new[] { "ContractFileName", "OwnerAddress" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeployedContracts_ContractFileId",
                table: "DeployedContracts",
                column: "ContractFileId");

            migrationBuilder.CreateIndex(
                name: "IX_DeployedContracts_ContractName",
                table: "DeployedContracts",
                column: "ContractName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeployedContracts_NetworkId",
                table: "DeployedContracts",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_DeployedContracts_OwnerAddress",
                table: "DeployedContracts",
                column: "OwnerAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Networks_NetworkName",
                table: "Networks",
                column: "NetworkName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeployedContracts");

            migrationBuilder.DropTable(
                name: "ContractFiles");

            migrationBuilder.DropTable(
                name: "Networks");
        }
    }
}
