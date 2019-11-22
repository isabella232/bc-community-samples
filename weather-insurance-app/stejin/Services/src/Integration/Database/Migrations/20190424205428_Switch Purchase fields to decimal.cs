using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherInsurance.Integration.Database.Migrations
{
    public partial class SwitchPurchasefieldstodecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Premium",
                table: "Purchases",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<decimal>(
                name: "Notional",
                table: "Purchases",
                nullable: false,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Premium",
                table: "Purchases",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<long>(
                name: "Notional",
                table: "Purchases",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
