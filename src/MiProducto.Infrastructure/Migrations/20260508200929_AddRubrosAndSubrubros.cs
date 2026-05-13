using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiProducto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRubrosAndSubrubros : Migration
    {
        /// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Crear tabla Rubros
    migrationBuilder.CreateTable(
        name: "Rubros",
        columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            IsActive = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        },
        constraints: table => table.PrimaryKey("PK_Rubros", x => x.Id));

    // Crear tabla Subrubros
    migrationBuilder.CreateTable(
        name: "Subrubros",
        columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            IsActive = table.Column<bool>(type: "boolean", nullable: false),
            RubroId = table.Column<Guid>(type: "uuid", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Subrubros", x => x.Id);
            table.ForeignKey("FK_Subrubros_Rubros_RubroId", x => x.RubroId, "Rubros", "Id", onDelete: ReferentialAction.Cascade);
        });

    // Insertar rubro y subrubro por defecto
    var rubroId = Guid.NewGuid();
    var subrubroId = Guid.NewGuid();

    migrationBuilder.InsertData("Rubros", 
        new[] { "Id", "Name", "IsActive", "CreatedAt" },
        new object[] { rubroId, "Sin clasificar", true, DateTime.UtcNow });

    migrationBuilder.InsertData("Subrubros",
        new[] { "Id", "Name", "IsActive", "RubroId", "CreatedAt" },
        new object[] { subrubroId, "Sin clasificar", true, rubroId, DateTime.UtcNow });

    // Agregar columnas RubroId y SubrubroId a Products con valor por defecto
    migrationBuilder.AddColumn<Guid>(
        name: "RubroId",
        table: "Products",
        type: "uuid",
        nullable: false,
        defaultValue: rubroId);

    migrationBuilder.AddColumn<Guid>(
        name: "SubrubroId",
        table: "Products",
        type: "uuid",
        nullable: false,
        defaultValue: subrubroId);

    // Agregar FKs
    migrationBuilder.AddForeignKey("FK_Products_Rubros_RubroId", "Products", "RubroId", "Rubros", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
    migrationBuilder.AddForeignKey("FK_Products_Subrubros_SubrubroId", "Products", "SubrubroId", "Subrubros", principalColumn: "Id", onDelete: ReferentialAction.Restrict);

    // Índices
    migrationBuilder.CreateIndex("IX_Subrubros_RubroId", "Subrubros", "RubroId");
    migrationBuilder.CreateIndex("IX_Products_RubroId", "Products", "RubroId");
    migrationBuilder.CreateIndex("IX_Products_SubrubroId", "Products", "SubrubroId");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Rubros_RubroId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Subrubros_SubrubroId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Subrubros");

            migrationBuilder.DropTable(
                name: "Rubros");

            migrationBuilder.DropIndex(
                name: "IX_Products_RubroId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SubrubroId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RubroId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SubrubroId",
                table: "Products");
        }
    }
}
