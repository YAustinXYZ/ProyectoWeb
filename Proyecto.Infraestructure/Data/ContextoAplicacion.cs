using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;
using Proyecto.Infraestructure.Data.Seed;

namespace Proyecto.Infraestructure.Data;

public class ContextoAplicacion(DbContextOptions<ContextoAplicacion> options) : DbContext(options)
{
    public DbSet<CategoriaActivo> CategoriasActivos => Set<CategoriaActivo>();
    public DbSet<Ubicacion> Ubicaciones => Set<Ubicacion>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Activo> Activos => Set<Activo>();
    public DbSet<TipoMantenimiento> TiposMantenimiento => Set<TipoMantenimiento>();
    public DbSet<OrdenMantenimiento> OrdenesMantenimiento => Set<OrdenMantenimiento>();
    public DbSet<Tecnico> Tecnicos => Set<Tecnico>();
    public DbSet<Especialidad> Especialidades => Set<Especialidad>();
    public DbSet<TecnicoEspecialidad> EspecialidadesTecnico => Set<TecnicoEspecialidad>();
    public DbSet<Repuesto> Repuestos => Set<Repuesto>();
    public DbSet<RepuestoProveedor> RepuestosProveedor => Set<RepuestoProveedor>();

    protected override void OnModelCreating(ModelBuilder constructorModelo)
    {
        constructorModelo.Entity<CategoriaActivo>(entidad =>
        {
            entidad.ToTable("AssetCategories", table =>
            {
                table.HasCheckConstraint("CK_AssetCategories_PreventiveThresholdHours", "[PreventiveThresholdHours] > 0");
                // COLLATE binario: con la intercalación por defecto (CI), [A-Z] también aceptaría minúsculas
                table.HasCheckConstraint("CK_AssetCategories_CodePrefix",
                    "LEN([CodePrefix]) BETWEEN 2 AND 5 AND [CodePrefix] NOT LIKE '%[^A-Z]%' COLLATE Latin1_General_BIN");
            });
            entidad.HasKey(categoria => categoria.Identificador);
            entidad.Property(categoria => categoria.Nombre).HasMaxLength(80).IsRequired();
            entidad.Property(categoria => categoria.Descripcion).HasMaxLength(250);
            entidad.Property(categoria => categoria.PrefijoCodigo).HasMaxLength(5).IsRequired();
            entidad.Property(categoria => categoria.EstaActivo).HasDefaultValue(true);
            entidad.HasIndex(categoria => categoria.Nombre).IsUnique();
            entidad.HasIndex(categoria => categoria.PrefijoCodigo).IsUnique();
        });

        constructorModelo.Entity<Ubicacion>(entidad =>
        {
            entidad.ToTable("Locations");
            entidad.HasKey(ubicacion => ubicacion.Identificador);
            entidad.Property(ubicacion => ubicacion.Nombre).HasMaxLength(100).IsRequired();
            entidad.Property(ubicacion => ubicacion.Descripcion).HasMaxLength(250);
            entidad.Property(ubicacion => ubicacion.EstaActivo).HasDefaultValue(true);
            entidad.HasIndex(ubicacion => ubicacion.Nombre).IsUnique();
        });

        constructorModelo.Entity<Proveedor>(entidad =>
        {
            entidad.ToTable("Suppliers");
            entidad.HasKey(proveedor => proveedor.Identificador);
            entidad.Property(proveedor => proveedor.Nombre).HasMaxLength(120).IsRequired();
            entidad.Property(proveedor => proveedor.IdentificacionFiscal).HasMaxLength(30).IsRequired();
            entidad.Property(proveedor => proveedor.NombreContacto).HasMaxLength(120);
            entidad.Property(proveedor => proveedor.Telefono).HasMaxLength(30);
            entidad.Property(proveedor => proveedor.Correo).HasMaxLength(254);
            entidad.Property(proveedor => proveedor.Direccion).HasMaxLength(250);
            entidad.Property(proveedor => proveedor.CondicionesGenerales).HasMaxLength(500);
            entidad.Property(proveedor => proveedor.EstaActivo).HasDefaultValue(true);
            entidad.HasIndex(proveedor => proveedor.IdentificacionFiscal).IsUnique();
        });

        constructorModelo.Entity<Activo>(entidad =>
        {
            entidad.ToTable("Assets", table =>
            {
                table.HasCheckConstraint("CK_Assets_PurchaseCost", "[PurchaseCost] >= 0");
                table.HasCheckConstraint("CK_Assets_UsefulLifeMonths", "[UsefulLifeMonths] > 0");
                table.HasCheckConstraint("CK_Assets_Status", "[Status] IN ('Operational', 'UnderMaintenance', 'OutOfService', 'Decommissioned')");
                table.HasCheckConstraint("CK_Assets_Criticality", "[Criticality] IN ('High', 'Medium', 'Low')");
                table.HasCheckConstraint("CK_Assets_UsageHours",
                    "[UsageHours] >= 0 AND [UsageHoursAtLastMaintenance] >= 0 AND [UsageHoursAtLastMaintenance] <= [UsageHours]");
            });
            entidad.HasKey(activo => activo.Identificador);
            entidad.Property(activo => activo.Identificador).HasColumnName("Id");
            entidad.Property(activo => activo.CodigoActivo).HasColumnName("AssetCode").HasMaxLength(30).IsRequired();
            entidad.Property(activo => activo.Nombre).HasColumnName("Name").HasMaxLength(120).IsRequired();
            entidad.Property(activo => activo.Descripcion).HasColumnName("Description").HasMaxLength(500);
            entidad.Property(activo => activo.Marca).HasColumnName("Brand").HasMaxLength(80);
            entidad.Property(activo => activo.Modelo).HasColumnName("Model").HasMaxLength(80);
            entidad.Property(activo => activo.NumeroSerie).HasColumnName("SerialNumber").HasMaxLength(100);
            entidad.Property(activo => activo.FechaAdquisicion).HasColumnName("AcquiredOn");
            entidad.Property(activo => activo.CostoCompra).HasColumnName("PurchaseCost").HasPrecision(18, 2);
            entidad.Property(activo => activo.MesesVidaUtil).HasColumnName("UsefulLifeMonths");
            entidad.Property(activo => activo.Estado).HasColumnName("Status").HasMaxLength(20).HasDefaultValue("Operational").IsRequired();
            entidad.Property(activo => activo.Imagen).HasColumnName("Image").HasMaxLength(200);
            entidad.Property(activo => activo.HorasUso).HasColumnName("UsageHours");
            entidad.Property(activo => activo.HorasUsoAlUltimoMantenimiento).HasColumnName("UsageHoursAtLastMaintenance");
            entidad.Property(activo => activo.Criticidad).HasColumnName("Criticality").HasMaxLength(10).HasDefaultValue("Medium").IsRequired();
            entidad.Property(activo => activo.CategoriaActivoId).HasColumnName("CategoryId");
            entidad.Property(activo => activo.UbicacionId).HasColumnName("LocationId");
            entidad.Property(activo => activo.ProveedorId).HasColumnName("SupplierId");
            entidad.Property(activo => activo.CreadoEn).HasColumnName("CreatedAt").HasDefaultValueSql("SYSUTCDATETIME()");
            entidad.HasIndex(activo => activo.CodigoActivo).IsUnique();
            entidad.HasIndex(activo => activo.NumeroSerie).IsUnique().HasFilter("[SerialNumber] IS NOT NULL");
            entidad.HasOne(activo => activo.Categoria).WithMany(categoria => categoria.Activos)
                .HasForeignKey(activo => activo.CategoriaActivoId).OnDelete(DeleteBehavior.Restrict);
            entidad.HasOne(activo => activo.Ubicacion).WithMany(ubicacion => ubicacion.Activos)
                .HasForeignKey(activo => activo.UbicacionId).OnDelete(DeleteBehavior.Restrict);
            entidad.HasOne(activo => activo.Proveedor).WithMany(proveedor => proveedor.Activos)
                .HasForeignKey(activo => activo.ProveedorId).OnDelete(DeleteBehavior.Restrict);
        });

        constructorModelo.Entity<TipoMantenimiento>(entidad =>
        {
            entidad.ToTable("MaintenanceTypes", table =>
                table.HasCheckConstraint("CK_MaintenanceTypes_DefaultIntervalDays", "[DefaultIntervalDays] IS NULL OR [DefaultIntervalDays] > 0"));
            entidad.HasKey(type => type.Identificador);
            entidad.Property(type => type.Nombre).HasMaxLength(80).IsRequired();
            entidad.Property(type => type.Descripcion).HasMaxLength(250);
            entidad.Property(type => type.EsEmergencia).HasDefaultValue(false);
            entidad.HasIndex(type => type.Nombre).IsUnique();
            entidad.HasOne(type => type.EspecialidadRequerida).WithMany(especialidad => especialidad.TiposMantenimiento)
                .HasForeignKey(type => type.EspecialidadRequeridaId).OnDelete(DeleteBehavior.Restrict);
        });

        constructorModelo.Entity<OrdenMantenimiento>(entidad =>
        {
            entidad.ToTable("MaintenanceOrders", table =>
            {
                table.HasCheckConstraint("CK_MaintenanceOrders_Priority", "[Priority] IN ('Low', 'Medium', 'High', 'Urgent')");
                table.HasCheckConstraint("CK_MaintenanceOrders_Status",
                    "[Status] IN ('Requested', 'Diagnosed', 'Approved', 'InProgress', 'WaitingForParts', 'Completed', 'Rejected', 'Cancelled')");
                table.HasCheckConstraint("CK_MaintenanceOrders_TotalCost", "[TotalCost] IS NULL OR [TotalCost] >= 0");
                table.HasCheckConstraint("CK_MaintenanceOrders_DateOrder", "[ClosedAt] IS NULL OR [StartedAt] IS NULL OR [ClosedAt] >= [StartedAt]");
            });
            entidad.HasKey(orden => orden.Identificador);
            entidad.Property(orden => orden.CodigoOrdenTrabajo).HasMaxLength(30).IsRequired();
            entidad.Property(orden => orden.Titulo).HasMaxLength(150).IsRequired();
            entidad.Property(orden => orden.Descripcion).HasMaxLength(1000);
            entidad.Property(orden => orden.Motivo).HasMaxLength(500).IsRequired();
            entidad.Property(orden => orden.Prioridad).HasMaxLength(16).HasDefaultValue("Medium").IsRequired();
            entidad.Property(orden => orden.Estado).HasMaxLength(20).HasDefaultValue("Requested").IsRequired();
            entidad.Property(orden => orden.SolicitadoEn).HasDefaultValueSql("SYSUTCDATETIME()");
            entidad.Property(orden => orden.CostoTotal).HasPrecision(18, 2);
            entidad.Property(orden => orden.Hallazgos).HasMaxLength(2000);
            entidad.Property(orden => orden.CreadoEn).HasDefaultValueSql("SYSUTCDATETIME()");
            entidad.HasIndex(orden => orden.CodigoOrdenTrabajo).IsUnique();
            entidad.HasOne(orden => orden.Activo).WithMany(activo => activo.OrdenesMantenimiento)
                .HasForeignKey(orden => orden.ActivoId).OnDelete(DeleteBehavior.Restrict);
            entidad.HasOne(orden => orden.TipoMantenimiento).WithMany(type => type.OrdenesMantenimiento)
                .HasForeignKey(orden => orden.TipoMantenimientoId).OnDelete(DeleteBehavior.Restrict);
            entidad.HasOne(orden => orden.Proveedor).WithMany(proveedor => proveedor.OrdenesMantenimiento)
                .HasForeignKey(orden => orden.ProveedorId).OnDelete(DeleteBehavior.Restrict);
        });

        constructorModelo.Entity<Tecnico>(entidad =>
        {
            entidad.ToTable("Technicians", table =>
                table.HasCheckConstraint("CK_Technicians_HourlyRate", "[HourlyRate] >= 0"));
            entidad.HasKey(tecnico => tecnico.Identificador);
            entidad.Property(tecnico => tecnico.NombreCompleto).HasMaxLength(150).IsRequired();
            entidad.Property(tecnico => tecnico.NumeroIdentificacion).HasMaxLength(30).IsRequired();
            entidad.Property(tecnico => tecnico.Correo).HasMaxLength(254).IsRequired();
            entidad.Property(tecnico => tecnico.Telefono).HasMaxLength(30).IsRequired();
            entidad.Property(tecnico => tecnico.EstaDisponible).HasDefaultValue(true);
            entidad.Property(tecnico => tecnico.TarifaPorHora).HasPrecision(18, 2);
            entidad.HasIndex(tecnico => tecnico.NumeroIdentificacion).IsUnique();
        });

        constructorModelo.Entity<Especialidad>(entidad =>
        {
            entidad.ToTable("Specialties");
            entidad.HasKey(especialidad => especialidad.Identificador);
            entidad.Property(especialidad => especialidad.Nombre).HasMaxLength(100).IsRequired();
            entidad.Property(especialidad => especialidad.Descripcion).HasMaxLength(250);
            entidad.HasIndex(especialidad => especialidad.Nombre).IsUnique();
        });

        constructorModelo.Entity<TecnicoEspecialidad>(entidad =>
        {
            entidad.ToTable("TechnicianSpecialties", table =>
                table.HasCheckConstraint("CK_TechnicianSpecialties_CertificationLevel", "[CertificationLevel] IN ('Basic', 'Intermediate', 'Advanced')"));
            entidad.HasKey(link => new { link.TecnicoId, link.EspecialidadId });
            entidad.Property(link => link.NivelCertificacion).HasMaxLength(20).IsRequired();
            entidad.Property(link => link.CertificadoDesde).HasColumnType("date");
            entidad.HasOne(link => link.Tecnico).WithMany(tecnico => tecnico.EspecialidadesTecnico)
                .HasForeignKey(link => link.TecnicoId).OnDelete(DeleteBehavior.Restrict);
            entidad.HasOne(link => link.Especialidad).WithMany(especialidad => especialidad.EspecialidadesTecnico)
                .HasForeignKey(link => link.EspecialidadId).OnDelete(DeleteBehavior.Restrict);
        });

        constructorModelo.Entity<Repuesto>(entidad =>
        {
            entidad.ToTable("SpareParts", table =>
            {
                // El stock nunca puede ser negativo: se valida en el DTO y la base de datos lo garantiza
                table.HasCheckConstraint("CK_SpareParts_Stock", "[CurrentStock] >= 0 AND [MinimumStock] >= 0");
                table.HasCheckConstraint("CK_SpareParts_UnitOfMeasure",
                    "[UnitOfMeasure] IN ('Unit', 'Box', 'Pack', 'Meter', 'Roll', 'Kit')");
            });
            entidad.HasKey(repuesto => repuesto.Identificador);
            entidad.Property(repuesto => repuesto.Identificador).HasColumnName("Id");
            entidad.Property(repuesto => repuesto.Codigo).HasColumnName("Code").HasMaxLength(30).IsRequired();
            entidad.Property(repuesto => repuesto.Nombre).HasColumnName("Name").HasMaxLength(120).IsRequired();
            entidad.Property(repuesto => repuesto.Descripcion).HasColumnName("Description").HasMaxLength(500);
            entidad.Property(repuesto => repuesto.UnidadMedida).HasColumnName("UnitOfMeasure").HasMaxLength(10).IsRequired();
            entidad.Property(repuesto => repuesto.StockActual).HasColumnName("CurrentStock");
            entidad.Property(repuesto => repuesto.StockMinimo).HasColumnName("MinimumStock");
            entidad.Property(repuesto => repuesto.CreadoEn).HasColumnName("CreatedAt").HasDefaultValueSql("SYSUTCDATETIME()");
            entidad.HasIndex(repuesto => repuesto.Codigo).IsUnique();
        });

        constructorModelo.Entity<RepuestoProveedor>(entidad =>
        {
            entidad.ToTable("SparePartSuppliers", table =>
            {
                table.HasCheckConstraint("CK_SparePartSuppliers_OfferedPrice", "[OfferedPrice] >= 0");
                table.HasCheckConstraint("CK_SparePartSuppliers_LeadTimeDays", "[EstimatedLeadTimeDays] >= 0");
            });
            // Llave compuesta: un proveedor aparece una sola vez por repuesto
            entidad.HasKey(enlace => new { enlace.RepuestoId, enlace.ProveedorId });
            entidad.Property(enlace => enlace.RepuestoId).HasColumnName("SparePartId");
            entidad.Property(enlace => enlace.ProveedorId).HasColumnName("SupplierId");
            entidad.Property(enlace => enlace.PrecioOfrecido).HasColumnName("OfferedPrice").HasPrecision(18, 2);
            entidad.Property(enlace => enlace.DiasEntregaEstimados).HasColumnName("EstimatedLeadTimeDays");
            entidad.HasOne(enlace => enlace.Repuesto).WithMany(repuesto => repuesto.ProveedoresRepuesto)
                .HasForeignKey(enlace => enlace.RepuestoId).OnDelete(DeleteBehavior.Restrict);
            entidad.HasOne(enlace => enlace.Proveedor).WithMany(proveedor => proveedor.RepuestosProveedor)
                .HasForeignKey(enlace => enlace.ProveedorId).OnDelete(DeleteBehavior.Restrict);
        });

        constructorModelo.Entity<CategoriaActivo>().Property(entidad => entidad.Identificador).HasColumnName("Id");
        constructorModelo.Entity<CategoriaActivo>().Property(entidad => entidad.Nombre).HasColumnName("Name");
        constructorModelo.Entity<CategoriaActivo>().Property(entidad => entidad.Descripcion).HasColumnName("Description");
        constructorModelo.Entity<CategoriaActivo>().Property(entidad => entidad.UmbralPreventivoHoras).HasColumnName("PreventiveThresholdHours");
        constructorModelo.Entity<CategoriaActivo>().Property(entidad => entidad.PrefijoCodigo).HasColumnName("CodePrefix");
        constructorModelo.Entity<CategoriaActivo>().Property(entidad => entidad.EstaActivo).HasColumnName("IsActive");

        constructorModelo.Entity<Ubicacion>().Property(entidad => entidad.Identificador).HasColumnName("Id");
        constructorModelo.Entity<Ubicacion>().Property(entidad => entidad.Nombre).HasColumnName("Name");
        constructorModelo.Entity<Ubicacion>().Property(entidad => entidad.Descripcion).HasColumnName("Description");
        constructorModelo.Entity<Ubicacion>().Property(entidad => entidad.EstaActivo).HasColumnName("IsActive");

        constructorModelo.Entity<Proveedor>().Property(entidad => entidad.Identificador).HasColumnName("Id");
        constructorModelo.Entity<Proveedor>().Property(entidad => entidad.Nombre).HasColumnName("Name");
        constructorModelo.Entity<Proveedor>().Property(entidad => entidad.IdentificacionFiscal).HasColumnName("TaxId");
        constructorModelo.Entity<Proveedor>().Property(entidad => entidad.NombreContacto).HasColumnName("ContactName");
        constructorModelo.Entity<Proveedor>().Property(entidad => entidad.Telefono).HasColumnName("Phone");
        constructorModelo.Entity<Proveedor>().Property(entidad => entidad.Correo).HasColumnName("Email");
        constructorModelo.Entity<Proveedor>().Property(entidad => entidad.Direccion).HasColumnName("Address");
        constructorModelo.Entity<Proveedor>().Property(entidad => entidad.CondicionesGenerales).HasColumnName("GeneralConditions");
        constructorModelo.Entity<Proveedor>().Property(entidad => entidad.EstaActivo).HasColumnName("IsActive");

        constructorModelo.Entity<TipoMantenimiento>().Property(entidad => entidad.Identificador).HasColumnName("Id");
        constructorModelo.Entity<TipoMantenimiento>().Property(entidad => entidad.Nombre).HasColumnName("Name");
        constructorModelo.Entity<TipoMantenimiento>().Property(entidad => entidad.Descripcion).HasColumnName("Description");
        constructorModelo.Entity<TipoMantenimiento>().Property(entidad => entidad.IntervaloPredeterminadoDias).HasColumnName("DefaultIntervalDays");
        constructorModelo.Entity<TipoMantenimiento>().Property(entidad => entidad.EsEmergencia).HasColumnName("IsEmergency");
        constructorModelo.Entity<TipoMantenimiento>().Property(entidad => entidad.EspecialidadRequeridaId).HasColumnName("RequiredSpecialtyId");

        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.Identificador).HasColumnName("Id");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.CodigoOrdenTrabajo).HasColumnName("WorkOrderCode");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.ActivoId).HasColumnName("AssetId");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.TipoMantenimientoId).HasColumnName("MaintenanceTypeId");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.ProveedorId).HasColumnName("SupplierId");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.Titulo).HasColumnName("Title");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.Descripcion).HasColumnName("Description");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.Motivo).HasColumnName("Reason");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.Prioridad).HasColumnName("Priority");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.Estado).HasColumnName("Status");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.SolicitadoEn).HasColumnName("RequestedAt");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.ProgramadoPara).HasColumnName("ScheduledFor");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.IniciadoEn).HasColumnName("StartedAt");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.CerradoEn).HasColumnName("ClosedAt");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.CostoTotal).HasColumnName("TotalCost");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.Hallazgos).HasColumnName("Findings");
        constructorModelo.Entity<OrdenMantenimiento>().Property(entidad => entidad.CreadoEn).HasColumnName("CreatedAt");

        constructorModelo.Entity<Tecnico>().Property(entidad => entidad.Identificador).HasColumnName("Id");
        constructorModelo.Entity<Tecnico>().Property(entidad => entidad.NombreCompleto).HasColumnName("FullName");
        constructorModelo.Entity<Tecnico>().Property(entidad => entidad.NumeroIdentificacion).HasColumnName("IdentificationNumber");
        constructorModelo.Entity<Tecnico>().Property(entidad => entidad.Correo).HasColumnName("Email");
        constructorModelo.Entity<Tecnico>().Property(entidad => entidad.Telefono).HasColumnName("Phone");
        constructorModelo.Entity<Tecnico>().Property(entidad => entidad.EstaDisponible).HasColumnName("IsAvailable");
        constructorModelo.Entity<Tecnico>().Property(entidad => entidad.TarifaPorHora).HasColumnName("HourlyRate");

        constructorModelo.Entity<Especialidad>().Property(entidad => entidad.Identificador).HasColumnName("Id");
        constructorModelo.Entity<Especialidad>().Property(entidad => entidad.Nombre).HasColumnName("Name");
        constructorModelo.Entity<Especialidad>().Property(entidad => entidad.Descripcion).HasColumnName("Description");

        constructorModelo.Entity<TecnicoEspecialidad>().Property(entidad => entidad.TecnicoId).HasColumnName("TechnicianId");
        constructorModelo.Entity<TecnicoEspecialidad>().Property(entidad => entidad.EspecialidadId).HasColumnName("SpecialtyId");
        constructorModelo.Entity<TecnicoEspecialidad>().Property(entidad => entidad.NivelCertificacion).HasColumnName("CertificationLevel");
        constructorModelo.Entity<TecnicoEspecialidad>().Property(entidad => entidad.CertificadoDesde).HasColumnName("CertifiedSince");

        constructorModelo.SembrarCatalogos();
    }
}