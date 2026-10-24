# Proyecto Mantenimiento de Activos

API para administrar activos de TI, categorías, ubicaciones, proveedores, repuestos, técnicos, especialidades y órdenes de mantenimiento.

## Requisitos

- .NET SDK 10.
- SQL Server o SQL Server LocalDB.

## Base de datos

El modelo es Model First con Entity Framework Core: las entidades y `ContextoAplicacion` definen las tablas, y las migraciones de `Proyecto.Infraestructure/Migrations` las crean. En desarrollo, la API aplica las migraciones al arrancar (`MigrateAsync`) y los datos semilla de `Data/Seed/InicializadorCatalogos.cs` viajan dentro de las migraciones, así que no hay que insertar nada a mano.

Para recrear la base desde cero, desde la carpeta de la solución:

```powershell
dotnet ef database drop --project Proyecto.Infraestructure --startup-project Proyecto.WebAPI -- --environment Development
dotnet ef database update --project Proyecto.Infraestructure --startup-project Proyecto.WebAPI -- --environment Development
```

La cadena de conexión de desarrollo está en `Proyecto.WebAPI/appsettings.Development.json`.

## Compilar y probar

```powershell
dotnet build Proyecto.slnx
dotnet test Proyecto.slnx
```

La documentación de los endpoints está en Scalar: `/scalar` al ejecutar en Development.

## Respuestas y errores

Todas las respuestas usan `Resultado<T>` (`isSuccess`, `statusCode`, `title`, `detail`, `timestamp`, `data`). Los errores incluyen además `traceId`, el mismo identificador que queda en el log (`Proyecto.WebAPI/Logs`). Las reglas de negocio lanzan excepciones propias (`ExcepcionSolicitudInvalida` 400, `ExcepcionNoEncontrado` 404, `ExcepcionConflicto` 409) que traduce `ManejadorGlobalExcepciones`.

## Endpoints

| Recurso | Rutas |
| --- | --- |
| Activos | `GET/POST /api/Asset` (filtros `categoryId` y `status`), `GET/PUT/DELETE /api/Asset/{id}`, `PATCH /api/Asset/{id}/status`, `POST/DELETE /api/Asset/{id}/image` |
| Categorías | `GET/POST /api/AssetCategory`, `GET/PUT/DELETE /api/AssetCategory/{id}` |
| Proveedores | `GET/POST /api/Supplier`, `GET/PUT/DELETE /api/Supplier/{id}` |
| Repuestos | `GET/POST /api/SparePart`, `GET /api/SparePart/below-minimum`, `GET/PUT /api/SparePart/{id}`, `POST/PUT /api/SparePart/{id}/suppliers/{supplierId}` |
| Técnicos | `GET/POST /api/technicians` (filtros `specialtyId` e `isAvailable`), `GET/PUT /api/technicians/{id}`, `POST/DELETE /api/technicians/{id}/specialties/{specialtyId}` |
| Especialidades | `GET/POST /api/specialties`, `GET/PUT /api/specialties/{id}` |
| Órdenes | `GET/POST /api/MaintenanceOrder`, `GET /api/MaintenanceOrder/{id}`, `GET /api/MaintenanceOrder/by-asset/{assetId}` |
| Catálogos | `GET /api/locations`, `GET /api/maintenance-types` |

Las listas principales también tienen paginación: `GET /api/{recurso}/{pageNumber}/{pageSize}`.
