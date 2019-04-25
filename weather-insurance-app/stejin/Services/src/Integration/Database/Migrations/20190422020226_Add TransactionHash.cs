using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherInsurance.Integration.Database.Migrations
{
    public partial class AddTransactionHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionHash",
                table: "Fees",
                maxLength: 66,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionHash",
                table: "Fees");
        }
    }
}
