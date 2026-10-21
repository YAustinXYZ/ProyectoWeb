using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;
using Proyecto.Infraestructure.Data.Seed;

namespace Proyecto.Infraestructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<MaintenanceType> MaintenanceTypes => Set<MaintenanceType>();
    public DbSet<MaintenanceOrder> MaintenanceOrders => Set<MaintenanceOrder>();
    public DbSet<Technician> Technicians => Set<Technician>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<TechnicianSpecialty> TechnicianSpecialties => Set<TechnicianSpecialty>();
    public DbSet<SparePart> SpareParts => Set<SparePart>();
    public DbSet<PartSupplier> PartSuppliers => Set<PartSupplier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssetCategory>(entity =>
        {
            entity.ToTable("AssetCategories");
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Name).HasMaxLength(80).IsRequired();
            entity.Property(category => category.Description).HasMaxLength(250);
            entity.Property(category => category.PreventiveMaintenanceIntervalDays);
            entity.Property(category => category.IsActive).HasDefaultValue(true);
            entity.HasIndex(category => category.Name).IsUnique();
            entity.ToTable("AssetCategories", table => table.HasCheckConstraint(
                "CK_AssetCategories_PreventiveMaintenanceIntervalDays",
                "[PreventiveMaintenanceIntervalDays] IS NULL OR [PreventiveMaintenanceIntervalDays] > 0"));
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Locations");
            entity.HasKey(location => location.Id);
            entity.Property(location => location.Name).HasMaxLength(100).IsRequired();
            entity.Property(location => location.Description).HasMaxLength(250);
            entity.Property(location => location.IsActive).HasDefaultValue(true);
            entity.HasIndex(location => location.Name).IsUnique();
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Suppliers");
            entity.HasKey(supplier => supplier.Id);
            entity.Property(supplier => supplier.Name).HasMaxLength(120).IsRequired();
            entity.Property(supplier => supplier.TaxId).HasMaxLength(30).IsRequired();
            entity.Property(supplier => supplier.ContactName).HasMaxLength(120);
            entity.Property(supplier => supplier.Phone).HasMaxLength(30);
            entity.Property(supplier => supplier.Email).HasMaxLength(254);
            entity.Property(supplier => supplier.Address).HasMaxLength(250);
            entity.Property(supplier => supplier.GeneralConditions).HasMaxLength(1000);
            entity.Property(supplier => supplier.IsActive).HasDefaultValue(true);
            entity.HasIndex(supplier => supplier.TaxId).IsUnique();
        });

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.ToTable("Assets", table =>
            {
                table.HasCheckConstraint("CK_Assets_PurchaseCost", "[PurchaseCost] >= 0");
                table.HasCheckConstraint("CK_Assets_UsefulLifeMonths", "[UsefulLifeMonths] > 0");
                table.HasCheckConstraint("CK_Assets_UsageHours", "[UsageHours] >= 0");
                table.HasCheckConstraint("CK_Assets_Criticality", "[Criticality] IN ('Baja', 'Media', 'Alta', 'Crítica')");
                table.HasCheckConstraint("CK_Assets_Status", "[Status] IN ('Operativo', 'En Mantenimiento', 'Fuera de Servicio', 'Dado de Baja')");
            });
            entity.HasKey(asset => asset.Id);
            entity.Property(asset => asset.AssetCode).HasMaxLength(30).IsRequired();
            entity.Property(asset => asset.Name).HasMaxLength(120).IsRequired();
            entity.Property(asset => asset.Description).HasMaxLength(500);
            entity.Property(asset => asset.Brand).HasMaxLength(80);
            entity.Property(asset => asset.Model).HasMaxLength(80);
            entity.Property(asset => asset.SerialNumber).HasMaxLength(100);
            entity.Property(asset => asset.PurchaseCost).HasPrecision(18, 2);
            entity.Property(asset => asset.UsageHours).HasPrecision(18, 2);
            entity.Property(asset => asset.Criticality).HasMaxLength(20).HasDefaultValue("Media").IsRequired();
            entity.Property(asset => asset.ImageUrl).HasMaxLength(1000);
            entity.Property(asset => asset.Status).HasMaxLength(30).HasDefaultValue("Operativo").IsRequired();
            entity.Property(asset => asset.IsActive).HasDefaultValue(true);
            entity.Property(asset => asset.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.HasIndex(asset => asset.AssetCode).IsUnique();
            entity.HasIndex(asset => asset.SerialNumber).IsUnique().HasFilter("[SerialNumber] IS NOT NULL");
            entity.HasOne(asset => asset.Category).WithMany(category => category.Assets)
                .HasForeignKey(asset => asset.CategoryId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(asset => asset.Location).WithMany(location => location.Assets)
                .HasForeignKey(asset => asset.LocationId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(asset => asset.Supplier).WithMany(supplier => supplier.Assets)
                .HasForeignKey(asset => asset.SupplierId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MaintenanceType>(entity =>
        {
            entity.ToTable("MaintenanceTypes", table =>
                table.HasCheckConstraint("CK_MaintenanceTypes_DefaultIntervalDays", "[DefaultIntervalDays] IS NULL OR [DefaultIntervalDays] > 0"));
            entity.HasKey(type => type.Id);
            entity.Property(type => type.Name).HasMaxLength(80).IsRequired();
            entity.Property(type => type.Description).HasMaxLength(250);
            entity.HasIndex(type => type.Name).IsUnique();
        });

        modelBuilder.Entity<MaintenanceOrder>(entity =>
        {
            entity.ToTable("MaintenanceOrders", table =>
            {
                table.HasCheckConstraint("CK_MaintenanceOrders_Priority", "[Priority] IN ('Low', 'Medium', 'High', 'Urgent')");
                table.HasCheckConstraint(
                    "CK_MaintenanceOrders_Status",
                    "[Status] IN ('Solicitada', 'Planned', 'InProgress', 'Completed', 'Cancelled')");
                table.HasCheckConstraint("CK_MaintenanceOrders_Cost", "[Cost] IS NULL OR [Cost] >= 0");
                table.HasCheckConstraint("CK_MaintenanceOrders_DateOrder", "[CompletedAt] IS NULL OR [StartedAt] IS NULL OR [CompletedAt] >= [StartedAt]");
            });
            entity.HasKey(order => order.Id);
            entity.Property(order => order.WorkOrderCode).HasMaxLength(30).IsRequired();
            entity.Property(order => order.Title).HasMaxLength(150).IsRequired();
            entity.Property(order => order.Description).HasMaxLength(1000);
            entity.Property(order => order.Priority).HasMaxLength(16).HasDefaultValue("Medium").IsRequired();
            entity.Property(order => order.Status).HasMaxLength(20).HasDefaultValue("Solicitada").IsRequired();
            entity.Property(order => order.RequestedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(order => order.Cost).HasPrecision(18, 2);
            entity.Property(order => order.Findings).HasMaxLength(2000);
            entity.Property(order => order.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.HasIndex(order => order.WorkOrderCode).IsUnique();
            entity.HasOne(order => order.Asset).WithMany(asset => asset.MaintenanceOrders)
                .HasForeignKey(order => order.AssetId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(order => order.MaintenanceType).WithMany(type => type.MaintenanceOrders)
                .HasForeignKey(order => order.MaintenanceTypeId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(order => order.Supplier).WithMany(supplier => supplier.MaintenanceOrders)
                .HasForeignKey(order => order.SupplierId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Technician>(entity =>
        {
            entity.ToTable("Technicians", table => table.HasCheckConstraint(
                "CK_Technicians_HourlyRate", "[HourlyRate] >= 0"));
            entity.HasKey(technician => technician.Id);
            entity.Property(technician => technician.FullName).HasMaxLength(120).IsRequired();
            entity.Property(technician => technician.Identification).HasMaxLength(30).IsRequired();
            entity.Property(technician => technician.Email).HasMaxLength(254);
            entity.Property(technician => technician.Phone).HasMaxLength(30);
            entity.Property(technician => technician.HourlyRate).HasPrecision(18, 2);
            entity.Property(technician => technician.IsAvailable).HasDefaultValue(true);
            entity.HasIndex(technician => technician.Identification).IsUnique();
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.ToTable("Specialties");
            entity.HasKey(specialty => specialty.Id);
            entity.Property(specialty => specialty.Name).HasMaxLength(80).IsRequired();
            entity.Property(specialty => specialty.Description).HasMaxLength(250);
            entity.HasIndex(specialty => specialty.Name).IsUnique();
        });

        modelBuilder.Entity<TechnicianSpecialty>(entity =>
        {
            entity.ToTable("TechnicianSpecialties");
            entity.HasKey(relation => new { relation.TechnicianId, relation.SpecialtyId });
            entity.Property(relation => relation.CertificationLevel).HasMaxLength(40).IsRequired();
            entity.HasOne(relation => relation.Technician).WithMany(technician => technician.Specialties)
                .HasForeignKey(relation => relation.TechnicianId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(relation => relation.Specialty).WithMany(specialty => specialty.Technicians)
                .HasForeignKey(relation => relation.SpecialtyId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SparePart>(entity =>
        {
            entity.ToTable("SpareParts", table =>
            {
                table.HasCheckConstraint("CK_SpareParts_CurrentStock", "[CurrentStock] >= 0");
                table.HasCheckConstraint("CK_SpareParts_MinimumStock", "[MinimumStock] >= 0");
            });
            entity.HasKey(part => part.Id);
            entity.Property(part => part.Code).HasMaxLength(30).IsRequired();
            entity.Property(part => part.Name).HasMaxLength(120).IsRequired();
            entity.Property(part => part.UnitOfMeasure).HasMaxLength(30).IsRequired();
            entity.Property(part => part.CurrentStock).HasPrecision(18, 2);
            entity.Property(part => part.MinimumStock).HasPrecision(18, 2);
            entity.Property(part => part.Description).HasMaxLength(500);
            entity.HasIndex(part => part.Code).IsUnique();
        });

        modelBuilder.Entity<PartSupplier>(entity =>
        {
            entity.ToTable("PartSuppliers", table =>
            {
                table.HasCheckConstraint("CK_PartSuppliers_OfferedPrice", "[OfferedPrice] > 0");
                table.HasCheckConstraint("CK_PartSuppliers_EstimatedDeliveryDays", "[EstimatedDeliveryDays] > 0");
            });
            entity.HasKey(relation => new { relation.SparePartId, relation.SupplierId });
            entity.Property(relation => relation.OfferedPrice).HasPrecision(18, 2);
            entity.HasOne(relation => relation.SparePart).WithMany(part => part.Suppliers)
                .HasForeignKey(relation => relation.SparePartId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(relation => relation.Supplier).WithMany(supplier => supplier.PartSuppliers)
                .HasForeignKey(relation => relation.SupplierId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.SeedCatalogs();
    }
}