using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Proyecto.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSparePartsAndItSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpareParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CurrentStock = table.Column<int>(type: "int", nullable: false),
                    MinimumStock = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpareParts", x => x.Id);
                    table.CheckConstraint("CK_SpareParts_Stock", "[CurrentStock] >= 0 AND [MinimumStock] >= 0");
                    table.CheckConstraint("CK_SpareParts_UnitOfMeasure", "[UnitOfMeasure] IN ('Unit', 'Box', 'Pack', 'Meter', 'Roll', 'Kit')");
                });

            migrationBuilder.CreateTable(
                name: "SparePartSuppliers",
                columns: table => new
                {
                    SparePartId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    OfferedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstimatedLeadTimeDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartSuppliers", x => new { x.SparePartId, x.SupplierId });
                    table.CheckConstraint("CK_SparePartSuppliers_LeadTimeDays", "[EstimatedLeadTimeDays] >= 0");
                    table.CheckConstraint("CK_SparePartSuppliers_OfferedPrice", "[OfferedPrice] >= 0");
                    table.ForeignKey(
                        name: "FK_SparePartSuppliers_SpareParts_SparePartId",
                        column: x => x.SparePartId,
                        principalTable: "SpareParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SparePartSuppliers_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Hardware de servidores, VMware e Hyper-V", "Servidores y virtualización" },
                    { 2, "Switching, routing, firewalls y cableado estructurado", "Redes y telecomunicaciones" },
                    { 3, "Laptops, equipos de escritorio y sistemas operativos", "Soporte a usuario final" },
                    { 4, "Impresoras láser, multifuncionales y escáneres", "Impresión y periféricos" },
                    { 5, "UPS, baterías, PDU y tableros eléctricos", "Sistemas eléctricos y UPS" },
                    { 6, "Aires de precisión y control ambiental", "Climatización de data center" },
                    { 7, "NAS, SAN, RAID y políticas de respaldo", "Almacenamiento y respaldo" },
                    { 8, "Hardening, parches de seguridad y firmware", "Ciberseguridad" }
                });

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "CodePrefix" },
                values: new object[] { "Servidores físicos de rack que alojan bases de datos y aplicaciones", "Servidores", "SRV" });

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name", "CodePrefix" },
                values: new object[] { "Switches, routers, firewalls y puntos de acceso", "Equipos de red", "NET" });

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name", "CodePrefix", "PreventiveThresholdHours" },
                values: new object[] { "Computadoras portátiles asignadas a colaboradores", "Laptops", "LAP", 1500 });

            migrationBuilder.InsertData(
                table: "AssetCategories",
                columns: new[] { "Id", "Description", "IsActive", "Name", "CodePrefix", "PreventiveThresholdHours" },
                values: new object[,]
                {
                    { 4, "Computadoras de escritorio y estaciones de trabajo", true, "Equipos de escritorio", "PC", 2500 },
                    { 5, "Impresoras láser y multifuncionales", true, "Impresoras", "IMP", 800 },
                    { 6, "Sistemas de alimentación ininterrumpida y PDU", true, "Energía y UPS", "UPS", 3000 },
                    { 7, "NAS, SAN y unidades de respaldo", true, "Almacenamiento", "STO", 3500 },
                    { 8, "Aires acondicionados de precisión del data center", true, "Climatización", "CLI", 1000 }
                });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Rack principal de servidores, sótano del edificio central", "Data Center - Rack A" });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Sala de energía contigua al data center", "Data Center - Sala UPS" });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Finanzas y recepción", "Oficinas piso 1" });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 4, "Ventas y recursos humanos", true, "Oficinas piso 2" },
                    { 5, "Sala de videoconferencias del piso 2", true, "Sala de reuniones" },
                    { 6, "Equipos en espera de asignación o de baja", true, "Bodega de TI" },
                    { 7, "Banco de pruebas y reparación", true, "Laboratorio de soporte" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 8, "Oficinas cerradas en 2025", "Sede antigua" });

            migrationBuilder.UpdateData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GeneralConditions", "Email", "Address", "TaxId", "Name", "Phone" },
                values: new object[] { "Soporte 24/7, respuesta en 4 horas para equipos críticos", "soporte@solucionesdc.example", "La Uruca, San José", "3-101-456701", "Soluciones Data Center CR", "2290-1100" });

            migrationBuilder.UpdateData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GeneralConditions", "Email", "Address", "TaxId", "Name", "Phone" },
                values: new object[] { "Garantía de 12 meses en instalaciones", "ventas@redesvalle.example", "Heredia centro", "3-101-456702", "Redes y Cableado del Valle", "2261-2200" });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "GeneralConditions", "Email", "Address", "IsActive", "TaxId", "Name", "ContactName", "Phone" },
                values: new object[,]
                {
                    { 3, "Crédito a 30 días", "pedidos@dcc.example", "Paseo Colón, San José", true, "3-101-456703", "Distribuidora de Cómputo Central", "Ana Jiménez", "2222-3300" },
                    { 4, "Mantenimiento de UPS con contrato anual", "servicio@energiacontinua.example", "Alajuela", true, "3-101-456704", "Energía Continua S.A.", "Luis Calderón", "2443-4400" },
                    { 5, "Visita técnica en 48 horas", "tecnicos@impresiontotal.example", "San Pedro, Montes de Oca", true, "3-101-456705", "Impresión Total CR", "Karla Soto", "2253-5500" },
                    { 6, "Revisión trimestral incluida en el contrato", "proyectos@climatech.example", "Escazú", true, "3-101-456706", "ClimaTech Ingeniería", "Jorge Ureña", "2215-6600" },
                    { 7, "Reemplazo de discos en garantía al día siguiente", "info@almseguro.example", "Sabana Norte, San José", true, "3-101-456707", "Almacenamiento Seguro TI", "Daniela Rojas", "2290-7700" }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "GeneralConditions", "Email", "Address", "TaxId", "Name", "ContactName", "Phone" },
                values: new object[] { 8, "Proveedor suspendido por incumplimiento de entregas", "ventas@tecnorepuestos.example", "Santo Domingo, Heredia", "3-101-456708", "TecnoRepuestos Heredia", "Esteban Murillo", "2260-8800" });

            migrationBuilder.UpdateData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Revisión programada al alcanzar el umbral de horas de uso de la categoría");

            migrationBuilder.UpdateData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Revisión general del estado físico y lógico del equipo");

            migrationBuilder.InsertData(
                table: "MaintenanceTypes",
                columns: new[] { "Id", "Description", "RequiredSpecialtyId", "DefaultIntervalDays", "Name" },
                values: new object[,]
                {
                    { 5, "Limpieza interna de polvo, ventiladores y filtros", null, 60, "Limpieza física" },
                    { 8, "Cambio de discos, fuentes, baterías u otras piezas", null, null, "Reemplazo de componente" }
                });

            migrationBuilder.InsertData(
                table: "MaintenanceTypes",
                columns: new[] { "Id", "Description", "RequiredSpecialtyId", "DefaultIntervalDays", "Name" },
                values: new object[] { 4, "Instalación de firmware y parches del fabricante", 8, 120, "Actualización de firmware" });

            migrationBuilder.InsertData(
                table: "MaintenanceTypes",
                columns: new[] { "Id", "Description", "IsEmergency", "RequiredSpecialtyId", "DefaultIntervalDays", "Name" },
                values: new object[] { 6, "Atención inmediata de una caída de servicio", true, 1, null, "Emergencia" });

            migrationBuilder.InsertData(
                table: "MaintenanceTypes",
                columns: new[] { "Id", "Description", "RequiredSpecialtyId", "DefaultIntervalDays", "Name" },
                values: new object[] { 7, "Ajuste de sensores y parámetros de operación", 6, 365, "Calibración" });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "IsAvailable", "FullName", "IdentificationNumber", "HourlyRate", "Phone" },
                values: new object[,]
                {
                    { 1, "laura.mora@empresa.example", true, "Laura Mora Chaves", "1-1234-0567", 18500m, "8810-1001" },
                    { 2, "diego.solano@empresa.example", true, "Diego Solano Ruiz", "1-1456-0789", 16000m, "8810-1002" }
                });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "FullName", "IdentificationNumber", "HourlyRate", "Phone" },
                values: new object[] { 3, "andrea.vargas@empresa.example", "Andrea Vargas León", "2-0678-0912", 14500m, "8810-1003" });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "IsAvailable", "FullName", "IdentificationNumber", "HourlyRate", "Phone" },
                values: new object[,]
                {
                    { 4, "jose.quesada@empresa.example", true, "José Quesada Brenes", "1-1789-0234", 12000m, "8810-1004" },
                    { 5, "mariana.castro@empresa.example", true, "Mariana Castro Núñez", "4-0234-0456", 15000m, "8810-1005" }
                });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "FullName", "IdentificationNumber", "HourlyRate", "Phone" },
                values: new object[] { 6, "ricardo.jimenez@empresa.example", "Ricardo Jiménez Mora", "1-1890-0345", 17000m, "8810-1006" });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "IsAvailable", "FullName", "IdentificationNumber", "HourlyRate", "Phone" },
                values: new object[,]
                {
                    { 7, "sofia.arias@empresa.example", true, "Sofía Arias Campos", "3-0456-0678", 13500m, "8810-1007" },
                    { 8, "pablo.herrera@empresa.example", true, "Pablo Herrera Soto", "1-1901-0456", 9500m, "8810-1008" }
                });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AssetCode", "PurchaseCost", "CreatedAt", "Criticality", "Description", "AcquiredOn", "UsageHours", "UsageHoursAtLastMaintenance", "UsefulLifeMonths", "Model", "Name", "SerialNumber" },
                values: new object[] { "SRV-0001", 6850000m, new DateTime(2022, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), "High", "Aloja SQL Server de producción", new DateTime(2022, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 8760, 7200, 60, "PowerEdge R750", "Servidor de base de datos", "DL-R750-7HX21" });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "AssetCode", "PurchaseCost", "CreatedAt", "Description", "Status", "AcquiredOn", "UsageHours", "UsageHoursAtLastMaintenance", "Brand", "UsefulLifeMonths", "Model", "Name", "SerialNumber", "SupplierId", "LocationId" },
                values: new object[] { 1, "SRV-0002", 5400000m, new DateTime(2020, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Aloja la intranet y el sistema de planillas", "UnderMaintenance", new DateTime(2020, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 15000, 12500, "HPE", 60, "ProLiant DL380 Gen10", "Servidor de aplicaciones", "HPE-DL380-CZ2241", 1, 1 });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AssetCode", "PurchaseCost", "CreatedAt", "Criticality", "Description", "AcquiredOn", "UsageHours", "UsageHoursAtLastMaintenance", "Brand", "UsefulLifeMonths", "Model", "Name", "SerialNumber", "SupplierId" },
                values: new object[] { "NET-0001", 3200000m, new DateTime(2021, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), "High", "Switch de capa 3 que interconecta los pisos", new DateTime(2021, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 12000, 9000, "Cisco", 84, "Catalyst 9300-48P", "Switch core", "CS-9300-FOC2345", 2 });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "CategoryId", "AssetCode", "PurchaseCost", "CreatedAt", "Criticality", "Description", "Status", "AcquiredOn", "UsageHours", "UsageHoursAtLastMaintenance", "Image", "Brand", "UsefulLifeMonths", "Model", "Name", "SerialNumber", "SupplierId", "LocationId" },
                values: new object[] { 4, 2, "NET-0002", 2100000m, new DateTime(2023, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "High", "Firewall de salida a internet y VPN", "Operational", new DateTime(2023, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 6500, 4000, null, "Fortinet", 60, "FortiGate 100F", "Firewall perimetral", "FG100F-TK2290", 2, 1 });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "CategoryId", "AssetCode", "PurchaseCost", "CreatedAt", "Criticality", "Description", "Status", "AcquiredOn", "UsageHours", "UsageHoursAtLastMaintenance", "Image", "Brand", "UsefulLifeMonths", "Model", "Name", "SerialNumber", "SupplierId", "LocationId" },
                values: new object[,]
                {
                    { 5, 3, "LAP-0001", 780000m, new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Medium", "Asignada a la jefatura de finanzas", "Operational", new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1200, 0, null, "Lenovo", 48, "ThinkPad T14 Gen 4", "Laptop de finanzas", "LN-T14-PF4KX2", 3, 3 },
                    { 6, 3, "LAP-0002", 690000m, new DateTime(2021, 11, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Low", "Pantalla dañada; fuera de servicio hasta recibir el repuesto", "OutOfService", new DateTime(2021, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 5200, 4100, null, "HP", 48, "EliteBook 840 G8", "Laptop de ventas", "HP-840G8-5CG123", 3, 7 },
                    { 7, 4, "PC-0001", 420000m, new DateTime(2017, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Low", "Equipo obsoleto dado de baja tras la inspección", "Decommissioned", new DateTime(2017, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 18000, 16000, null, "Dell", 60, "OptiPlex 3050", "Equipo de recepción", "DL-3050-9JK21", null, 6 },
                    { 8, 5, "IMP-0001", 950000m, new DateTime(2023, 7, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Medium", "Impresión y escaneo para ventas y RR. HH.", "Operational", new DateTime(2023, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 950, 0, null, "HP", 72, "LaserJet Enterprise M528", "Impresora multifuncional piso 2", "HP-M528-JPB345", 5, 4 },
                    { 9, 6, "UPS-0001", 4300000m, new DateTime(2022, 8, 25, 0, 0, 0, 0, DateTimeKind.Utc), "High", "Respaldo eléctrico de 10 kVA para los racks", "Operational", new DateTime(2022, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 9500, 6000, null, "APC", 96, "Smart-UPS SRT 10kVA", "UPS del data center", "APC-SRT10-AS2209", 4, 2 },
                    { 10, 7, "STO-0001", 3900000m, new DateTime(2022, 4, 7, 0, 0, 0, 0, DateTimeKind.Utc), "High", "Almacena los respaldos nocturnos; RAID degradado", "Operational", new DateTime(2022, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 11000, 8000, null, "Synology", 72, "RS3621xs+", "NAS de respaldos", "SYN-RS36-2190QZ", 7, 1 },
                    { 11, 8, "CLI-0001", 7200000m, new DateTime(2021, 3, 16, 0, 0, 0, 0, DateTimeKind.Utc), "High", "Mantiene 22 °C en la sala de servidores", "Operational", new DateTime(2021, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 14500, 14000, null, "Vertiv", 120, "Liebert PDX", "Aire acondicionado de precisión", "VRT-PDX-118274", 6, 1 }
                });

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClosedAt", "TotalCost", "CreatedAt", "Description", "Status", "Findings", "StartedAt", "Reason", "ScheduledFor", "RequestedAt", "Title" },
                values: new object[] { new DateTime(2026, 3, 9, 12, 0, 0, 0, DateTimeKind.Utc), 95000m, new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Limpieza, revisión de ventiladores y actualización de BIOS", "Completed", "Ventilador 3 con ruido; se reemplazó", new DateTime(2026, 3, 9, 8, 0, 0, 0, DateTimeKind.Utc), "Se alcanzó el umbral de 2000 horas de la categoría", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Preventivo por horas de uso" });

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ClosedAt", "TotalCost", "CreatedAt", "Description", "Status", "Findings", "StartedAt", "Reason", "ScheduledFor", "SupplierId", "RequestedAt", "MaintenanceTypeId", "Title" },
                values: new object[] { null, null, new DateTime(2026, 10, 12, 0, 0, 0, 0, DateTimeKind.Utc), "La fuente 2 no entrega energía", "InProgress", null, new DateTime(2026, 10, 14, 9, 0, 0, 0, DateTimeKind.Utc), "Alerta del iLO: PSU 2 en falla", new DateTime(2026, 10, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2026, 10, 12, 0, 0, 0, 0, DateTimeKind.Utc), 2, "Falla de fuente de poder redundante" });

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AssetId", "CreatedAt", "Description", "Reason", "Priority", "ScheduledFor", "SupplierId", "RequestedAt", "MaintenanceTypeId", "Title" },
                values: new object[] { 4, new DateTime(2026, 10, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Aplicar el parche de seguridad publicado por el fabricante", "Boletín de seguridad crítico del fabricante", "Urgent", null, 2, new DateTime(2026, 10, 20, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Actualizar FortiOS a la versión estable" });

            migrationBuilder.InsertData(
                table: "MaintenanceOrders",
                columns: new[] { "Id", "AssetId", "ClosedAt", "WorkOrderCode", "TotalCost", "CreatedAt", "Description", "Status", "Findings", "StartedAt", "Reason", "Priority", "ScheduledFor", "SupplierId", "RequestedAt", "MaintenanceTypeId", "Title" },
                values: new object[] { 9, 1, new DateTime(2026, 3, 2, 2, 0, 0, 0, DateTimeKind.Utc), "OT-2026-0009", null, new DateTime(2026, 3, 2, 1, 0, 0, 0, DateTimeKind.Utc), "Se registró por error junto con la OT-2026-0001", "Cancelled", null, null, "Revisión general", "Low", null, null, new DateTime(2026, 3, 2, 1, 0, 0, 0, DateTimeKind.Utc), 3, "Inspección duplicada" });

            migrationBuilder.InsertData(
                table: "MaintenanceOrders",
                columns: new[] { "Id", "AssetId", "ClosedAt", "WorkOrderCode", "TotalCost", "CreatedAt", "Description", "Status", "Findings", "StartedAt", "Reason", "Priority", "ScheduledFor", "SupplierId", "RequestedAt", "MaintenanceTypeId", "Title" },
                values: new object[,]
                {
                    { 4, 6, new DateTime(2026, 9, 3, 11, 0, 0, 0, DateTimeKind.Utc), "OT-2026-0004", 15000m, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "La pantalla muestra líneas verticales", "Completed", "Panel LCD dañado; se deja fuera de servicio hasta comprar el repuesto", new DateTime(2026, 9, 3, 10, 0, 0, 0, DateTimeKind.Utc), "Reporte del usuario de ventas", "Low", null, null, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "Diagnóstico de pantalla" },
                    { 5, 7, new DateTime(2026, 5, 6, 10, 0, 0, 0, DateTimeKind.Utc), "OT-2026-0005", 0m, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Evaluar si conviene repararlo o darlo de baja", "Completed", "Obsoleto; se recomienda la baja", new DateTime(2026, 5, 6, 9, 0, 0, 0, DateTimeKind.Utc), "El equipo superó su vida útil", "Low", null, null, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Inspección de equipo antiguo" },
                    { 6, 8, null, "OT-2026-0006", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Las hojas salen con manchas", "Requested", null, null, "Se alcanzó el umbral de 800 horas de la categoría", "Medium", new DateTime(2026, 10, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Limpieza del fusor y rodillos" },
                    { 7, 9, null, "OT-2026-0007", null, new DateTime(2026, 10, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Medir autonomía real con carga del data center", "Approved", null, null, "Mantenimiento preventivo del contrato anual", "High", new DateTime(2026, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, new DateTime(2026, 10, 5, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Prueba de baterías y autonomía" },
                    { 8, 11, new DateTime(2026, 7, 8, 11, 0, 0, 0, DateTimeKind.Utc), "OT-2026-0008", 120000m, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Revisión de gas refrigerante y filtros", "Completed", "Filtros saturados; se cambiaron", new DateTime(2026, 7, 8, 8, 0, 0, 0, DateTimeKind.Utc), "Revisión trimestral del contrato", "Medium", null, 6, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Inspección trimestral" },
                    { 10, 10, null, "OT-2026-0010", null, new DateTime(2026, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Disco 5 del arreglo RAID 6 en falla", "WaitingForParts", null, null, "Alerta de RAID degradado", "High", null, 7, new DateTime(2026, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), 8, "Reemplazo de disco 4 TB" }
                });

            migrationBuilder.InsertData(
                table: "TechnicianSpecialties",
                columns: new[] { "SpecialtyId", "TechnicianId", "CertifiedSince", "CertificationLevel" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2019, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Advanced" },
                    { 7, 1, new DateTime(2021, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intermediate" },
                    { 8, 1, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Basic" },
                    { 2, 2, new DateTime(2018, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Advanced" },
                    { 8, 2, new DateTime(2022, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intermediate" },
                    { 3, 3, new DateTime(2020, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Advanced" },
                    { 4, 3, new DateTime(2023, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intermediate" },
                    { 4, 4, new DateTime(2017, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Advanced" },
                    { 5, 5, new DateTime(2019, 10, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Advanced" },
                    { 6, 5, new DateTime(2022, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intermediate" },
                    { 6, 6, new DateTime(2016, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Advanced" },
                    { 3, 7, new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Basic" }
                });

            migrationBuilder.InsertData(
                table: "SpareParts",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "Name", "CurrentStock", "MinimumStock", "UnitOfMeasure" },
                values: new object[,]
                {
                    { 1, "REP-SSD-960", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Para servidores Dell PowerEdge", "Disco SSD SAS 960 GB", 1, 4, "Unit" },
                    { 2, "REP-HDD-4TB", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Para NAS Synology", "Disco HDD SATA 4 TB", 6, 4, "Unit" },
                    { 3, "REP-RAM-32G", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, "Memoria RAM DDR4 32 GB ECC", 8, 4, "Unit" },
                    { 4, "REP-PSU-800", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Compatible con HPE ProLiant Gen10", "Fuente de poder 800 W hot-plug", 3, 2, "Unit" },
                    { 5, "REP-TON-89A", new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tóner HP 89A", 0, 3, "Unit" },
                    { 6, "REP-FUS-M528", new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Kit de fusor HP M528", 2, 1, "Kit" },
                    { 7, "REP-BAT-RBC", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Banco de baterías para Smart-UPS SRT", "Batería APC RBC55", 2, 2, "Unit" },
                    { 8, "REP-SFP-10G", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Módulo SFP+ 10G SR", 10, 4, "Unit" },
                    { 9, "REP-CAB-C6A", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Caja de 305 m", "Cable UTP Cat 6A", 3, 1, "Box" },
                    { 10, "REP-FIL-PDX", new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, "Filtro de aire Liebert PDX", 4, 2, "Pack" },
                    { 11, "REP-LCD-840", new DateTime(2026, 9, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pantalla 14\" para EliteBook 840 G8", 0, 1, "Unit" }
                });

            migrationBuilder.InsertData(
                table: "SparePartSuppliers",
                columns: new[] { "SupplierId", "SparePartId", "EstimatedLeadTimeDays", "OfferedPrice" },
                values: new object[,]
                {
                    { 1, 1, 5, 285000m },
                    { 3, 1, 12, 262000m },
                    { 7, 2, 2, 98000m },
                    { 3, 3, 4, 76000m },
                    { 1, 4, 3, 190000m },
                    { 5, 5, 1, 64000m },
                    { 8, 5, 10, 52000m },
                    { 5, 6, 6, 145000m },
                    { 4, 7, 8, 410000m },
                    { 2, 8, 3, 38000m },
                    { 2, 9, 2, 165000m },
                    { 6, 10, 4, 42000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_Code",
                table: "SpareParts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SparePartSuppliers_SupplierId",
                table: "SparePartSuppliers",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SparePartSuppliers");

            migrationBuilder.DropTable(
                name: "SpareParts");

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 5);

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
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 7, 1 });

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 8, 1 });

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 8, 2 });

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "TechnicianSpecialties",
                keyColumns: new[] { "SpecialtyId", "TechnicianId" },
                keyValues: new object[] { 3, 7 });

            migrationBuilder.DeleteData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: 8);

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
                table: "Assets",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Technicians",
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
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "CodePrefix" },
                values: new object[] { "Computadoras y periféricos 1234", "Equipo de cómputo", "COMP" });

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name", "CodePrefix" },
                values: new object[] { "Muebles y estaciones de trabajo", "Mobiliario", "MOB" });

            migrationBuilder.UpdateData(
                table: "AssetCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name", "CodePrefix", "PreventiveThresholdHours" },
                values: new object[] { "Máquinas y herramientas de producción", "Equipo industrial", "IND", 500 });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AssetCode", "PurchaseCost", "CreatedAt", "Criticality", "Description", "AcquiredOn", "UsageHours", "UsageHoursAtLastMaintenance", "UsefulLifeMonths", "Model", "Name", "SerialNumber" },
                values: new object[] { "ACT-0001", 625000m, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Medium", "Equipo portátil de oficina", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1800, 0, 48, "Latitude 5440", "Laptop de administración", "SN-DELL-0001" });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "AssetCode", "PurchaseCost", "CreatedAt", "Description", "Status", "AcquiredOn", "UsageHours", "UsageHoursAtLastMaintenance", "Brand", "UsefulLifeMonths", "Model", "Name", "SerialNumber", "SupplierId", "LocationId" },
                values: new object[] { 3, "ACT-0002", 185000m, new DateTime(2023, 8, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Taladro para trabajos de taller", "Operational", new DateTime(2023, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 950, 600, "Bosch", 72, "GBM 13-2 RE", "Taladro de banco", "SN-BOSCH-0002", 2, 2 });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AssetCode", "PurchaseCost", "CreatedAt", "Criticality", "Description", "AcquiredOn", "UsageHours", "UsageHoursAtLastMaintenance", "Brand", "UsefulLifeMonths", "Model", "Name", "SerialNumber", "SupplierId" },
                values: new object[] { "ACT-0003", 210000m, new DateTime(2022, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Low", "Módulo de recepción", new DateTime(2022, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "Oficina CR", 120, "Recepción 180", "Escritorio de recepción", null, null });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Sede central", "Edificio principal" });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Área de mantenimiento", "Taller" });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Almacenamiento de activos", "Bodega" });

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClosedAt", "TotalCost", "CreatedAt", "Description", "Status", "Findings", "StartedAt", "Reason", "ScheduledFor", "RequestedAt", "Title" },
                values: new object[] { null, 25000m, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Limpieza interna, revisión de batería y diagnóstico", "Requested", null, null, "Mantenimiento preventivo según el intervalo de 90 días", new DateTime(2026, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Limpieza y revisión preventiva" });

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ClosedAt", "TotalCost", "CreatedAt", "Description", "Status", "Findings", "StartedAt", "Reason", "ScheduledFor", "SupplierId", "RequestedAt", "MaintenanceTypeId", "Title" },
                values: new object[] { new DateTime(2026, 8, 20, 10, 0, 0, 0, DateTimeKind.Utc), 18000m, new DateTime(2026, 8, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Verificar broca, cableado y fijación de la herramienta", "Completed", "Equipo operativo; se ajustó la base", new DateTime(2026, 8, 20, 8, 0, 0, 0, DateTimeKind.Utc), "Inspección periódica de seguridad del equipo de taller", new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2026, 8, 10, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Inspección de seguridad" });

            migrationBuilder.UpdateData(
                table: "MaintenanceOrders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AssetId", "CreatedAt", "Description", "Reason", "Priority", "ScheduledFor", "SupplierId", "RequestedAt", "MaintenanceTypeId", "Title" },
                values: new object[] { 3, new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Ajustar uniones y comprobar estabilidad", "Se reportan uniones flojas en el módulo de recepción", "Low", new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Revisión de estructura" });

            migrationBuilder.UpdateData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Mantenimiento programado para prevenir fallas");

            migrationBuilder.UpdateData(
                table: "MaintenanceTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Revisión general del estado del activo");

            migrationBuilder.UpdateData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GeneralConditions", "Email", "Address", "TaxId", "Name", "Phone" },
                values: new object[] { null, "servicio@example.com", "San José", "3-101-000001", "Servicios Técnicos CR", "2200-1000" });

            migrationBuilder.UpdateData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GeneralConditions", "Email", "Address", "TaxId", "Name", "Phone" },
                values: new object[] { null, "taller@example.com", "Heredia", "3-101-000002", "Mantenimiento Industrial del Valle", "2200-2000" });
        }
    }
}
