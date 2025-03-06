﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySoTietKiem.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrlColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                   name: "AvatarUrl",
                   table: "AspNetUsers",
                   type: "nvarchar(max)",
                   nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "AspNetUsers");
        }
    }
}
