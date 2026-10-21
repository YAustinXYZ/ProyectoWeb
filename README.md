# Mantenimiento de activos

API REST para administrar el inventario de activos, categorías, técnicos y sus especialidades, repuestos, proveedores y solicitudes de mantenimiento. El alcance implementado corresponde al Avance 1 de ISW-621; no incluye frontend ni flujo de estados posterior de las órdenes.

## Requisitos

- .NET SDK 10
- SQL Server o SQL Server LocalDB
- Una cadena de conexión SQL Server accesible desde la máquina

## Ejecutar en desarrollo

Desde la raíz de este repositorio:

```powershell
dotnet run --project .\Proyecto.WebAPI\Proyecto.WebAPI.csproj --launch-profile http
```

El perfil `http` usa `http://localhost:5022` y el entorno `Development`. En ese entorno, la API aplica las migraciones de EF Core al arrancar; si la base todavía no existe, SQL Server la crea. Los catálogos y registros de demostración se cargan mediante `HasData` dentro de las migraciones.

La cadena LocalDB predeterminada está en `Proyecto.WebAPI/appsettings.Development.json`. Para usar otra instancia, define `ConnectionStrings__DefaultConnection` antes de ejecutar la API. En otros entornos, aplica la migración explícitamente:

```powershell
dotnet ef database update `
  --project .\Proyecto.Infraestructure\Proyecto.Infraestructure.csproj `
  --startup-project .\Proyecto.WebAPI\Proyecto.WebAPI.csproj
```

La documentación interactiva está disponible en [Scalar](http://localhost:5022/scalar/v1); OpenAPI en `http://localhost:5022/openapi/v1.json`. También se incluye [Proyecto.WebAPI.http](./Proyecto.WebAPI/Proyecto.WebAPI.http) con solicitudes de ejemplo.

## Arquitectura

- `Proyecto.Domain`: entidades y excepciones de negocio.
- `Proyecto.Application`: contratos de servicios, DTOs de entrada/salida y proyecciones de respuesta.
- `Proyecto.Infraestructure`: `AppDbContext`, migraciones, datos iniciales e implementación de servicios con EF Core.
- `Proyecto.WebAPI`: controladores REST, validación HTTP, documentación Scalar y middleware global de excepciones.

Los controladores dependen de `IAssetManagementService`; no consultan el `DbContext`. El esquema se define en las entidades y `AppDbContext`, y se versiona mediante migraciones EF Core. `database/MantenimientoActivos.sql` es el script idempotente generado a partir de esas migraciones, no una definición independiente del modelo.

## Recursos REST

| Recurso | Operaciones |
| --- | --- |
| `/api/assets` | Listar por categoría/estado, consultar detalle, crear, actualizar, cambiar estado administrativo, dar de baja lógica y actualizar la URL de imagen. |
| `/api/asset-categories` | Listar, consultar detalle, crear, actualizar y eliminar (no permite eliminar categorías con activos asociados). |
| `/api/technicians` | Listar por especialidad/disponibilidad, consultar, crear, actualizar y asociar/desasociar especialidades. |
| `/api/specialties` | Listar, consultar detalle, crear y actualizar. |
| `/api/spare-parts` | Listar, consultar bajo mínimo, consultar detalle, crear/actualizar, asociar proveedor y cambiar sus condiciones. |
| `/api/suppliers` | Listar, consultar detalle, crear y actualizar. |
| `/api/maintenance-orders` | Crear una solicitud, listar (con filtro por activo) y consultar detalle. |
| `/api/locations`, `/api/maintenance-types` | Consultar catálogos para asignar activos y crear solicitudes. |

Los códigos siguen los formatos `ACT-0001`, `REP-0001` y `OT-2026-0001`. Los estados de activo son `Operativo`, `En Mantenimiento`, `Fuera de Servicio` y `Dado de Baja`. El endpoint de estado administrativo solo acepta `Fuera de Servicio` o `Dado de Baja`, y bloquea el cambio si hay una orden abierta. La baja lógica conserva el registro y no está permitida cuando existen órdenes asociadas.

Las órdenes nuevas siempre se guardan con estado `Solicitada`; el DTO de creación no permite indicar un estado ni implementar transiciones. Las imágenes representativas se registran como URL con `PUT /api/assets/{id}/image`.

## Respuestas, validación y logging

Las respuestas exitosas usan `Result<T>`. Los errores tienen un formato uniforme e incluyen `status`, `code`, `message`, `timestamp`, `traceId` y, para errores de validación de entrada, `errors` por campo. Ejemplo:

```json
{
  "status": 400,
  "code": "REQUEST_VALIDATION_FAILED",
  "message": "Uno o más campos de la solicitud no son válidos.",
  "timestamp": "2026-10-21T15:00:00+00:00",
  "traceId": "0HNPFGPKNEV5O:00000001",
  "errors": {
    "AssetCode": ["Use el formato ACT-0001."]
  }
}
```

Las validaciones de formato usan Data Annotations; las reglas de negocio se aplican en los servicios. El middleware global traduce excepciones de negocio a 400, 404 o 409 y registra errores inesperados con su stack trace. Los controladores registran inicio y final de las operaciones con `ILogger` y `TraceId`.

Los campos derivados no se persisten ni se aceptan en solicitudes: edad del activo en meses, nombre de categoría, conteos de relaciones, antigüedad de certificaciones, indicadores/faltante de stock y días desde la solicitud. Los conteos de listados se calculan en consultas LINQ a SQL Server. Los detalles devuelven DTOs con sus relaciones resueltas.

## Datos de demostración

La migración inicializa 8 categorías, 8 ubicaciones, 10 proveedores, 10 activos, 8 tipos de mantenimiento, 10 órdenes, 10 técnicos, 8 especialidades y 10 repuestos, junto con las relaciones Técnico–Especialidad y Repuesto–Proveedor. Incluye activos fuera de servicio, técnicos con varias especialidades y repuestos por debajo del mínimo.

## Compilar y validar

```powershell
dotnet restore .\Proyecto.slnx
dotnet build .\Proyecto.slnx
dotnet ef migrations list `
  --project .\Proyecto.Infraestructure\Proyecto.Infraestructure.csproj `
  --startup-project .\Proyecto.WebAPI\Proyecto.WebAPI.csproj
```
