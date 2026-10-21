using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;

namespace Proyecto.Infraestructure.Data.Seed;

public static class CatalogSeeder
{
    public static ModelBuilder SeedCatalogs(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssetCategory>().HasData(
            new AssetCategory { Id = SeedIds.AssetCategories.ComputerEquipment, Name = "Equipo de cómputo", Description = "Computadoras y periféricos", PreventiveMaintenanceIntervalDays = 90 },
            new AssetCategory { Id = SeedIds.AssetCategories.Furniture, Name = "Mobiliario", Description = "Muebles y estaciones de trabajo", PreventiveMaintenanceIntervalDays = 365 },
            new AssetCategory { Id = SeedIds.AssetCategories.IndustrialEquipment, Name = "Equipo industrial", Description = "Máquinas y herramientas de producción", PreventiveMaintenanceIntervalDays = 90 },
            new AssetCategory { Id = SeedIds.AssetCategories.Safety, Name = "Seguridad industrial", Description = "Equipos de protección y respuesta a emergencias", PreventiveMaintenanceIntervalDays = 180 },
            new AssetCategory { Id = SeedIds.AssetCategories.Electrical, Name = "Instalaciones eléctricas", Description = "Tableros, circuitos y respaldo eléctrico", PreventiveMaintenanceIntervalDays = 120 },
            new AssetCategory { Id = SeedIds.AssetCategories.BuildingInfrastructure, Name = "Infraestructura", Description = "Instalaciones y sistemas del edificio", PreventiveMaintenanceIntervalDays = 180 },
            new AssetCategory { Id = SeedIds.AssetCategories.Vehicles, Name = "Vehículos", Description = "Vehículos y equipos móviles institucionales", PreventiveMaintenanceIntervalDays = 90 },
            new AssetCategory { Id = SeedIds.AssetCategories.Laboratory, Name = "Laboratorio", Description = "Instrumentos y equipos de laboratorio", PreventiveMaintenanceIntervalDays = 180 });

        modelBuilder.Entity<Location>().HasData(
            new Location { Id = SeedIds.Locations.MainBuilding, Name = "Edificio principal", Description = "Sede central" },
            new Location { Id = SeedIds.Locations.Workshop, Name = "Taller", Description = "Área de mantenimiento" },
            new Location { Id = SeedIds.Locations.Warehouse, Name = "Bodega", Description = "Almacenamiento de activos" },
            new Location { Id = SeedIds.Locations.ElectricalRoom, Name = "Cuarto eléctrico", Description = "Tableros y sistemas de respaldo" },
            new Location { Id = SeedIds.Locations.Laboratory, Name = "Laboratorio central", Description = "Equipos de medición y ensayo" },
            new Location { Id = SeedIds.Locations.LoadingDock, Name = "Andén de carga", Description = "Recepción y despacho de materiales" },
            new Location { Id = SeedIds.Locations.VehicleYard, Name = "Patio de vehículos", Description = "Flota y equipo móvil" },
            new Location { Id = SeedIds.Locations.TrainingRoom, Name = "Aula de capacitación", Description = "Espacio de formación técnica" });

        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = SeedIds.Suppliers.TechnicalServices, Name = "Servicios Técnicos CR", TaxId = "3-101-000001", ContactName = "María Rodríguez", Phone = "2200-1000", Email = "servicio@example.com", Address = "San José", GeneralConditions = "Atención de lunes a viernes; garantía de 90 días." },
            new Supplier { Id = SeedIds.Suppliers.IndustrialMaintenance, Name = "Mantenimiento Industrial del Valle", TaxId = "3-101-000002", ContactName = "Carlos Méndez", Phone = "2200-2000", Email = "taller@example.com", Address = "Heredia", GeneralConditions = "Visitas programadas y garantía de mano de obra por 6 meses." },
            new Supplier { Id = SeedIds.Suppliers.ElectromecanicSupply, Name = "Suministros Electromecánicos del Este", TaxId = "3-101-000003", ContactName = "Laura Araya", Phone = "2200-3000", Email = "ventas@electromecanicos.example.com", Address = "Cartago", GeneralConditions = "Entrega en 48 horas y crédito institucional a 30 días." },
            new Supplier { Id = SeedIds.Suppliers.SafetySolutions, Name = "Soluciones de Seguridad Ocupacional", TaxId = "3-101-000004", ContactName = "Andrés Pacheco", Phone = "2200-4000", Email = "contacto@seguridad.example.com", Address = "Alajuela", GeneralConditions = "Certificados de conformidad incluidos en cada entrega." },
            new Supplier { Id = SeedIds.Suppliers.OfficeEquipment, Name = "Tecnología y Oficina Central", TaxId = "3-101-000005", ContactName = "Paola León", Phone = "2200-5000", Email = "soporte@oficina.example.com", Address = "San José", GeneralConditions = "Garantía del fabricante y soporte técnico local." },
            new Supplier { Id = SeedIds.Suppliers.AutomotiveParts, Name = "Repuestos La Uruca", TaxId = "3-101-000006", ContactName = "Ricardo Solís", Phone = "2200-6000", Email = "flotas@repuestos.example.com", Address = "San José", GeneralConditions = "Repuestos originales; entrega a flotilla sin costo." },
            new Supplier { Id = SeedIds.Suppliers.LaboratorySupply, Name = "Instrumentación Científica Tica", TaxId = "3-101-000007", ContactName = "Verónica Rojas", Phone = "2200-7000", Email = "calidad@instrumentacion.example.com", Address = "Heredia", GeneralConditions = "Trazabilidad metrológica y certificado de calibración." },
            new Supplier { Id = SeedIds.Suppliers.BuildingServices, Name = "Servicios Integrales de Edificios", TaxId = "3-101-000008", ContactName = "José Brenes", Phone = "2200-8000", Email = "servicios@edificios.example.com", Address = "Puntarenas", GeneralConditions = "Atención de emergencias con cobertura nacional." },
            new Supplier { Id = SeedIds.Suppliers.RefrigerationWorks, Name = "Refrigeración del Valle", TaxId = "3-101-000009", ContactName = "Mónica Quesada", Phone = "2200-9000", Email = "refrigeracion@valle.example.com", Address = "Alajuela", GeneralConditions = "Manejo y disposición de refrigerantes certificados." },
            new Supplier { Id = SeedIds.Suppliers.HydraulicSystems, Name = "Sistemas Hidráulicos Nacionales", TaxId = "3-101-000010", ContactName = "Felipe Chacón", Phone = "2210-1000", Email = "hidraulica@sistemas.example.com", Address = "Limón", GeneralConditions = "Pruebas de presión y garantía por 12 meses." });

        modelBuilder.Entity<Asset>().HasData(
            new Asset { Id = SeedIds.Assets.AdministrationLaptop, AssetCode = "ACT-0001", Name = "Laptop de administración", Description = "Equipo portátil de oficina", Brand = "Dell", Model = "Latitude 5440", SerialNumber = "SN-DELL-0001", AcquiredOn = new DateTime(2024, 1, 15), PurchaseCost = 625000m, UsefulLifeMonths = 48, UsageHours = 1850m, Criticality = "Media", ImageUrl = "https://images.example.com/assets/laptop-administracion.jpg", CategoryId = SeedIds.AssetCategories.ComputerEquipment, LocationId = SeedIds.Locations.MainBuilding, SupplierId = SeedIds.Suppliers.TechnicalServices, CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Asset { Id = SeedIds.Assets.BenchDrill, AssetCode = "ACT-0002", Name = "Taladro de banco", Description = "Taladro para trabajos de taller", Brand = "Bosch", Model = "GBM 13-2 RE", SerialNumber = "SN-BOSCH-0002", AcquiredOn = new DateTime(2023, 8, 10), PurchaseCost = 185000m, UsefulLifeMonths = 72, UsageHours = 430m, Criticality = "Alta", Status = "Fuera de Servicio", CategoryId = SeedIds.AssetCategories.IndustrialEquipment, LocationId = SeedIds.Locations.Workshop, SupplierId = SeedIds.Suppliers.IndustrialMaintenance, CreatedAt = new DateTime(2023, 8, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Asset { Id = SeedIds.Assets.ReceptionDesk, AssetCode = "ACT-0003", Name = "Escritorio de recepción", Description = "Módulo de recepción", Brand = "Oficina CR", Model = "Recepción 180", AcquiredOn = new DateTime(2022, 3, 2), PurchaseCost = 210000m, UsefulLifeMonths = 120, UsageHours = 0m, Criticality = "Baja", CategoryId = SeedIds.AssetCategories.Furniture, LocationId = SeedIds.Locations.MainBuilding, CreatedAt = new DateTime(2022, 3, 2, 0, 0, 0, DateTimeKind.Utc) },
            new Asset { Id = 4, AssetCode = "ACT-0004", Name = "Generador de emergencia", Description = "Generador diésel para respaldo eléctrico del edificio", Brand = "Cummins", Model = "C90D5", SerialNumber = "SN-CUM-0400", AcquiredOn = new DateTime(2021, 6, 12), PurchaseCost = 8500000m, UsefulLifeMonths = 180, UsageHours = 740m, Criticality = "Crítica", CategoryId = SeedIds.AssetCategories.Electrical, LocationId = SeedIds.Locations.ElectricalRoom, SupplierId = SeedIds.Suppliers.ElectromecanicSupply, CreatedAt = new DateTime(2021, 6, 12, 0, 0, 0, DateTimeKind.Utc) },
            new Asset { Id = 5, AssetCode = "ACT-0005", Name = "Bomba contra incendios", Description = "Bomba principal del sistema de protección contra incendios", Brand = "Grundfos", Model = "NK 50-200", SerialNumber = "SN-GRU-0500", AcquiredOn = new DateTime(2020, 2, 20), PurchaseCost = 3200000m, UsefulLifeMonths = 180, UsageHours = 2150m, Criticality = "Crítica", CategoryId = SeedIds.AssetCategories.Safety, LocationId = SeedIds.Locations.Warehouse, SupplierId = SeedIds.Suppliers.SafetySolutions, CreatedAt = new DateTime(2020, 2, 20, 0, 0, 0, DateTimeKind.Utc) },
            new Asset { Id = 6, AssetCode = "ACT-0006", Name = "Aire acondicionado del laboratorio", Description = "Unidad de climatización de precisión", Brand = "Daikin", Model = "FTXM36", SerialNumber = "SN-DAI-0600", AcquiredOn = new DateTime(2022, 11, 4), PurchaseCost = 745000m, UsefulLifeMonths = 120, UsageHours = 9600m, Criticality = "Alta", CategoryId = SeedIds.AssetCategories.BuildingInfrastructure, LocationId = SeedIds.Locations.Laboratory, SupplierId = SeedIds.Suppliers.RefrigerationWorks, CreatedAt = new DateTime(2022, 11, 4, 0, 0, 0, DateTimeKind.Utc) },
            new Asset { Id = 7, AssetCode = "ACT-0007", Name = "Microbús de servicio", Description = "Vehículo para traslado del personal de mantenimiento", Brand = "Toyota", Model = "Hiace", SerialNumber = "SN-TOY-0700", AcquiredOn = new DateTime(2023, 5, 9), PurchaseCost = 18500000m, UsefulLifeMonths = 120, UsageHours = 3250m, Criticality = "Alta", CategoryId = SeedIds.AssetCategories.Vehicles, LocationId = SeedIds.Locations.VehicleYard, SupplierId = SeedIds.Suppliers.AutomotiveParts, CreatedAt = new DateTime(2023, 5, 9, 0, 0, 0, DateTimeKind.Utc) },
            new Asset { Id = 8, AssetCode = "ACT-0008", Name = "Centrífuga de laboratorio", Description = "Centrífuga refrigerada para control de calidad", Brand = "Eppendorf", Model = "5804 R", SerialNumber = "SN-EPP-0800", AcquiredOn = new DateTime(2021, 9, 1), PurchaseCost = 4250000m, UsefulLifeMonths = 120, UsageHours = 5100m, Criticality = "Alta", CategoryId = SeedIds.AssetCategories.Laboratory, LocationId = SeedIds.Locations.Laboratory, SupplierId = SeedIds.Suppliers.LaboratorySupply, CreatedAt = new DateTime(2021, 9, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Asset { Id = 9, AssetCode = "ACT-0009", Name = "Tablero eléctrico principal", Description = "Tablero de distribución de baja tensión", Brand = "Schneider Electric", Model = "PrismaSeT", SerialNumber = "SN-SCH-0900", AcquiredOn = new DateTime(2019, 7, 15), PurchaseCost = 2700000m, UsefulLifeMonths = 240, UsageHours = 14500m, Criticality = "Crítica", CategoryId = SeedIds.AssetCategories.Electrical, LocationId = SeedIds.Locations.ElectricalRoom, SupplierId = SeedIds.Suppliers.ElectromecanicSupply, CreatedAt = new DateTime(2019, 7, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Asset { Id = 10, AssetCode = "ACT-0010", Name = "Montacargas eléctrico", Description = "Montacargas para recepción y despacho de suministros", Brand = "Toyota", Model = "8FBE20", SerialNumber = "SN-TOY-1000", AcquiredOn = new DateTime(2022, 4, 18), PurchaseCost = 12000000m, UsefulLifeMonths = 180, UsageHours = 3890m, Criticality = "Alta", CategoryId = SeedIds.AssetCategories.IndustrialEquipment, LocationId = SeedIds.Locations.LoadingDock, SupplierId = SeedIds.Suppliers.HydraulicSystems, CreatedAt = new DateTime(2022, 4, 18, 0, 0, 0, DateTimeKind.Utc) });

        modelBuilder.Entity<MaintenanceType>().HasData(
            new MaintenanceType { Id = SeedIds.MaintenanceTypes.Preventive, Name = "Preventivo", Description = "Mantenimiento programado para prevenir fallas", DefaultIntervalDays = 90 },
            new MaintenanceType { Id = SeedIds.MaintenanceTypes.Corrective, Name = "Correctivo", Description = "Reparación de una falla reportada" },
            new MaintenanceType { Id = SeedIds.MaintenanceTypes.Inspection, Name = "Inspección", Description = "Revisión general del estado del activo", DefaultIntervalDays = 180 },
            new MaintenanceType { Id = SeedIds.MaintenanceTypes.Electrical, Name = "Mantenimiento eléctrico", Description = "Diagnóstico y reparación de equipos y sistemas eléctricos", DefaultIntervalDays = 120 },
            new MaintenanceType { Id = SeedIds.MaintenanceTypes.Calibration, Name = "Calibración", Description = "Verificación metrológica de instrumentos", DefaultIntervalDays = 180 },
            new MaintenanceType { Id = SeedIds.MaintenanceTypes.Safety, Name = "Seguridad", Description = "Inspección de sistemas y equipos de seguridad", DefaultIntervalDays = 180 },
            new MaintenanceType { Id = SeedIds.MaintenanceTypes.Mechanical, Name = "Mantenimiento mecánico", Description = "Lubricación, ajuste y reemplazo de componentes mecánicos", DefaultIntervalDays = 90 },
            new MaintenanceType { Id = SeedIds.MaintenanceTypes.Building, Name = "Infraestructura edilicia", Description = "Conservación de instalaciones y sistemas del edificio", DefaultIntervalDays = 365 });

        modelBuilder.Entity<MaintenanceOrder>().HasData(
            new MaintenanceOrder { Id = SeedIds.MaintenanceOrders.LaptopPreventiveMaintenance, WorkOrderCode = "OT-2026-0001", AssetId = SeedIds.Assets.AdministrationLaptop, MaintenanceTypeId = SeedIds.MaintenanceTypes.Preventive, SupplierId = SeedIds.Suppliers.TechnicalServices, Title = "Limpieza y revisión preventiva", Description = "Limpieza interna, revisión de batería y diagnóstico", Priority = "Medium", Status = "Solicitada", RequestedAt = new DateTime(2026, 9, 1), ScheduledFor = new DateTime(2026, 10, 15), CreatedAt = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc) },
            new MaintenanceOrder { Id = SeedIds.MaintenanceOrders.DrillSafetyInspection, WorkOrderCode = "OT-2026-0002", AssetId = SeedIds.Assets.BenchDrill, MaintenanceTypeId = SeedIds.MaintenanceTypes.Inspection, SupplierId = SeedIds.Suppliers.IndustrialMaintenance, Title = "Inspección de seguridad", Description = "Verificar broca, cableado y fijación de la herramienta", Priority = "High", Status = "Solicitada", RequestedAt = new DateTime(2026, 8, 10), ScheduledFor = new DateTime(2026, 8, 20), CreatedAt = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc) },
            new MaintenanceOrder { Id = SeedIds.MaintenanceOrders.DeskStructureReview, WorkOrderCode = "OT-2026-0003", AssetId = SeedIds.Assets.ReceptionDesk, MaintenanceTypeId = SeedIds.MaintenanceTypes.Preventive, Title = "Revisión de estructura", Description = "Ajustar uniones y comprobar estabilidad", Priority = "Low", Status = "Solicitada", RequestedAt = new DateTime(2026, 9, 5), ScheduledFor = new DateTime(2026, 11, 5), CreatedAt = new DateTime(2026, 9, 5, 0, 0, 0, DateTimeKind.Utc) },
            new MaintenanceOrder { Id = 4, WorkOrderCode = "OT-2026-0004", AssetId = SeedIds.Assets.EmergencyGenerator, MaintenanceTypeId = SeedIds.MaintenanceTypes.Electrical, SupplierId = SeedIds.Suppliers.ElectromecanicSupply, Title = "Servicio semestral del generador", Description = "Prueba de carga, cambio de filtros y revisión de baterías", Priority = "Urgent", Status = "Solicitada", RequestedAt = new DateTime(2026, 9, 12), ScheduledFor = new DateTime(2026, 10, 25), CreatedAt = new DateTime(2026, 9, 12, 0, 0, 0, DateTimeKind.Utc) },
            new MaintenanceOrder { Id = 5, WorkOrderCode = "OT-2026-0005", AssetId = SeedIds.Assets.FirePump, MaintenanceTypeId = SeedIds.MaintenanceTypes.Safety, SupplierId = SeedIds.Suppliers.SafetySolutions, Title = "Prueba de presión de bomba contra incendios", Description = "Verificar caudal, presión y sello mecánico", Priority = "High", Status = "Solicitada", RequestedAt = new DateTime(2026, 9, 17), CreatedAt = new DateTime(2026, 9, 17, 0, 0, 0, DateTimeKind.Utc) },
            new MaintenanceOrder { Id = 6, WorkOrderCode = "OT-2026-0006", AssetId = SeedIds.Assets.AirConditioner, MaintenanceTypeId = SeedIds.MaintenanceTypes.Mechanical, SupplierId = SeedIds.Suppliers.RefrigerationWorks, Title = "Limpieza de unidad de precisión", Description = "Limpieza de serpentines y verificación del drenaje", Priority = "Medium", Status = "Solicitada", RequestedAt = new DateTime(2026, 9, 21), ScheduledFor = new DateTime(2026, 10, 30), CreatedAt = new DateTime(2026, 9, 21, 0, 0, 0, DateTimeKind.Utc) },
            new MaintenanceOrder { Id = 7, WorkOrderCode = "OT-2026-0007", AssetId = SeedIds.Assets.CargoVan, MaintenanceTypeId = SeedIds.MaintenanceTypes.Inspection, SupplierId = SeedIds.Suppliers.AutomotiveParts, Title = "Inspección de flotilla", Description = "Revisión de frenos, neumáticos y niveles de fluidos", Priority = "High", Status = "Solicitada", RequestedAt = new DateTime(2026, 9, 23), CreatedAt = new DateTime(2026, 9, 23, 0, 0, 0, DateTimeKind.Utc) },
            new MaintenanceOrder { Id = 8, WorkOrderCode = "OT-2026-0008", AssetId = SeedIds.Assets.Centrifuge, MaintenanceTypeId = SeedIds.MaintenanceTypes.Calibration, SupplierId = SeedIds.Suppliers.LaboratorySupply, Title = "Calibración anual de centrífuga", Description = "Verificar velocidad y temperatura con patrones certificados", Priority = "Medium", Status = "Solicitada", RequestedAt = new DateTime(2026, 9, 25), ScheduledFor = new DateTime(2026, 11, 10), CreatedAt = new DateTime(2026, 9, 25, 0, 0, 0, DateTimeKind.Utc) },
            new MaintenanceOrder { Id = 9, WorkOrderCode = "OT-2026-0009", AssetId = SeedIds.Assets.MainSwitchboard, MaintenanceTypeId = SeedIds.MaintenanceTypes.Electrical, SupplierId = SeedIds.Suppliers.ElectromecanicSupply, Title = "Termografía del tablero principal", Description = "Inspección termográfica de conexiones y barras", Priority = "Urgent", Status = "Solicitada", RequestedAt = new DateTime(2026, 9, 27), CreatedAt = new DateTime(2026, 9, 27, 0, 0, 0, DateTimeKind.Utc) },
            new MaintenanceOrder { Id = 10, WorkOrderCode = "OT-2026-0010", AssetId = SeedIds.Assets.Forklift, MaintenanceTypeId = SeedIds.MaintenanceTypes.Mechanical, SupplierId = SeedIds.Suppliers.HydraulicSystems, Title = "Revisión de sistema hidráulico", Description = "Verificar mangueras, cilindros y nivel de aceite", Priority = "High", Status = "Solicitada", RequestedAt = new DateTime(2026, 9, 29), ScheduledFor = new DateTime(2026, 10, 28), CreatedAt = new DateTime(2026, 9, 29, 0, 0, 0, DateTimeKind.Utc) });

        modelBuilder.Entity<Technician>().HasData(
            new Technician { Id = 1, FullName = "Ana Lucía Valverde", Identification = "1-1020-0101", Email = "ana.valverde@example.com", Phone = "8888-1001", IsAvailable = true, HourlyRate = 8500m },
            new Technician { Id = 2, FullName = "Luis Fernando Mora", Identification = "1-1020-0102", Email = "luis.mora@example.com", Phone = "8888-1002", IsAvailable = true, HourlyRate = 9200m },
            new Technician { Id = 3, FullName = "Sofía Jiménez Araya", Identification = "1-1020-0103", Email = "sofia.jimenez@example.com", Phone = "8888-1003", IsAvailable = false, HourlyRate = 8800m },
            new Technician { Id = 4, FullName = "Diego Rojas Campos", Identification = "1-1020-0104", Email = "diego.rojas@example.com", Phone = "8888-1004", IsAvailable = true, HourlyRate = 10500m },
            new Technician { Id = 5, FullName = "Gabriela Vargas León", Identification = "1-1020-0105", Email = "gabriela.vargas@example.com", Phone = "8888-1005", IsAvailable = true, HourlyRate = 9700m },
            new Technician { Id = 6, FullName = "Esteban Chaves Solano", Identification = "1-1020-0106", Email = "esteban.chaves@example.com", Phone = "8888-1006", IsAvailable = false, HourlyRate = 11000m },
            new Technician { Id = 7, FullName = "Natalia Soto Méndez", Identification = "1-1020-0107", Email = "natalia.soto@example.com", Phone = "8888-1007", IsAvailable = true, HourlyRate = 9000m },
            new Technician { Id = 8, FullName = "Carlos Méndez Quesada", Identification = "1-1020-0108", Email = "carlos.mendez@example.com", Phone = "8888-1008", IsAvailable = true, HourlyRate = 9800m },
            new Technician { Id = 9, FullName = "Irene Campos Ureña", Identification = "1-1020-0109", Email = "irene.campos@example.com", Phone = "8888-1009", IsAvailable = false, HourlyRate = 10200m },
            new Technician { Id = 10, FullName = "Marco Salas Rodríguez", Identification = "1-1020-0110", Email = "marco.salas@example.com", Phone = "8888-1010", IsAvailable = true, HourlyRate = 8700m });

        modelBuilder.Entity<Specialty>().HasData(
            new Specialty { Id = 1, Name = "Electricidad industrial", Description = "Instalación y mantenimiento de motores, tableros y circuitos" },
            new Specialty { Id = 2, Name = "Mecánica industrial", Description = "Mantenimiento de máquinas, bombas y componentes mecánicos" },
            new Specialty { Id = 3, Name = "Refrigeración y climatización", Description = "Sistemas de refrigeración, aire acondicionado y manejo de refrigerantes" },
            new Specialty { Id = 4, Name = "Equipo de cómputo", Description = "Diagnóstico, reparación y mantenimiento de estaciones de trabajo" },
            new Specialty { Id = 5, Name = "Metrología y calibración", Description = "Calibración y trazabilidad de instrumentos de medición" },
            new Specialty { Id = 6, Name = "Seguridad ocupacional", Description = "Inspección de sistemas de emergencia y equipos de protección" },
            new Specialty { Id = 7, Name = "Sistemas hidráulicos", Description = "Mantenimiento de bombas, cilindros y sistemas hidráulicos" },
            new Specialty { Id = 8, Name = "Infraestructura edilicia", Description = "Conservación de edificios, mobiliario e instalaciones" });

        modelBuilder.Entity<TechnicianSpecialty>().HasData(
            new TechnicianSpecialty { TechnicianId = 1, SpecialtyId = 1, CertificationLevel = "Avanzado", CertifiedSince = new DateTime(2018, 3, 1) },
            new TechnicianSpecialty { TechnicianId = 1, SpecialtyId = 6, CertificationLevel = "Intermedio", CertifiedSince = new DateTime(2020, 6, 15) },
            new TechnicianSpecialty { TechnicianId = 2, SpecialtyId = 2, CertificationLevel = "Avanzado", CertifiedSince = new DateTime(2017, 8, 10) },
            new TechnicianSpecialty { TechnicianId = 2, SpecialtyId = 7, CertificationLevel = "Intermedio", CertifiedSince = new DateTime(2021, 2, 1) },
            new TechnicianSpecialty { TechnicianId = 3, SpecialtyId = 3, CertificationLevel = "Avanzado", CertifiedSince = new DateTime(2019, 5, 20) },
            new TechnicianSpecialty { TechnicianId = 4, SpecialtyId = 1, CertificationLevel = "Intermedio", CertifiedSince = new DateTime(2022, 1, 10) },
            new TechnicianSpecialty { TechnicianId = 4, SpecialtyId = 2, CertificationLevel = "Intermedio", CertifiedSince = new DateTime(2023, 4, 3) },
            new TechnicianSpecialty { TechnicianId = 5, SpecialtyId = 5, CertificationLevel = "Avanzado", CertifiedSince = new DateTime(2016, 9, 14) },
            new TechnicianSpecialty { TechnicianId = 6, SpecialtyId = 7, CertificationLevel = "Avanzado", CertifiedSince = new DateTime(2018, 11, 5) },
            new TechnicianSpecialty { TechnicianId = 7, SpecialtyId = 4, CertificationLevel = "Avanzado", CertifiedSince = new DateTime(2020, 2, 12) },
            new TechnicianSpecialty { TechnicianId = 8, SpecialtyId = 6, CertificationLevel = "Avanzado", CertifiedSince = new DateTime(2015, 7, 1) },
            new TechnicianSpecialty { TechnicianId = 8, SpecialtyId = 8, CertificationLevel = "Intermedio", CertifiedSince = new DateTime(2019, 10, 1) },
            new TechnicianSpecialty { TechnicianId = 9, SpecialtyId = 3, CertificationLevel = "Intermedio", CertifiedSince = new DateTime(2021, 1, 20) },
            new TechnicianSpecialty { TechnicianId = 10, SpecialtyId = 8, CertificationLevel = "Avanzado", CertifiedSince = new DateTime(2017, 4, 18) },
            new TechnicianSpecialty { TechnicianId = 10, SpecialtyId = 2, CertificationLevel = "Intermedio", CertifiedSince = new DateTime(2022, 8, 2) });

        modelBuilder.Entity<SparePart>().HasData(
            new SparePart { Id = 1, Code = "REP-0001", Name = "Filtro de aire para generador", UnitOfMeasure = "unidad", CurrentStock = 8, MinimumStock = 4, Description = "Filtro de admisión compatible con generador de emergencia" },
            new SparePart { Id = 2, Code = "REP-0002", Name = "Correa de transmisión industrial", UnitOfMeasure = "unidad", CurrentStock = 3, MinimumStock = 5, Description = "Correa trapezoidal para maquinaria de taller; requiere reposición" },
            new SparePart { Id = 3, Code = "REP-0003", Name = "Fusible de potencia 100 A", UnitOfMeasure = "unidad", CurrentStock = 12, MinimumStock = 6, Description = "Fusible de protección para tablero de distribución" },
            new SparePart { Id = 4, Code = "REP-0004", Name = "Mandril para taladro de banco", UnitOfMeasure = "unidad", CurrentStock = 4, MinimumStock = 2, Description = "Mandril de 13 mm con llave para taladro de banco" },
            new SparePart { Id = 5, Code = "REP-0005", Name = "Batería para laptop Latitude", UnitOfMeasure = "unidad", CurrentStock = 6, MinimumStock = 3, Description = "Batería de reemplazo para estación portátil de administración" },
            new SparePart { Id = 6, Code = "REP-0006", Name = "Sello mecánico para bomba", UnitOfMeasure = "unidad", CurrentStock = 2, MinimumStock = 4, Description = "Kit de sellado para bomba principal contra incendios" },
            new SparePart { Id = 7, Code = "REP-0007", Name = "Rodamiento para centrífuga", UnitOfMeasure = "unidad", CurrentStock = 5, MinimumStock = 2, Description = "Rodamiento de precisión para rotor de centrífuga" },
            new SparePart { Id = 8, Code = "REP-0008", Name = "Aceite hidráulico ISO 46", UnitOfMeasure = "litro", CurrentStock = 18, MinimumStock = 10, Description = "Lubricante para montacargas y sistemas hidráulicos" },
            new SparePart { Id = 9, Code = "REP-0009", Name = "Contactor trifásico 32 A", UnitOfMeasure = "unidad", CurrentStock = 2, MinimumStock = 4, Description = "Contactor para tablero eléctrico; inventario bajo" },
            new SparePart { Id = 10, Code = "REP-0010", Name = "Guantes dieléctricos clase 0", UnitOfMeasure = "par", CurrentStock = 9, MinimumStock = 5, Description = "Equipo de protección personal para trabajos eléctricos" });

        modelBuilder.Entity<PartSupplier>().HasData(
            new PartSupplier { SparePartId = 1, SupplierId = 3, OfferedPrice = 14500m, EstimatedDeliveryDays = 2 },
            new PartSupplier { SparePartId = 1, SupplierId = 10, OfferedPrice = 15200m, EstimatedDeliveryDays = 3 },
            new PartSupplier { SparePartId = 2, SupplierId = 2, OfferedPrice = 8700m, EstimatedDeliveryDays = 1 },
            new PartSupplier { SparePartId = 2, SupplierId = 3, OfferedPrice = 9100m, EstimatedDeliveryDays = 2 },
            new PartSupplier { SparePartId = 3, SupplierId = 3, OfferedPrice = 6300m, EstimatedDeliveryDays = 2 },
            new PartSupplier { SparePartId = 3, SupplierId = 8, OfferedPrice = 6800m, EstimatedDeliveryDays = 4 },
            new PartSupplier { SparePartId = 4, SupplierId = 2, OfferedPrice = 18400m, EstimatedDeliveryDays = 1 },
            new PartSupplier { SparePartId = 5, SupplierId = 5, OfferedPrice = 49500m, EstimatedDeliveryDays = 3 },
            new PartSupplier { SparePartId = 6, SupplierId = 8, OfferedPrice = 28200m, EstimatedDeliveryDays = 2 },
            new PartSupplier { SparePartId = 6, SupplierId = 10, OfferedPrice = 26700m, EstimatedDeliveryDays = 4 },
            new PartSupplier { SparePartId = 7, SupplierId = 7, OfferedPrice = 41500m, EstimatedDeliveryDays = 5 },
            new PartSupplier { SparePartId = 8, SupplierId = 10, OfferedPrice = 4400m, EstimatedDeliveryDays = 1 },
            new PartSupplier { SparePartId = 9, SupplierId = 3, OfferedPrice = 22000m, EstimatedDeliveryDays = 2 },
            new PartSupplier { SparePartId = 9, SupplierId = 8, OfferedPrice = 23600m, EstimatedDeliveryDays = 3 },
            new PartSupplier { SparePartId = 10, SupplierId = 4, OfferedPrice = 18500m, EstimatedDeliveryDays = 2 });

        return modelBuilder;
    }
}