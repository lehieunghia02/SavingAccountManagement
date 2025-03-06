using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySoTietKiem.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingColumnsToGiaoDich : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiaoDichs_SoTietKiems_MaSoTietKiem",
                table: "GiaoDichs");

            migrationBuilder.AlterColumn<decimal>(
                name: "SoTien",
                table: "GiaoDichs",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(18)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "MaSoTietKiem",
                table: "GiaoDichs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "LoaiGiaoDich",
                table: "GiaoDichs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "GiaoDichs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "GiaoDichs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "GiaoDichs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichs_UserId",
                table: "GiaoDichs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GiaoDichs_AspNetUsers_UserId",
                table: "GiaoDichs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GiaoDichs_SoTietKiems_MaSoTietKiem",
                table: "GiaoDichs",
                column: "MaSoTietKiem",
                principalTable: "SoTietKiems",
                principalColumn: "MaSoTietKiem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiaoDichs_AspNetUsers_UserId",
                table: "GiaoDichs");

            migrationBuilder.DropForeignKey(
                name: "FK_GiaoDichs_SoTietKiems_MaSoTietKiem",
                table: "GiaoDichs");

            migrationBuilder.DropIndex(
                name: "IX_GiaoDichs_UserId",
                table: "GiaoDichs");

            migrationBuilder.DropColumn(
                name: "LoaiGiaoDich",
                table: "GiaoDichs");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "GiaoDichs");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "GiaoDichs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GiaoDichs");

            migrationBuilder.AlterColumn<double>(
                name: "SoTien",
                table: "GiaoDichs",
                type: "float(18)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "MaSoTietKiem",
                table: "GiaoDichs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GiaoDichs_SoTietKiems_MaSoTietKiem",
                table: "GiaoDichs",
                column: "MaSoTietKiem",
                principalTable: "SoTietKiems",
                principalColumn: "MaSoTietKiem",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
