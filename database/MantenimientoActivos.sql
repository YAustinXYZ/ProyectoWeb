IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE TABLE [AssetCategories] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(80) NOT NULL,
        [Description] nvarchar(250) NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_AssetCategories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE TABLE [Locations] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(250) NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_Locations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE TABLE [MaintenanceTypes] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(80) NOT NULL,
        [Description] nvarchar(250) NULL,
        [DefaultIntervalDays] int NULL,
        CONSTRAINT [PK_MaintenanceTypes] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_MaintenanceTypes_DefaultIntervalDays] CHECK ([DefaultIntervalDays] IS NULL OR [DefaultIntervalDays] > 0)
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE TABLE [Suppliers] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(120) NOT NULL,
        [TaxId] nvarchar(30) NOT NULL,
        [ContactName] nvarchar(120) NULL,
        [Phone] nvarchar(30) NULL,
        [Email] nvarchar(254) NULL,
        [Address] nvarchar(250) NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_Suppliers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE TABLE [Assets] (
        [Id] int NOT NULL IDENTITY,
        [AssetCode] nvarchar(30) NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Brand] nvarchar(80) NULL,
        [Model] nvarchar(80) NULL,
        [SerialNumber] nvarchar(100) NULL,
        [AcquiredOn] datetime2 NOT NULL,
        [PurchaseCost] decimal(18,2) NOT NULL,
        [UsefulLifeMonths] int NOT NULL,
        [Status] nvarchar(20) NOT NULL DEFAULT N'Active',
        [CategoryId] int NOT NULL,
        [LocationId] int NOT NULL,
        [SupplierId] int NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Assets] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Assets_PurchaseCost] CHECK ([PurchaseCost] >= 0),
        CONSTRAINT [CK_Assets_Status] CHECK ([Status] IN ('Active', 'UnderMaintenance', 'Retired')),
        CONSTRAINT [CK_Assets_UsefulLifeMonths] CHECK ([UsefulLifeMonths] > 0),
        CONSTRAINT [FK_Assets_AssetCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [AssetCategories] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Assets_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Assets_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE TABLE [MaintenanceOrders] (
        [Id] int NOT NULL IDENTITY,
        [WorkOrderCode] nvarchar(30) NOT NULL,
        [AssetId] int NOT NULL,
        [MaintenanceTypeId] int NOT NULL,
        [SupplierId] int NULL,
        [Title] nvarchar(150) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [Priority] nvarchar(16) NOT NULL DEFAULT N'Medium',
        [Status] nvarchar(20) NOT NULL DEFAULT N'Planned',
        [ScheduledFor] datetime2 NOT NULL,
        [StartedAt] datetime2 NULL,
        [CompletedAt] datetime2 NULL,
        [Cost] decimal(18,2) NULL,
        [Findings] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_MaintenanceOrders] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_MaintenanceOrders_Cost] CHECK ([Cost] IS NULL OR [Cost] >= 0),
        CONSTRAINT [CK_MaintenanceOrders_DateOrder] CHECK ([CompletedAt] IS NULL OR [StartedAt] IS NULL OR [CompletedAt] >= [StartedAt]),
        CONSTRAINT [CK_MaintenanceOrders_Priority] CHECK ([Priority] IN ('Low', 'Medium', 'High', 'Urgent')),
        CONSTRAINT [CK_MaintenanceOrders_Status] CHECK ([Status] IN ('Planned', 'InProgress', 'Completed', 'Cancelled')),
        CONSTRAINT [FK_MaintenanceOrders_Assets_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [Assets] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MaintenanceOrders_MaintenanceTypes_MaintenanceTypeId] FOREIGN KEY ([MaintenanceTypeId]) REFERENCES [MaintenanceTypes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MaintenanceOrders_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[AssetCategories]'))
        SET IDENTITY_INSERT [AssetCategories] ON;
    EXEC(N'INSERT INTO [AssetCategories] ([Id], [Description], [IsActive], [Name])
    VALUES (1, N''Computadoras y periféricos'', CAST(1 AS bit), N''Equipo de cómputo''),
    (2, N''Muebles y estaciones de trabajo'', CAST(1 AS bit), N''Mobiliario''),
    (3, N''Máquinas y herramientas de producción'', CAST(1 AS bit), N''Equipo industrial'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[AssetCategories]'))
        SET IDENTITY_INSERT [AssetCategories] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Locations]'))
        SET IDENTITY_INSERT [Locations] ON;
    EXEC(N'INSERT INTO [Locations] ([Id], [Description], [IsActive], [Name])
    VALUES (1, N''Sede central'', CAST(1 AS bit), N''Edificio principal''),
    (2, N''Área de mantenimiento'', CAST(1 AS bit), N''Taller''),
    (3, N''Almacenamiento de activos'', CAST(1 AS bit), N''Bodega'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Locations]'))
        SET IDENTITY_INSERT [Locations] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DefaultIntervalDays', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[MaintenanceTypes]'))
        SET IDENTITY_INSERT [MaintenanceTypes] ON;
    EXEC(N'INSERT INTO [MaintenanceTypes] ([Id], [DefaultIntervalDays], [Description], [Name])
    VALUES (1, 90, N''Mantenimiento programado para prevenir fallas'', N''Preventivo''),
    (2, NULL, N''Reparación de una falla reportada'', N''Correctivo''),
    (3, 180, N''Revisión general del estado del activo'', N''Inspección'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DefaultIntervalDays', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[MaintenanceTypes]'))
        SET IDENTITY_INSERT [MaintenanceTypes] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'ContactName', N'Email', N'IsActive', N'Name', N'Phone', N'TaxId') AND [object_id] = OBJECT_ID(N'[Suppliers]'))
        SET IDENTITY_INSERT [Suppliers] ON;
    EXEC(N'INSERT INTO [Suppliers] ([Id], [Address], [ContactName], [Email], [IsActive], [Name], [Phone], [TaxId])
    VALUES (1, N''San José'', N''María Rodríguez'', N''servicio@example.com'', CAST(1 AS bit), N''Servicios Técnicos CR'', N''2200-1000'', N''3-101-000001''),
    (2, N''Heredia'', N''Carlos Méndez'', N''taller@example.com'', CAST(1 AS bit), N''Mantenimiento Industrial del Valle'', N''2200-2000'', N''3-101-000002'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'ContactName', N'Email', N'IsActive', N'Name', N'Phone', N'TaxId') AND [object_id] = OBJECT_ID(N'[Suppliers]'))
        SET IDENTITY_INSERT [Suppliers] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AcquiredOn', N'AssetCode', N'Brand', N'CategoryId', N'CreatedAt', N'Description', N'LocationId', N'Model', N'Name', N'PurchaseCost', N'SerialNumber', N'Status', N'SupplierId', N'UsefulLifeMonths') AND [object_id] = OBJECT_ID(N'[Assets]'))
        SET IDENTITY_INSERT [Assets] ON;
    EXEC(N'INSERT INTO [Assets] ([Id], [AcquiredOn], [AssetCode], [Brand], [CategoryId], [CreatedAt], [Description], [LocationId], [Model], [Name], [PurchaseCost], [SerialNumber], [Status], [SupplierId], [UsefulLifeMonths])
    VALUES (1, ''2024-01-15T00:00:00.0000000'', N''ACT-0001'', N''Dell'', 1, ''2024-01-15T00:00:00.0000000Z'', N''Equipo portátil de oficina'', 1, N''Latitude 5440'', N''Laptop de administración'', 625000.0, N''SN-DELL-0001'', N''Active'', 1, 48),
    (2, ''2023-08-10T00:00:00.0000000'', N''ACT-0002'', N''Bosch'', 3, ''2023-08-10T00:00:00.0000000Z'', N''Taladro para trabajos de taller'', 2, N''GBM 13-2 RE'', N''Taladro de banco'', 185000.0, N''SN-BOSCH-0002'', N''Active'', 2, 72),
    (3, ''2022-03-02T00:00:00.0000000'', N''ACT-0003'', N''Oficina CR'', 2, ''2022-03-02T00:00:00.0000000Z'', N''Módulo de recepción'', 1, N''Recepción 180'', N''Escritorio de recepción'', 210000.0, NULL, N''Active'', NULL, 120)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AcquiredOn', N'AssetCode', N'Brand', N'CategoryId', N'CreatedAt', N'Description', N'LocationId', N'Model', N'Name', N'PurchaseCost', N'SerialNumber', N'Status', N'SupplierId', N'UsefulLifeMonths') AND [object_id] = OBJECT_ID(N'[Assets]'))
        SET IDENTITY_INSERT [Assets] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AssetId', N'CompletedAt', N'Cost', N'CreatedAt', N'Description', N'Findings', N'MaintenanceTypeId', N'Priority', N'ScheduledFor', N'StartedAt', N'Status', N'SupplierId', N'Title', N'WorkOrderCode') AND [object_id] = OBJECT_ID(N'[MaintenanceOrders]'))
        SET IDENTITY_INSERT [MaintenanceOrders] ON;
    EXEC(N'INSERT INTO [MaintenanceOrders] ([Id], [AssetId], [CompletedAt], [Cost], [CreatedAt], [Description], [Findings], [MaintenanceTypeId], [Priority], [ScheduledFor], [StartedAt], [Status], [SupplierId], [Title], [WorkOrderCode])
    VALUES (1, 1, NULL, 25000.0, ''2026-09-01T00:00:00.0000000Z'', N''Limpieza interna, revisión de batería y diagnóstico'', NULL, 1, N''Medium'', ''2026-10-15T00:00:00.0000000'', NULL, N''Planned'', 1, N''Limpieza y revisión preventiva'', N''OT-2026-0001''),
    (2, 2, ''2026-08-20T10:00:00.0000000Z'', 18000.0, ''2026-08-10T00:00:00.0000000Z'', N''Verificar broca, cableado y fijación de la herramienta'', N''Equipo operativo; se ajustó la base'', 3, N''High'', ''2026-08-20T00:00:00.0000000'', ''2026-08-20T08:00:00.0000000Z'', N''Completed'', 2, N''Inspección de seguridad'', N''OT-2026-0002''),
    (3, 3, NULL, NULL, ''2026-09-05T00:00:00.0000000Z'', N''Ajustar uniones y comprobar estabilidad'', NULL, 1, N''Low'', ''2026-11-05T00:00:00.0000000'', NULL, N''Planned'', NULL, N''Revisión de estructura'', N''OT-2026-0003'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AssetId', N'CompletedAt', N'Cost', N'CreatedAt', N'Description', N'Findings', N'MaintenanceTypeId', N'Priority', N'ScheduledFor', N'StartedAt', N'Status', N'SupplierId', N'Title', N'WorkOrderCode') AND [object_id] = OBJECT_ID(N'[MaintenanceOrders]'))
        SET IDENTITY_INSERT [MaintenanceOrders] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AssetCategories_Name] ON [AssetCategories] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Assets_AssetCode] ON [Assets] ([AssetCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Assets_CategoryId] ON [Assets] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Assets_LocationId] ON [Assets] ([LocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Assets_SerialNumber] ON [Assets] ([SerialNumber]) WHERE [SerialNumber] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Assets_SupplierId] ON [Assets] ([SupplierId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Locations_Name] ON [Locations] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_MaintenanceOrders_AssetId] ON [MaintenanceOrders] ([AssetId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_MaintenanceOrders_MaintenanceTypeId] ON [MaintenanceOrders] ([MaintenanceTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_MaintenanceOrders_SupplierId] ON [MaintenanceOrders] ([SupplierId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MaintenanceOrders_WorkOrderCode] ON [MaintenanceOrders] ([WorkOrderCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MaintenanceTypes_Name] ON [MaintenanceTypes] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Suppliers_TaxId] ON [Suppliers] ([TaxId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007062459_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261007062459_InitialCreate', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    ALTER TABLE [MaintenanceOrders] DROP CONSTRAINT [CK_MaintenanceOrders_Status];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    ALTER TABLE [Assets] DROP CONSTRAINT [CK_Assets_Status];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    UPDATE [Assets]
    SET [Status] = CASE [Status]
        WHEN 'UnderMaintenance' THEN 'En Mantenimiento'
        WHEN 'Retired' THEN 'Dado de Baja'
        ELSE 'Operativo'
    END;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    ALTER TABLE [Suppliers] ADD [GeneralConditions] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MaintenanceOrders]') AND [c].[name] = N'Status');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [MaintenanceOrders] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [MaintenanceOrders] ADD DEFAULT N'Solicitada' FOR [Status];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MaintenanceOrders]') AND [c].[name] = N'ScheduledFor');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [MaintenanceOrders] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [MaintenanceOrders] ALTER COLUMN [ScheduledFor] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    ALTER TABLE [MaintenanceOrders] ADD [RequestedAt] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Assets]') AND [c].[name] = N'Status');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Assets] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Assets] ALTER COLUMN [Status] nvarchar(30) NOT NULL;
    ALTER TABLE [Assets] ADD DEFAULT N'Operativo' FOR [Status];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    ALTER TABLE [Assets] ADD [Criticality] nvarchar(20) NOT NULL DEFAULT N'Media';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    ALTER TABLE [Assets] ADD [ImageUrl] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    ALTER TABLE [Assets] ADD [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    ALTER TABLE [Assets] ADD [UsageHours] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    ALTER TABLE [AssetCategories] ADD [PreventiveMaintenanceIntervalDays] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    CREATE TABLE [SpareParts] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(30) NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [UnitOfMeasure] nvarchar(30) NOT NULL,
        [CurrentStock] decimal(18,2) NOT NULL,
        [MinimumStock] decimal(18,2) NOT NULL,
        [Description] nvarchar(500) NULL,
        CONSTRAINT [PK_SpareParts] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_SpareParts_CurrentStock] CHECK ([CurrentStock] >= 0),
        CONSTRAINT [CK_SpareParts_MinimumStock] CHECK ([MinimumStock] >= 0)
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    CREATE TABLE [Specialties] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(80) NOT NULL,
        [Description] nvarchar(250) NULL,
        CONSTRAINT [PK_Specialties] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    CREATE TABLE [Technicians] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(120) NOT NULL,
        [Identification] nvarchar(30) NOT NULL,
        [Email] nvarchar(254) NULL,
        [Phone] nvarchar(30) NULL,
        [IsAvailable] bit NOT NULL DEFAULT CAST(1 AS bit),
        [HourlyRate] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_Technicians] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Technicians_HourlyRate] CHECK ([HourlyRate] >= 0)
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    CREATE TABLE [PartSuppliers] (
        [SparePartId] int NOT NULL,
        [SupplierId] int NOT NULL,
        [OfferedPrice] decimal(18,2) NOT NULL,
        [EstimatedDeliveryDays] int NOT NULL,
        CONSTRAINT [PK_PartSuppliers] PRIMARY KEY ([SparePartId], [SupplierId]),
        CONSTRAINT [CK_PartSuppliers_EstimatedDeliveryDays] CHECK ([EstimatedDeliveryDays] > 0),
        CONSTRAINT [CK_PartSuppliers_OfferedPrice] CHECK ([OfferedPrice] > 0),
        CONSTRAINT [FK_PartSuppliers_SpareParts_SparePartId] FOREIGN KEY ([SparePartId]) REFERENCES [SpareParts] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PartSuppliers_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    CREATE TABLE [TechnicianSpecialties] (
        [TechnicianId] int NOT NULL,
        [SpecialtyId] int NOT NULL,
        [CertificationLevel] nvarchar(40) NOT NULL,
        [CertifiedSince] datetime2 NOT NULL,
        CONSTRAINT [PK_TechnicianSpecialties] PRIMARY KEY ([TechnicianId], [SpecialtyId]),
        CONSTRAINT [FK_TechnicianSpecialties_Specialties_SpecialtyId] FOREIGN KEY ([SpecialtyId]) REFERENCES [Specialties] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TechnicianSpecialties_Technicians_TechnicianId] FOREIGN KEY ([TechnicianId]) REFERENCES [Technicians] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'UPDATE [AssetCategories] SET [PreventiveMaintenanceIntervalDays] = 90
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'UPDATE [AssetCategories] SET [PreventiveMaintenanceIntervalDays] = 365
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'UPDATE [AssetCategories] SET [PreventiveMaintenanceIntervalDays] = 90
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name', N'PreventiveMaintenanceIntervalDays') AND [object_id] = OBJECT_ID(N'[AssetCategories]'))
        SET IDENTITY_INSERT [AssetCategories] ON;
    EXEC(N'INSERT INTO [AssetCategories] ([Id], [Description], [IsActive], [Name], [PreventiveMaintenanceIntervalDays])
    VALUES (4, N''Equipos de protección y respuesta a emergencias'', CAST(1 AS bit), N''Seguridad industrial'', 180),
    (5, N''Tableros, circuitos y respaldo eléctrico'', CAST(1 AS bit), N''Instalaciones eléctricas'', 120),
    (6, N''Instalaciones y sistemas del edificio'', CAST(1 AS bit), N''Infraestructura'', 180),
    (7, N''Vehículos y equipos móviles institucionales'', CAST(1 AS bit), N''Vehículos'', 90),
    (8, N''Instrumentos y equipos de laboratorio'', CAST(1 AS bit), N''Laboratorio'', 180)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name', N'PreventiveMaintenanceIntervalDays') AND [object_id] = OBJECT_ID(N'[AssetCategories]'))
        SET IDENTITY_INSERT [AssetCategories] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'UPDATE [Assets] SET [Criticality] = N''Media'', [ImageUrl] = N''https://images.example.com/assets/laptop-administracion.jpg'', [IsActive] = CAST(1 AS bit), [Status] = N''Operativo'', [UsageHours] = 1850.0
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'UPDATE [Assets] SET [Criticality] = N''Alta'', [ImageUrl] = NULL, [IsActive] = CAST(1 AS bit), [Status] = N''Fuera de Servicio'', [UsageHours] = 430.0
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'UPDATE [Assets] SET [Criticality] = N''Baja'', [ImageUrl] = NULL, [IsActive] = CAST(1 AS bit), [Status] = N''Operativo'', [UsageHours] = 0.0
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Locations]'))
        SET IDENTITY_INSERT [Locations] ON;
    EXEC(N'INSERT INTO [Locations] ([Id], [Description], [IsActive], [Name])
    VALUES (4, N''Tableros y sistemas de respaldo'', CAST(1 AS bit), N''Cuarto eléctrico''),
    (5, N''Equipos de medición y ensayo'', CAST(1 AS bit), N''Laboratorio central''),
    (6, N''Recepción y despacho de materiales'', CAST(1 AS bit), N''Andén de carga''),
    (7, N''Flota y equipo móvil'', CAST(1 AS bit), N''Patio de vehículos''),
    (8, N''Espacio de formación técnica'', CAST(1 AS bit), N''Aula de capacitación'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Locations]'))
        SET IDENTITY_INSERT [Locations] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'UPDATE [MaintenanceOrders] SET [Cost] = NULL, [RequestedAt] = ''2026-09-01T00:00:00.0000000'', [Status] = N''Solicitada''
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'UPDATE [MaintenanceOrders] SET [CompletedAt] = NULL, [Cost] = NULL, [Findings] = NULL, [RequestedAt] = ''2026-08-10T00:00:00.0000000'', [StartedAt] = NULL, [Status] = N''Solicitada''
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'UPDATE [MaintenanceOrders] SET [RequestedAt] = ''2026-09-05T00:00:00.0000000'', [Status] = N''Solicitada''
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DefaultIntervalDays', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[MaintenanceTypes]'))
        SET IDENTITY_INSERT [MaintenanceTypes] ON;
    EXEC(N'INSERT INTO [MaintenanceTypes] ([Id], [DefaultIntervalDays], [Description], [Name])
    VALUES (4, 120, N''Diagnóstico y reparación de equipos y sistemas eléctricos'', N''Mantenimiento eléctrico''),
    (5, 180, N''Verificación metrológica de instrumentos'', N''Calibración''),
    (6, 180, N''Inspección de sistemas y equipos de seguridad'', N''Seguridad''),
    (7, 90, N''Lubricación, ajuste y reemplazo de componentes mecánicos'', N''Mantenimiento mecánico''),
    (8, 365, N''Conservación de instalaciones y sistemas del edificio'', N''Infraestructura edilicia'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DefaultIntervalDays', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[MaintenanceTypes]'))
        SET IDENTITY_INSERT [MaintenanceTypes] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CurrentStock', N'Description', N'MinimumStock', N'Name', N'UnitOfMeasure') AND [object_id] = OBJECT_ID(N'[SpareParts]'))
        SET IDENTITY_INSERT [SpareParts] ON;
    EXEC(N'INSERT INTO [SpareParts] ([Id], [Code], [CurrentStock], [Description], [MinimumStock], [Name], [UnitOfMeasure])
    VALUES (1, N''REP-0001'', 8.0, N''Filtro de admisión compatible con generador de emergencia'', 4.0, N''Filtro de aire para generador'', N''unidad''),
    (2, N''REP-0002'', 3.0, N''Correa trapezoidal para maquinaria de taller; requiere reposición'', 5.0, N''Correa de transmisión industrial'', N''unidad''),
    (3, N''REP-0003'', 12.0, N''Fusible de protección para tablero de distribución'', 6.0, N''Fusible de potencia 100 A'', N''unidad''),
    (4, N''REP-0004'', 4.0, N''Mandril de 13 mm con llave para taladro de banco'', 2.0, N''Mandril para taladro de banco'', N''unidad''),
    (5, N''REP-0005'', 6.0, N''Batería de reemplazo para estación portátil de administración'', 3.0, N''Batería para laptop Latitude'', N''unidad''),
    (6, N''REP-0006'', 2.0, N''Kit de sellado para bomba principal contra incendios'', 4.0, N''Sello mecánico para bomba'', N''unidad''),
    (7, N''REP-0007'', 5.0, N''Rodamiento de precisión para rotor de centrífuga'', 2.0, N''Rodamiento para centrífuga'', N''unidad''),
    (8, N''REP-0008'', 18.0, N''Lubricante para montacargas y sistemas hidráulicos'', 10.0, N''Aceite hidráulico ISO 46'', N''litro''),
    (9, N''REP-0009'', 2.0, N''Contactor para tablero eléctrico; inventario bajo'', 4.0, N''Contactor trifásico 32 A'', N''unidad''),
    (10, N''REP-0010'', 9.0, N''Equipo de protección personal para trabajos eléctricos'', 5.0, N''Guantes dieléctricos clase 0'', N''par'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CurrentStock', N'Description', N'MinimumStock', N'Name', N'UnitOfMeasure') AND [object_id] = OBJECT_ID(N'[SpareParts]'))
        SET IDENTITY_INSERT [SpareParts] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Specialties]'))
        SET IDENTITY_INSERT [Specialties] ON;
    EXEC(N'INSERT INTO [Specialties] ([Id], [Description], [Name])
    VALUES (1, N''Instalación y mantenimiento de motores, tableros y circuitos'', N''Electricidad industrial''),
    (2, N''Mantenimiento de máquinas, bombas y componentes mecánicos'', N''Mecánica industrial''),
    (3, N''Sistemas de refrigeración, aire acondicionado y manejo de refrigerantes'', N''Refrigeración y climatización''),
    (4, N''Diagnóstico, reparación y mantenimiento de estaciones de trabajo'', N''Equipo de cómputo''),
    (5, N''Calibración y trazabilidad de instrumentos de medición'', N''Metrología y calibración''),
    (6, N''Inspección de sistemas de emergencia y equipos de protección'', N''Seguridad ocupacional''),
    (7, N''Mantenimiento de bombas, cilindros y sistemas hidráulicos'', N''Sistemas hidráulicos''),
    (8, N''Conservación de edificios, mobiliario e instalaciones'', N''Infraestructura edilicia'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Specialties]'))
        SET IDENTITY_INSERT [Specialties] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'UPDATE [Suppliers] SET [GeneralConditions] = N''Atención de lunes a viernes; garantía de 90 días.''
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'UPDATE [Suppliers] SET [GeneralConditions] = N''Visitas programadas y garantía de mano de obra por 6 meses.''
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'ContactName', N'Email', N'GeneralConditions', N'IsActive', N'Name', N'Phone', N'TaxId') AND [object_id] = OBJECT_ID(N'[Suppliers]'))
        SET IDENTITY_INSERT [Suppliers] ON;
    EXEC(N'INSERT INTO [Suppliers] ([Id], [Address], [ContactName], [Email], [GeneralConditions], [IsActive], [Name], [Phone], [TaxId])
    VALUES (3, N''Cartago'', N''Laura Araya'', N''ventas@electromecanicos.example.com'', N''Entrega en 48 horas y crédito institucional a 30 días.'', CAST(1 AS bit), N''Suministros Electromecánicos del Este'', N''2200-3000'', N''3-101-000003''),
    (4, N''Alajuela'', N''Andrés Pacheco'', N''contacto@seguridad.example.com'', N''Certificados de conformidad incluidos en cada entrega.'', CAST(1 AS bit), N''Soluciones de Seguridad Ocupacional'', N''2200-4000'', N''3-101-000004''),
    (5, N''San José'', N''Paola León'', N''soporte@oficina.example.com'', N''Garantía del fabricante y soporte técnico local.'', CAST(1 AS bit), N''Tecnología y Oficina Central'', N''2200-5000'', N''3-101-000005''),
    (6, N''San José'', N''Ricardo Solís'', N''flotas@repuestos.example.com'', N''Repuestos originales; entrega a flotilla sin costo.'', CAST(1 AS bit), N''Repuestos La Uruca'', N''2200-6000'', N''3-101-000006''),
    (7, N''Heredia'', N''Verónica Rojas'', N''calidad@instrumentacion.example.com'', N''Trazabilidad metrológica y certificado de calibración.'', CAST(1 AS bit), N''Instrumentación Científica Tica'', N''2200-7000'', N''3-101-000007''),
    (8, N''Puntarenas'', N''José Brenes'', N''servicios@edificios.example.com'', N''Atención de emergencias con cobertura nacional.'', CAST(1 AS bit), N''Servicios Integrales de Edificios'', N''2200-8000'', N''3-101-000008''),
    (9, N''Alajuela'', N''Mónica Quesada'', N''refrigeracion@valle.example.com'', N''Manejo y disposición de refrigerantes certificados.'', CAST(1 AS bit), N''Refrigeración del Valle'', N''2200-9000'', N''3-101-000009''),
    (10, N''Limón'', N''Felipe Chacón'', N''hidraulica@sistemas.example.com'', N''Pruebas de presión y garantía por 12 meses.'', CAST(1 AS bit), N''Sistemas Hidráulicos Nacionales'', N''2210-1000'', N''3-101-000010'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'ContactName', N'Email', N'GeneralConditions', N'IsActive', N'Name', N'Phone', N'TaxId') AND [object_id] = OBJECT_ID(N'[Suppliers]'))
        SET IDENTITY_INSERT [Suppliers] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'IsAvailable', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] ON;
    EXEC(N'INSERT INTO [Technicians] ([Id], [Email], [FullName], [HourlyRate], [Identification], [IsAvailable], [Phone])
    VALUES (1, N''ana.valverde@example.com'', N''Ana Lucía Valverde'', 8500.0, N''1-1020-0101'', CAST(1 AS bit), N''8888-1001''),
    (2, N''luis.mora@example.com'', N''Luis Fernando Mora'', 9200.0, N''1-1020-0102'', CAST(1 AS bit), N''8888-1002'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'IsAvailable', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] ON;
    EXEC(N'INSERT INTO [Technicians] ([Id], [Email], [FullName], [HourlyRate], [Identification], [Phone])
    VALUES (3, N''sofia.jimenez@example.com'', N''Sofía Jiménez Araya'', 8800.0, N''1-1020-0103'', N''8888-1003'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'IsAvailable', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] ON;
    EXEC(N'INSERT INTO [Technicians] ([Id], [Email], [FullName], [HourlyRate], [Identification], [IsAvailable], [Phone])
    VALUES (4, N''diego.rojas@example.com'', N''Diego Rojas Campos'', 10500.0, N''1-1020-0104'', CAST(1 AS bit), N''8888-1004''),
    (5, N''gabriela.vargas@example.com'', N''Gabriela Vargas León'', 9700.0, N''1-1020-0105'', CAST(1 AS bit), N''8888-1005'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'IsAvailable', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] ON;
    EXEC(N'INSERT INTO [Technicians] ([Id], [Email], [FullName], [HourlyRate], [Identification], [Phone])
    VALUES (6, N''esteban.chaves@example.com'', N''Esteban Chaves Solano'', 11000.0, N''1-1020-0106'', N''8888-1006'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'IsAvailable', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] ON;
    EXEC(N'INSERT INTO [Technicians] ([Id], [Email], [FullName], [HourlyRate], [Identification], [IsAvailable], [Phone])
    VALUES (7, N''natalia.soto@example.com'', N''Natalia Soto Méndez'', 9000.0, N''1-1020-0107'', CAST(1 AS bit), N''8888-1007''),
    (8, N''carlos.mendez@example.com'', N''Carlos Méndez Quesada'', 9800.0, N''1-1020-0108'', CAST(1 AS bit), N''8888-1008'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'IsAvailable', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] ON;
    EXEC(N'INSERT INTO [Technicians] ([Id], [Email], [FullName], [HourlyRate], [Identification], [Phone])
    VALUES (9, N''irene.campos@example.com'', N''Irene Campos Ureña'', 10200.0, N''1-1020-0109'', N''8888-1009'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'IsAvailable', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] ON;
    EXEC(N'INSERT INTO [Technicians] ([Id], [Email], [FullName], [HourlyRate], [Identification], [IsAvailable], [Phone])
    VALUES (10, N''marco.salas@example.com'', N''Marco Salas Rodríguez'', 8700.0, N''1-1020-0110'', CAST(1 AS bit), N''8888-1010'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'HourlyRate', N'Identification', N'IsAvailable', N'Phone') AND [object_id] = OBJECT_ID(N'[Technicians]'))
        SET IDENTITY_INSERT [Technicians] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AcquiredOn', N'AssetCode', N'Brand', N'CategoryId', N'CreatedAt', N'Criticality', N'Description', N'ImageUrl', N'IsActive', N'LocationId', N'Model', N'Name', N'PurchaseCost', N'SerialNumber', N'Status', N'SupplierId', N'UsageHours', N'UsefulLifeMonths') AND [object_id] = OBJECT_ID(N'[Assets]'))
        SET IDENTITY_INSERT [Assets] ON;
    EXEC(N'INSERT INTO [Assets] ([Id], [AcquiredOn], [AssetCode], [Brand], [CategoryId], [CreatedAt], [Criticality], [Description], [ImageUrl], [IsActive], [LocationId], [Model], [Name], [PurchaseCost], [SerialNumber], [Status], [SupplierId], [UsageHours], [UsefulLifeMonths])
    VALUES (4, ''2021-06-12T00:00:00.0000000'', N''ACT-0004'', N''Cummins'', 5, ''2021-06-12T00:00:00.0000000Z'', N''Crítica'', N''Generador diésel para respaldo eléctrico del edificio'', NULL, CAST(1 AS bit), 4, N''C90D5'', N''Generador de emergencia'', 8500000.0, N''SN-CUM-0400'', N''Operativo'', 3, 740.0, 180),
    (5, ''2020-02-20T00:00:00.0000000'', N''ACT-0005'', N''Grundfos'', 4, ''2020-02-20T00:00:00.0000000Z'', N''Crítica'', N''Bomba principal del sistema de protección contra incendios'', NULL, CAST(1 AS bit), 3, N''NK 50-200'', N''Bomba contra incendios'', 3200000.0, N''SN-GRU-0500'', N''Operativo'', 4, 2150.0, 180),
    (6, ''2022-11-04T00:00:00.0000000'', N''ACT-0006'', N''Daikin'', 6, ''2022-11-04T00:00:00.0000000Z'', N''Alta'', N''Unidad de climatización de precisión'', NULL, CAST(1 AS bit), 5, N''FTXM36'', N''Aire acondicionado del laboratorio'', 745000.0, N''SN-DAI-0600'', N''Operativo'', 9, 9600.0, 120),
    (7, ''2023-05-09T00:00:00.0000000'', N''ACT-0007'', N''Toyota'', 7, ''2023-05-09T00:00:00.0000000Z'', N''Alta'', N''Vehículo para traslado del personal de mantenimiento'', NULL, CAST(1 AS bit), 7, N''Hiace'', N''Microbús de servicio'', 18500000.0, N''SN-TOY-0700'', N''Operativo'', 6, 3250.0, 120),
    (8, ''2021-09-01T00:00:00.0000000'', N''ACT-0008'', N''Eppendorf'', 8, ''2021-09-01T00:00:00.0000000Z'', N''Alta'', N''Centrífuga refrigerada para control de calidad'', NULL, CAST(1 AS bit), 5, N''5804 R'', N''Centrífuga de laboratorio'', 4250000.0, N''SN-EPP-0800'', N''Operativo'', 7, 5100.0, 120),
    (9, ''2019-07-15T00:00:00.0000000'', N''ACT-0009'', N''Schneider Electric'', 5, ''2019-07-15T00:00:00.0000000Z'', N''Crítica'', N''Tablero de distribución de baja tensión'', NULL, CAST(1 AS bit), 4, N''PrismaSeT'', N''Tablero eléctrico principal'', 2700000.0, N''SN-SCH-0900'', N''Operativo'', 3, 14500.0, 240),
    (10, ''2022-04-18T00:00:00.0000000'', N''ACT-0010'', N''Toyota'', 3, ''2022-04-18T00:00:00.0000000Z'', N''Alta'', N''Montacargas para recepción y despacho de suministros'', NULL, CAST(1 AS bit), 6, N''8FBE20'', N''Montacargas eléctrico'', 12000000.0, N''SN-TOY-1000'', N''Operativo'', 10, 3890.0, 180)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AcquiredOn', N'AssetCode', N'Brand', N'CategoryId', N'CreatedAt', N'Criticality', N'Description', N'ImageUrl', N'IsActive', N'LocationId', N'Model', N'Name', N'PurchaseCost', N'SerialNumber', N'Status', N'SupplierId', N'UsageHours', N'UsefulLifeMonths') AND [object_id] = OBJECT_ID(N'[Assets]'))
        SET IDENTITY_INSERT [Assets] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'SparePartId', N'SupplierId', N'EstimatedDeliveryDays', N'OfferedPrice') AND [object_id] = OBJECT_ID(N'[PartSuppliers]'))
        SET IDENTITY_INSERT [PartSuppliers] ON;
    EXEC(N'INSERT INTO [PartSuppliers] ([SparePartId], [SupplierId], [EstimatedDeliveryDays], [OfferedPrice])
    VALUES (1, 3, 2, 14500.0),
    (1, 10, 3, 15200.0),
    (2, 2, 1, 8700.0),
    (2, 3, 2, 9100.0),
    (3, 3, 2, 6300.0),
    (3, 8, 4, 6800.0),
    (4, 2, 1, 18400.0),
    (5, 5, 3, 49500.0),
    (6, 8, 2, 28200.0),
    (6, 10, 4, 26700.0),
    (7, 7, 5, 41500.0),
    (8, 10, 1, 4400.0),
    (9, 3, 2, 22000.0),
    (9, 8, 3, 23600.0),
    (10, 4, 2, 18500.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'SparePartId', N'SupplierId', N'EstimatedDeliveryDays', N'OfferedPrice') AND [object_id] = OBJECT_ID(N'[PartSuppliers]'))
        SET IDENTITY_INSERT [PartSuppliers] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'SpecialtyId', N'TechnicianId', N'CertificationLevel', N'CertifiedSince') AND [object_id] = OBJECT_ID(N'[TechnicianSpecialties]'))
        SET IDENTITY_INSERT [TechnicianSpecialties] ON;
    EXEC(N'INSERT INTO [TechnicianSpecialties] ([SpecialtyId], [TechnicianId], [CertificationLevel], [CertifiedSince])
    VALUES (1, 1, N''Avanzado'', ''2018-03-01T00:00:00.0000000''),
    (6, 1, N''Intermedio'', ''2020-06-15T00:00:00.0000000''),
    (2, 2, N''Avanzado'', ''2017-08-10T00:00:00.0000000''),
    (7, 2, N''Intermedio'', ''2021-02-01T00:00:00.0000000''),
    (3, 3, N''Avanzado'', ''2019-05-20T00:00:00.0000000''),
    (1, 4, N''Intermedio'', ''2022-01-10T00:00:00.0000000''),
    (2, 4, N''Intermedio'', ''2023-04-03T00:00:00.0000000''),
    (5, 5, N''Avanzado'', ''2016-09-14T00:00:00.0000000''),
    (7, 6, N''Avanzado'', ''2018-11-05T00:00:00.0000000''),
    (4, 7, N''Avanzado'', ''2020-02-12T00:00:00.0000000''),
    (6, 8, N''Avanzado'', ''2015-07-01T00:00:00.0000000''),
    (8, 8, N''Intermedio'', ''2019-10-01T00:00:00.0000000''),
    (3, 9, N''Intermedio'', ''2021-01-20T00:00:00.0000000''),
    (2, 10, N''Intermedio'', ''2022-08-02T00:00:00.0000000''),
    (8, 10, N''Avanzado'', ''2017-04-18T00:00:00.0000000'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'SpecialtyId', N'TechnicianId', N'CertificationLevel', N'CertifiedSince') AND [object_id] = OBJECT_ID(N'[TechnicianSpecialties]'))
        SET IDENTITY_INSERT [TechnicianSpecialties] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AssetId', N'CompletedAt', N'Cost', N'CreatedAt', N'Description', N'Findings', N'MaintenanceTypeId', N'Priority', N'RequestedAt', N'ScheduledFor', N'StartedAt', N'Status', N'SupplierId', N'Title', N'WorkOrderCode') AND [object_id] = OBJECT_ID(N'[MaintenanceOrders]'))
        SET IDENTITY_INSERT [MaintenanceOrders] ON;
    EXEC(N'INSERT INTO [MaintenanceOrders] ([Id], [AssetId], [CompletedAt], [Cost], [CreatedAt], [Description], [Findings], [MaintenanceTypeId], [Priority], [RequestedAt], [ScheduledFor], [StartedAt], [Status], [SupplierId], [Title], [WorkOrderCode])
    VALUES (4, 4, NULL, NULL, ''2026-09-12T00:00:00.0000000Z'', N''Prueba de carga, cambio de filtros y revisión de baterías'', NULL, 4, N''Urgent'', ''2026-09-12T00:00:00.0000000'', ''2026-10-25T00:00:00.0000000'', NULL, N''Solicitada'', 3, N''Servicio semestral del generador'', N''OT-2026-0004''),
    (5, 5, NULL, NULL, ''2026-09-17T00:00:00.0000000Z'', N''Verificar caudal, presión y sello mecánico'', NULL, 6, N''High'', ''2026-09-17T00:00:00.0000000'', NULL, NULL, N''Solicitada'', 4, N''Prueba de presión de bomba contra incendios'', N''OT-2026-0005''),
    (6, 6, NULL, NULL, ''2026-09-21T00:00:00.0000000Z'', N''Limpieza de serpentines y verificación del drenaje'', NULL, 7, N''Medium'', ''2026-09-21T00:00:00.0000000'', ''2026-10-30T00:00:00.0000000'', NULL, N''Solicitada'', 9, N''Limpieza de unidad de precisión'', N''OT-2026-0006''),
    (7, 7, NULL, NULL, ''2026-09-23T00:00:00.0000000Z'', N''Revisión de frenos, neumáticos y niveles de fluidos'', NULL, 3, N''High'', ''2026-09-23T00:00:00.0000000'', NULL, NULL, N''Solicitada'', 6, N''Inspección de flotilla'', N''OT-2026-0007''),
    (8, 8, NULL, NULL, ''2026-09-25T00:00:00.0000000Z'', N''Verificar velocidad y temperatura con patrones certificados'', NULL, 5, N''Medium'', ''2026-09-25T00:00:00.0000000'', ''2026-11-10T00:00:00.0000000'', NULL, N''Solicitada'', 7, N''Calibración anual de centrífuga'', N''OT-2026-0008''),
    (9, 9, NULL, NULL, ''2026-09-27T00:00:00.0000000Z'', N''Inspección termográfica de conexiones y barras'', NULL, 4, N''Urgent'', ''2026-09-27T00:00:00.0000000'', NULL, NULL, N''Solicitada'', 3, N''Termografía del tablero principal'', N''OT-2026-0009''),
    (10, 10, NULL, NULL, ''2026-09-29T00:00:00.0000000Z'', N''Verificar mangueras, cilindros y nivel de aceite'', NULL, 7, N''High'', ''2026-09-29T00:00:00.0000000'', ''2026-10-28T00:00:00.0000000'', NULL, N''Solicitada'', 10, N''Revisión de sistema hidráulico'', N''OT-2026-0010'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AssetId', N'CompletedAt', N'Cost', N'CreatedAt', N'Description', N'Findings', N'MaintenanceTypeId', N'Priority', N'RequestedAt', N'ScheduledFor', N'StartedAt', N'Status', N'SupplierId', N'Title', N'WorkOrderCode') AND [object_id] = OBJECT_ID(N'[MaintenanceOrders]'))
        SET IDENTITY_INSERT [MaintenanceOrders] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'ALTER TABLE [MaintenanceOrders] ADD CONSTRAINT [CK_MaintenanceOrders_Status] CHECK ([Status] IN (''Solicitada'', ''Planned'', ''InProgress'', ''Completed'', ''Cancelled''))');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'ALTER TABLE [Assets] ADD CONSTRAINT [CK_Assets_Criticality] CHECK ([Criticality] IN (''Baja'', ''Media'', ''Alta'', ''Crítica''))');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'ALTER TABLE [Assets] ADD CONSTRAINT [CK_Assets_Status] CHECK ([Status] IN (''Operativo'', ''En Mantenimiento'', ''Fuera de Servicio'', ''Dado de Baja''))');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'ALTER TABLE [Assets] ADD CONSTRAINT [CK_Assets_UsageHours] CHECK ([UsageHours] >= 0)');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    EXEC(N'ALTER TABLE [AssetCategories] ADD CONSTRAINT [CK_AssetCategories_PreventiveMaintenanceIntervalDays] CHECK ([PreventiveMaintenanceIntervalDays] IS NULL OR [PreventiveMaintenanceIntervalDays] > 0)');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    CREATE INDEX [IX_PartSuppliers_SupplierId] ON [PartSuppliers] ([SupplierId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SpareParts_Code] ON [SpareParts] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Specialties_Name] ON [Specialties] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Technicians_Identification] ON [Technicians] ([Identification]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    CREATE INDEX [IX_TechnicianSpecialties_SpecialtyId] ON [TechnicianSpecialties] ([SpecialtyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021151258_CompleteAdvanceOne'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261021151258_CompleteAdvanceOne', N'10.0.12');
END;

COMMIT;
GO
