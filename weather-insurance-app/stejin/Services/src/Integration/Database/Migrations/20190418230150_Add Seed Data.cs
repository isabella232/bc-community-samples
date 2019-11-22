using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherInsurance.Integration.Database.Migrations
{
    public partial class AddSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Networks",
                columns: new[] { "NetworkId", "NetworkName", "Platform", "ReferenceContractAddress", "Url" },
                values: new object[,]
                {
                    { 1L, "ETH:Unknown", 0, "0x0", "" },
                    { 2L, "ETH:Mainnet", 0, "0x13Cb835C47782dad075Ce7fAA1F8439b548B712D", "https://etherscan.io" },
                    { 3L, "ETH:Kovan", 0, "0x3422a48ebf29809bda10e264207ed94a5a819368", "https://kovan.etherscan.io" },
                    { 4L, "ETH:Sokol", 0, "0x64F84Fadae3F535BC02b17eD12a7Db33FBBEF29E", "" },
                    { 5L, "ETH:Ropsten", 0, "0x1F807D49324d83C3c5836Ad162839ba360EC834b", "https://ropsten.etherscan.io" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Networks",
                keyColumn: "NetworkId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Networks",
                keyColumn: "NetworkId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Networks",
                keyColumn: "NetworkId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Networks",
                keyColumn: "NetworkId",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Networks",
                keyColumn: "NetworkId",
                keyValue: 5L);
        }
    }
}
