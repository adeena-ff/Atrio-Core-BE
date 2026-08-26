using System;
using Atrio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atrio.Infrastructure.Persistence.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260826120000_AddTeacherClassAssignments")]
public partial class AddTeacherClassAssignments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "TeacherId",
            table: "Classes",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Classes_TeacherId",
            table: "Classes",
            column: "TeacherId");

        migrationBuilder.AddForeignKey(
            name: "FK_Classes_Users_TeacherId",
            table: "Classes",
            column: "TeacherId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Classes_Users_TeacherId", table: "Classes");
        migrationBuilder.DropIndex(name: "IX_Classes_TeacherId", table: "Classes");
        migrationBuilder.DropColumn(name: "TeacherId", table: "Classes");
    }
}
