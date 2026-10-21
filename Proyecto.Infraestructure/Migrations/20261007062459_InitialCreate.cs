using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Proyecto.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DefaultIntervalDays = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTypes", x => x.Id);
                    table.CheckConstraint("CK_MaintenanceTypes_DefaultIntervalDays", "[DefaultIntervalDays] IS NULL OR [DefaultIntervalDays] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TaxId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AcquiredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchaseCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UsefulLifeMonths = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.CheckConstraint("CK_Assets_PurchaseCost", "[PurchaseCost] >= 0");
                    table.CheckConstraint("CK_Assets_Status", "[Status] IN ('Active', 'UnderMaintenance', 'Retired')");
                    table.CheckConstraint("CK_Assets_UsefulLifeMonths", "[UsefulLifeMonths] > 0");
                    table.ForeignKey(
                        name: "FK_Assets_AssetCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "AssetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    MaintenanceTypeId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, defaultValue: "Medium"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Planned"),
                    ScheduledFor = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Findings = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceOrders", x => x.Id);
                    table.CheckConstraint("CK_MaintenanceOrders_Cost", "[Cost] IS NULL OR [Cost] >= 0");
                    table.CheckConstraint("CK_MaintenanceOrders_DateOrder", "[CompletedAt] IS NULL OR [StartedAt] IS NULL OR [CompletedAt] >= [StartedAt]");
                    table.CheckConstraint("CK_MaintenanceOrders_Priority", "[Priority] IN ('Low', 'Medium', 'High', 'Urgent')");
                    table.CheckConstraint("CK_MaintenanceOrders_Status", "[Status] IN ('Planned', 'InProgress', 'Completed', 'Cancelled')");
                    table.ForeignKey(
                        name: "FK_MaintenanceOrders_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaintenanceOrders_MaintenanceTypes_MaintenanceTypeId",
                        column: x => x.MaintenanceTypeId,
                        principalTable: "MaintenanceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaintenanceOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AssetCategories",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Computadoras y periféricos", true, "Equipo de cómputo" },
                    { 2, "Muebles y estaciones de trabajo", true, "Mobiliario" },
                    { 3, "Máquinas y herramientas de producción", true, "Equipo industrial" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Sede central", true, "Edificio principal" },
                    { 2, "Área de mantenimiento", true, "Taller" },
                    { 3, "Almacenamiento de activos", true, "Bodega" }
                });

            migrationBuilder.InsertData(
                table: "MaintenanceTypes",
                columns: new[] { "Id", "DefaultIntervalDays", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 90, "Mantenimiento programado para prevenir fallas", "Preventivo" },
                    { 2, null, "Reparación de una falla reportada", "Correctivo" },
                    { 3, 180, "Revisión general del estado del activo", "Inspección" }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "ContactName", "Email", "IsActive", "Name", "Phone", "TaxId" },
                values: new object[,]
                {
                    { 1, "San José", "María Rodríguez", "servicio@example.com", true, "Servicios Técnicos CR", "2200-1000", "3-101-000001" },
                    { 2, "Heredia", "Carlos Méndez", "taller@example.com", true, "Mantenimiento Industrial del Valle", "2200-2000", "3-101-000002" }
                });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "AcquiredOn", "AssetCode", "Brand", "CategoryId", "CreatedAt", "Description", "LocationId", "Model", "Name", "PurchaseCost", "SerialNumber", "Status", "SupplierId", "UsefulLifeMonths" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACT-0001", "Dell", 1, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Equipo portátil de oficina", 1, "Latitude 5440", "Laptop de administración", 625000m, "SN-DELL-0001", "Active", 1, 48 },
                    { 2, new DateTime(2023, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACT-0002", "Bosch", 3, new DateTime(2023, 8, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Taladro para trabajos de taller", 2, "GBM 13-2 RE", "Taladro de banco", 185000m, "SN-BOSCH-0002", "Active", 2, 72 },
                    { 3, new DateTime(2022, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACT-0003", "Oficina CR", 2, new DateTime(2022, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Módulo de recepción", 1, "Recepción 180", "Escritorio de recepción", 210000m, null, "Active", null, 120 }
                });

            migrationBuilder.InsertData(
                table: "MaintenanceOrders",
                columns: new[] { "Id", "AssetId", "CompletedAt", "Cost", "CreatedAt", "Description", "Findings", "MaintenanceTypeId", "Priority", "ScheduledFor", "StartedAt", "Status", "SupplierId", "Title", "WorkOrderCode" },
                values: new object[,]
                {
                    { 1, 1, null, 25000m, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Limpieza interna, revisión de batería y diagnóstico", null, 1, "Medium", new DateTime(2026, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Planned", 1, "Limpieza y revisión preventiva", "OT-2026-0001" },
                    { 2, 2, new DateTime(2026, 8, 20, 10, 0, 0, 0, DateTimeKind.Utc), 18000m, new DateTime(2026, 8, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Verificar broca, cableado y fijación de la herramienta", "Equipo operativo; se ajustó la base", 3, "High", new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 20, 8, 0, 0, 0, DateTimeKind.Utc), "Completed", 2, "Inspección de seguridad", "OT-2026-0002" },
                    { 3, 3, null, null, new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Ajustar uniones y comprobar estabilidad", null, 1, "Low", new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Planned", null, "Revisión de estructura", "OT-2026-0003" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_Name",
                table: "AssetCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetCode",
                table: "Assets",
                column: "AssetCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CategoryId",
                table: "Assets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_LocationId",
                table: "Assets",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SerialNumber",
                table: "Assets",
                column: "SerialNumber",
                unique: true,
                filter: "[SerialNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SupplierId",
                table: "Assets",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Name",
                table: "Locations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceOrders_AssetId",
                table: "MaintenanceOrders",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceOrders_MaintenanceTypeId",
                table: "MaintenanceOrders",
                column: "MaintenanceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceOrders_SupplierId",
                table: "MaintenanceOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceOrders_WorkOrderCode",
                table: "MaintenanceOrders",
                column: "WorkOrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTypes_Name",
                table: "MaintenanceTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_TaxId",
                table: "Suppliers",
                column: "TaxId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenanceOrders");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "MaintenanceTypes");

            migrationBuilder.DropTable(
                name: "AssetCategories");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
