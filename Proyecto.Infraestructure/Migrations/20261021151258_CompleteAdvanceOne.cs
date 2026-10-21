using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Proyecto.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class CompleteAdvanceOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_MaintenanceOrders_Status",
                table: "MaintenanceOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Assets_Status",
                table: "Assets");

            migrationBuilder.Sql(
                """
                UPDATE [Assets]
                SET [Status] = CASE [Status]
                    WHEN 'UnderMaintenance' THEN 'En Mantenimiento'
                    WHEN 'Retired' THEN 'Dado de Baja'
                    ELSE 'Operativo'
                END;
                """);

            migrationBuilder.AddColumn<string>(
                name: "GeneralConditions",
                table: "Suppliers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "MaintenanceOrders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Solicitada",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Planned");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledFor",
                table: "MaintenanceOrders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedAt",
                table: "MaintenanceOrders",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Assets",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Operativo",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Active");

            migrationBuilder.AddColumn<string>(
                name: "Criticality",
                table: "Assets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Media");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Assets",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Assets",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UsageHours",
                table: "Assets",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PreventiveMaintenanceIntervalDays",
                table: "AssetCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SpareParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CurrentStock = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinimumStock = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpareParts", x => x.Id);
                    table.CheckConstraint("CK_SpareParts_CurrentStock", "[CurrentStock] >= 0");
                    table.CheckConstraint("CK_SpareParts_MinimumStock", "[MinimumStock] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Technicians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Identification = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technicians", x => x.Id);
                    table.CheckConstraint("CK_Technicians_HourlyRate", "[HourlyRate] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "PartSuppliers",
                columns: table => new
                {
                    SparePartId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    OfferedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstimatedDeliveryDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartSuppliers", x => new { x.SparePartId, x.SupplierId });
                    table.CheckConstraint("CK_PartSuppliers_EstimatedDeliveryDays", "[EstimatedDeliveryDays] > 0");
                    table.CheckConstraint("CK_PartSuppliers_OfferedPrice", "[OfferedPrice] > 0");
                    table.ForeignKey(
                        name: "FK_PartSuppliers_SpareParts_SparePartId",
                        column: x => x.SparePartId,
                        principalTable: "SpareParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartSuppliers_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianSpecialties",
                columns: table => new
                {
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    SpecialtyId = table.Column<int>(type: "int", nullable: false),
                    CertificationLevel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CertifiedSince = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianSpecialties", x => new { x.TechnicianId, x.SpecialtyId });
                    table.ForeignKey(
                        name: "FK_TechnicianSpecialties_Specialties_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicianSpecialties_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "PreventiveMaintenanceIntervalDays",
                value: 90);

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "PreventiveMaintenanceIntervalDays",
                value: 365);

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "PreventiveMaintenanceIntervalDays",
                value: 90);

            migrationBuilder.InsertData(
                table: "AssetCategories",
                columns: new[] { "Id", "Description", "IsActive", "Name", "PreventiveMaintenanceIntervalDays" },
                values: new object[,]
                {
                    { 4, "Equipos de protección y respuesta a emergencias", true, "Seguridad industrial", 180 },
                    { 5, "Tableros, circuitos y respaldo eléctrico", true, "Instalaciones eléctricas", 120 },
                    { 6, "Instalaciones y sistemas del edificio", true, "Infraestructura", 180 },
                    { 7, "Vehículos y equipos móviles institucionales", true, "Vehículos", 90 },
                    { 8, "Instrumentos y equipos de laboratorio", true, "Laboratorio", 180 }
                });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Criticality", "ImageUrl", "IsActive", "Status", "UsageHours" },
                values: new object[] { "Media", "https://images.example.com/assets/laptop-administracion.jpg", true, "Operativo", 1850m });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Criticality", "ImageUrl", "IsActive", "Status", "UsageHours" },
                values: new object[] { "Alta", null, true, "Fuera de Servicio", 430m });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Criticality", "ImageUrl", "IsActive", "Status", "UsageHours" },
                values: new object[] { "Baja", null, true, "Operativo", 0m });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 4, "Tableros y sistemas de respaldo", true, "Cuarto eléctrico" },
                    { 5, "Equipos de medición y ensayo", true, "Laboratorio central" },
                    { 6, "Recepción y despacho de materiales", true, "Andén de carga" },
                    { 7, "Flota y equipo móvil", true, "Patio de vehículos" },
                    { 8, "Espacio de formación técnica", true, "Aula de capacitación" }
                });

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Cost", "RequestedAt", "Status" },
                values: new object[] { null, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Solicitada" });

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CompletedAt", "Cost", "Findings", "RequestedAt", "StartedAt", "Status" },
                values: new object[] { null, null, null, new DateTime(2026, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Solicitada" });

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "RequestedAt", "Status" },
                values: new object[] { new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Solicitada" });

            migrationBuilder.InsertData(
                table: "MaintenanceTypes",
                columns: new[] { "Id", "DefaultIntervalDays", "Description", "Name" },
                values: new object[,]
                {
                    { 4, 120, "Diagnóstico y reparación de equipos y sistemas eléctricos", "Mantenimiento eléctrico" },
                    { 5, 180, "Verificación metrológica de instrumentos", "Calibración" },
                    { 6, 180, "Inspección de sistemas y equipos de seguridad", "Seguridad" },
                    { 7, 90, "Lubricación, ajuste y reemplazo de componentes mecánicos", "Mantenimiento mecánico" },
                    { 8, 365, "Conservación de instalaciones y sistemas del edificio", "Infraestructura edilicia" }
                });

            migrationBuilder.InsertData(
                table: "SpareParts",
                columns: new[] { "Id", "Code", "CurrentStock", "Description", "MinimumStock", "Name", "UnitOfMeasure" },
                values: new object[,]
                {
                    { 1, "REP-0001", 8m, "Filtro de admisión compatible con generador de emergencia", 4m, "Filtro de aire para generador", "unidad" },
                    { 2, "REP-0002", 3m, "Correa trapezoidal para maquinaria de taller; requiere reposición", 5m, "Correa de transmisión industrial", "unidad" },
                    { 3, "REP-0003", 12m, "Fusible de protección para tablero de distribución", 6m, "Fusible de potencia 100 A", "unidad" },
                    { 4, "REP-0004", 4m, "Mandril de 13 mm con llave para taladro de banco", 2m, "Mandril para taladro de banco", "unidad" },
                    { 5, "REP-0005", 6m, "Batería de reemplazo para estación portátil de administración", 3m, "Batería para laptop Latitude", "unidad" },
                    { 6, "REP-0006", 2m, "Kit de sellado para bomba principal contra incendios", 4m, "Sello mecánico para bomba", "unidad" },
                    { 7, "REP-0007", 5m, "Rodamiento de precisión para rotor de centrífuga", 2m, "Rodamiento para centrífuga", "unidad" },
                    { 8, "REP-0008", 18m, "Lubricante para montacargas y sistemas hidráulicos", 10m, "Aceite hidráulico ISO 46", "litro" },
                    { 9, "REP-0009", 2m, "Contactor para tablero eléctrico; inventario bajo", 4m, "Contactor trifásico 32 A", "unidad" },
                    { 10, "REP-0010", 9m, "Equipo de protección personal para trabajos eléctricos", 5m, "Guantes dieléctricos clase 0", "par" }
                });

            migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Instalación y mantenimiento de motores, tableros y circuitos", "Electricidad industrial" },
                    { 2, "Mantenimiento de máquinas, bombas y componentes mecánicos", "Mecánica industrial" },
                    { 3, "Sistemas de refrigeración, aire acondicionado y manejo de refrigerantes", "Refrigeración y climatización" },
                    { 4, "Diagnóstico, reparación y mantenimiento de estaciones de trabajo", "Equipo de cómputo" },
                    { 5, "Calibración y trazabilidad de instrumentos de medición", "Metrología y calibración" },
                    { 6, "Inspección de sistemas de emergencia y equipos de protección", "Seguridad ocupacional" },
                    { 7, "Mantenimiento de bombas, cilindros y sistemas hidráulicos", "Sistemas hidráulicos" },
                    { 8, "Conservación de edificios, mobiliario e instalaciones", "Infraestructura edilicia" }
                });

            migrationBuilder.UpdateData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 1,
                column: "GeneralConditions",
                value: "Atención de lunes a viernes; garantía de 90 días.");

            migrationBuilder.UpdateData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 2,
                column: "GeneralConditions",
                value: "Visitas programadas y garantía de mano de obra por 6 meses.");

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "ContactName", "Email", "GeneralConditions", "IsActive", "Name", "Phone", "TaxId" },
                values: new object[,]
                {
                    { 3, "Cartago", "Laura Araya", "ventas@electromecanicos.example.com", "Entrega en 48 horas y crédito institucional a 30 días.", true, "Suministros Electromecánicos del Este", "2200-3000", "3-101-000003" },
                    { 4, "Alajuela", "Andrés Pacheco", "contacto@seguridad.example.com", "Certificados de conformidad incluidos en cada entrega.", true, "Soluciones de Seguridad Ocupacional", "2200-4000", "3-101-000004" },
                    { 5, "San José", "Paola León", "soporte@oficina.example.com", "Garantía del fabricante y soporte técnico local.", true, "Tecnología y Oficina Central", "2200-5000", "3-101-000005" },
                    { 6, "San José", "Ricardo Solís", "flotas@repuestos.example.com", "Repuestos originales; entrega a flotilla sin costo.", true, "Repuestos La Uruca", "2200-6000", "3-101-000006" },
                    { 7, "Heredia", "Verónica Rojas", "calidad@instrumentacion.example.com", "Trazabilidad metrológica y certificado de calibración.", true, "Instrumentación Científica Tica", "2200-7000", "3-101-000007" },
                    { 8, "Puntarenas", "José Brenes", "servicios@edificios.example.com", "Atención de emergencias con cobertura nacional.", true, "Servicios Integrales de Edificios", "2200-8000", "3-101-000008" },
                    { 9, "Alajuela", "Mónica Quesada", "refrigeracion@valle.example.com", "Manejo y disposición de refrigerantes certificados.", true, "Refrigeración del Valle", "2200-9000", "3-101-000009" },
                    { 10, "Limón", "Felipe Chacón", "hidraulica@sistemas.example.com", "Pruebas de presión y garantía por 12 meses.", true, "Sistemas Hidráulicos Nacionales", "2210-1000", "3-101-000010" }
                });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "FullName", "HourlyRate", "Identification", "IsAvailable", "Phone" },
                values: new object[,]
                {
                    { 1, "ana.valverde@example.com", "Ana Lucía Valverde", 8500m, "1-1020-0101", true, "8888-1001" },
                    { 2, "luis.mora@example.com", "Luis Fernando Mora", 9200m, "1-1020-0102", true, "8888-1002" }
                });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "FullName", "HourlyRate", "Identification", "Phone" },
                values: new object[] { 3, "sofia.jimenez@example.com", "Sofía Jiménez Araya", 8800m, "1-1020-0103", "8888-1003" });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "FullName", "HourlyRate", "Identification", "IsAvailable", "Phone" },
                values: new object[,]
                {
                    { 4, "diego.rojas@example.com", "Diego Rojas Campos", 10500m, "1-1020-0104", true, "8888-1004" },
                    { 5, "gabriela.vargas@example.com", "Gabriela Vargas León", 9700m, "1-1020-0105", true, "8888-1005" }
                });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "FullName", "HourlyRate", "Identification", "Phone" },
                values: new object[] { 6, "esteban.chaves@example.com", "Esteban Chaves Solano", 11000m, "1-1020-0106", "8888-1006" });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "FullName", "HourlyRate", "Identification", "IsAvailable", "Phone" },
                values: new object[,]
                {
                    { 7, "natalia.soto@example.com", "Natalia Soto Méndez", 9000m, "1-1020-0107", true, "8888-1007" },
                    { 8, "carlos.mendez@example.com", "Carlos Méndez Quesada", 9800m, "1-1020-0108", true, "8888-1008" }
                });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "FullName", "HourlyRate", "Identification", "Phone" },
                values: new object[] { 9, "irene.campos@example.com", "Irene Campos Ureña", 10200m, "1-1020-0109", "8888-1009" });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "FullName", "HourlyRate", "Identification", "IsAvailable", "Phone" },
                values: new object[] { 10, "marco.salas@example.com", "Marco Salas Rodríguez", 8700m, "1-1020-0110", true, "8888-1010" });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "AcquiredOn", "AssetCode", "Brand", "CategoryId", "CreatedAt", "Criticality", "Description", "ImageUrl", "IsActive", "LocationId", "Model", "Name", "PurchaseCost", "SerialNumber", "Status", "SupplierId", "UsageHours", "UsefulLifeMonths" },
                values: new object[,]
                {
                    { 4, new DateTime(2021, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACT-0004", "Cummins", 5, new DateTime(2021, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Crítica", "Generador diésel para respaldo eléctrico del edificio", null, true, 4, "C90D5", "Generador de emergencia", 8500000m, "SN-CUM-0400", "Operativo", 3, 740m, 180 },
                    { 5, new DateTime(2020, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACT-0005", "Grundfos", 4, new DateTime(2020, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Crítica", "Bomba principal del sistema de protección contra incendios", null, true, 3, "NK 50-200", "Bomba contra incendios", 3200000m, "SN-GRU-0500", "Operativo", 4, 2150m, 180 },
                    { 6, new DateTime(2022, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACT-0006", "Daikin", 6, new DateTime(2022, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Alta", "Unidad de climatización de precisión", null, true, 5, "FTXM36", "Aire acondicionado del laboratorio", 745000m, "SN-DAI-0600", "Operativo", 9, 9600m, 120 },
                    { 7, new DateTime(2023, 5, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACT-0007", "Toyota", 7, new DateTime(2023, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Alta", "Vehículo para traslado del personal de mantenimiento", null, true, 7, "Hiace", "Microbús de servicio", 18500000m, "SN-TOY-0700", "Operativo", 6, 3250m, 120 },
                    { 8, new DateTime(2021, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACT-0008", "Eppendorf", 8, new DateTime(2021, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Alta", "Centrífuga refrigerada para control de calidad", null, true, 5, "5804 R", "Centrífuga de laboratorio", 4250000m, "SN-EPP-0800", "Operativo", 7, 5100m, 120 },
                    { 9, new DateTime(2019, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACT-0009", "Schneider Electric", 5, new DateTime(2019, 7, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Crítica", "Tablero de distribución de baja tensión", null, true, 4, "PrismaSeT", "Tablero eléctrico principal", 2700000m, "SN-SCH-0900", "Operativo", 3, 14500m, 240 },
                    { 10, new DateTime(2022, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACT-0010", "Toyota", 3, new DateTime(2022, 4, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Alta", "Montacargas para recepción y despacho de suministros", null, true, 6, "8FBE20", "Montacargas eléctrico", 12000000m, "SN-TOY-1000", "Operativo", 10, 3890m, 180 }
                });

            migrationBuilder.InsertData(
                table: "PartSuppliers",
                columns: new[] { "SparePartId", "SupplierId", "EstimatedDeliveryDays", "OfferedPrice" },
                values: new object[,]
                {
                    { 1, 3, 2, 14500m },
                    { 1, 10, 3, 15200m },
                    { 2, 2, 1, 8700m },
                    { 2, 3, 2, 9100m },
                    { 3, 3, 2, 6300m },
                    { 3, 8, 4, 6800m },
                    { 4, 2, 1, 18400m },
                    { 5, 5, 3, 49500m },
                    { 6, 8, 2, 28200m },
                    { 6, 10, 4, 26700m },
                    { 7, 7, 5, 41500m },
                    { 8, 10, 1, 4400m },
                    { 9, 3, 2, 22000m },
                    { 9, 8, 3, 23600m },
                    { 10, 4, 2, 18500m }
                });

            migrationBuilder.InsertData(
                table: "TechnicianSpecialties",
                columns: new[] { "SpecialtyId", "TechnicianId", "CertificationLevel", "CertifiedSince" },
                values: new object[,]
                {
                    { 1, 1, "Avanzado", new DateTime(2018, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 1, "Intermedio", new DateTime(2020, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 2, "Avanzado", new DateTime(2017, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, 2, "Intermedio", new DateTime(2021, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, "Avanzado", new DateTime(2019, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 1, 4, "Intermedio", new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 4, "Intermedio", new DateTime(2023, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 5, "Avanzado", new DateTime(2016, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, 6, "Avanzado", new DateTime(2018, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 7, "Avanzado", new DateTime(2020, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 8, "Avanzado", new DateTime(2015, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, 8, "Intermedio", new DateTime(2019, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 9, "Intermedio", new DateTime(2021, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 10, "Intermedio", new DateTime(2022, 8, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, 10, "Avanzado", new DateTime(2017, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "MaintenanceOrders",
                columns: new[] { "Id", "AssetId", "CompletedAt", "Cost", "CreatedAt", "Description", "Findings", "MaintenanceTypeId", "Priority", "RequestedAt", "ScheduledFor", "StartedAt", "Status", "SupplierId", "Title", "WorkOrderCode" },
                values: new object[,]
                {
                    { 4, 4, null, null, new DateTime(2026, 9, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Prueba de carga, cambio de filtros y revisión de baterías", null, 4, "Urgent", new DateTime(2026, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Solicitada", 3, "Servicio semestral del generador", "OT-2026-0004" },
                    { 5, 5, null, null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Verificar caudal, presión y sello mecánico", null, 6, "High", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Solicitada", 4, "Prueba de presión de bomba contra incendios", "OT-2026-0005" },
                    { 6, 6, null, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Limpieza de serpentines y verificación del drenaje", null, 7, "Medium", new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Solicitada", 9, "Limpieza de unidad de precisión", "OT-2026-0006" },
                    { 7, 7, null, null, new DateTime(2026, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Revisión de frenos, neumáticos y niveles de fluidos", null, 3, "High", new DateTime(2026, 9, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Solicitada", 6, "Inspección de flotilla", "OT-2026-0007" },
                    { 8, 8, null, null, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Verificar velocidad y temperatura con patrones certificados", null, 5, "Medium", new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Solicitada", 7, "Calibración anual de centrífuga", "OT-2026-0008" },
                    { 9, 9, null, null, new DateTime(2026, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Inspección termográfica de conexiones y barras", null, 4, "Urgent", new DateTime(2026, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Solicitada", 3, "Termografía del tablero principal", "OT-2026-0009" },
                    { 10, 10, null, null, new DateTime(2026, 9, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Verificar mangueras, cilindros y nivel de aceite", null, 7, "High", new DateTime(2026, 9, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Solicitada", 10, "Revisión de sistema hidráulico", "OT-2026-0010" }
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_MaintenanceOrders_Status",
                table: "MaintenanceOrders",
                sql: "[Status] IN ('Solicitada', 'Planned', 'InProgress', 'Completed', 'Cancelled')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Assets_Criticality",
                table: "Assets",
                sql: "[Criticality] IN ('Baja', 'Media', 'Alta', 'Crítica')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Assets_Status",
                table: "Assets",
                sql: "[Status] IN ('Operativo', 'En Mantenimiento', 'Fuera de Servicio', 'Dado de Baja')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Assets_UsageHours",
                table: "Assets",
                sql: "[UsageHours] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AssetCategories_PreventiveMaintenanceIntervalDays",
                table: "AssetCategories",
                sql: "[PreventiveMaintenanceIntervalDays] IS NULL OR [PreventiveMaintenanceIntervalDays] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_PartSuppliers_SupplierId",
                table: "PartSuppliers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_Code",
                table: "SpareParts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_Name",
                table: "Specialties",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Technicians_Identification",
                table: "Technicians",
                column: "Identification",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSpecialties_SpecialtyId",
                table: "TechnicianSpecialties",
                column: "SpecialtyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartSuppliers");

            migrationBuilder.DropTable(
                name: "TechnicianSpecialties");

            migrationBuilder.DropTable(
                name: "SpareParts");

            migrationBuilder.DropTable(
                name: "Specialties");

            migrationBuilder.DropTable(
                name: "Technicians");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MaintenanceOrders_Status",
                table: "MaintenanceOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Assets_Criticality",
                table: "Assets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Assets_Status",
                table: "Assets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Assets_UsageHours",
                table: "Assets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AssetCategories_PreventiveMaintenanceIntervalDays",
                table: "AssetCategories");

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DropColumn(
                name: "GeneralConditions",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "RequestedAt",
                table: "MaintenanceOrders");

            migrationBuilder.DropColumn(
                name: "Criticality",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "UsageHours",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "PreventiveMaintenanceIntervalDays",
                table: "AssetCategories");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "MaintenanceOrders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Planned",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Solicitada");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledFor",
                table: "MaintenanceOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Assets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldDefaultValue: "Operativo");

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 3,
                column: "Status",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Cost", "Status" },
                values: new object[] { 25000m, "Planned" });

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CompletedAt", "Cost", "Findings", "StartedAt", "Status" },
                values: new object[] { new DateTime(2026, 8, 20, 10, 0, 0, 0, DateTimeKind.Utc), 18000m, "Equipo operativo; se ajustó la base", new DateTime(2026, 8, 20, 8, 0, 0, 0, DateTimeKind.Utc), "Completed" });

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 3,
                column: "Status",
                value: "Planned");

            migrationBuilder.Sql(
                """
                UPDATE [Assets]
                SET [Status] = CASE [Status]
                    WHEN 'En Mantenimiento' THEN 'UnderMaintenance'
                    WHEN 'Dado de Baja' THEN 'Retired'
                    ELSE 'Active'
                END;
                UPDATE [MaintenanceOrders]
                SET [Status] = 'Planned'
                WHERE [Status] = 'Solicitada';
                """);

            migrationBuilder.AddCheckConstraint(
                name: "CK_MaintenanceOrders_Status",
                table: "MaintenanceOrders",
                sql: "[Status] IN ('Planned', 'InProgress', 'Completed', 'Cancelled')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Assets_Status",
                table: "Assets",
                sql: "[Status] IN ('Active', 'UnderMaintenance', 'Retired')");
        }
    }
}
