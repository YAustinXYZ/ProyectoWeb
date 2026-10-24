# Sube los cambios de la demo a una rama nueva del repo de Austin, en commits separados.
# Uso (PowerShell, dentro de esta carpeta):  powershell -ExecutionPolicy Bypass -File .\subir_demo_github.ps1
# No toca main: todo va a la rama feat/avance1-completo.

$repo  = "https://github.com/YAustinXYZ/ProyectoWeb.git"
$rama  = "feat/avance1-completo"
$base  = "main"

function Paso($texto) { Write-Host "`n== $texto" -ForegroundColor Cyan }
function Fallo($texto) { Write-Host "ERROR: $texto" -ForegroundColor Red; exit 1 }

Set-Location -Path $PSScriptRoot

if (Test-Path .git) { Fallo "Esta carpeta ya tiene .git. Si es de un intento anterior, bórrala y vuelve a ejecutar." }

Paso "Conectando la carpeta con $repo"
git init | Out-Null
git config core.autocrlf input            # El repo está en LF: no convertir finales de línea
git remote add origin $repo
git fetch origin $base
if ($LASTEXITCODE -ne 0) { Fallo "No se pudo descargar $base. Revisa que tengas acceso al repo (inicia sesión en GitHub si lo pide)." }

# La carpeta queda encima del último main del repo, con nuestros cambios sin confirmar
git checkout -b $rama 2>$null | Out-Null
git reset --mixed "origin/$base" | Out-Null
if ($LASTEXITCODE -ne 0) { Fallo "No se pudo ubicar la rama en origin/$base." }

# Notas personales que no van al repo de Austin
Add-Content .git/info/exclude "PROMPTS_REPO_REAL.md"
Add-Content .git/info/exclude "Prompts_Avance1_Repo_Real.pdf"
Add-Content .git/info/exclude "subir_demo_github.ps1"

Paso "Cambios detectados contra origin/$base"
git status --short
Write-Host "`nRevisa la lista: deben ser solo archivos que tocamos en la demo." -ForegroundColor Yellow
Write-Host "Si aparece algo que NO tocamos (por ejemplo trabajo nuevo de Austin marcado como borrado), escribe n y pégale la lista a Claude." -ForegroundColor Yellow
if ((Read-Host "¿Continuar con los commits? (s/n)") -ne "s") { Write-Host "Cancelado. No se hizo ningún commit."; exit 0 }

function Commit($mensaje, [string[]]$rutas) {
    # Una ruta a la vez: si una no existe, git no descarta las demás
    foreach ($ruta in $rutas) { git add -A -- $ruta 2>$null }
    git diff --cached --quiet
    if ($LASTEXITCODE -eq 0) { Write-Host "  (sin cambios para: $mensaje)"; return }
    git commit -m $mensaje | Out-Null
    if ($LASTEXITCODE -ne 0) { Fallo "No se pudo hacer el commit '$mensaje'. Revisa git config user.name y user.email." }
    Write-Host "  commit: $mensaje" -ForegroundColor Green
}

Paso "Creando commits"
Commit "feat: add business exceptions and global exception handler" @(
    "Proyecto.Domain/Excepciones", "Proyecto.Domain/DTO/ResultadoPatron",
    "Proyecto.WebAPI/Middleware", "Proyecto.WebAPI/Program.cs",
    "Proyecto.WebAPI/Proyecto.WebAPI.csproj", "Proyecto.WebAPI/appsettings.json")

Commit "feat: add spare parts with supplier pricing" @(
    "Proyecto.Domain/Entities/Repuesto.cs", "Proyecto.Domain/Entities/RepuestoProveedor.cs",
    "Proyecto.Domain/Entities/Proveedor.cs", "Proyecto.Infraestructure/Data/ContextoAplicacion.cs",
    "Proyecto.Domain/DTO/Repuesto*", "Proyecto.Domain/DTO/ProveedorRepuestoDto.cs",
    "Proyecto.Domain/DTO/CondicionesProveedorRepuestoDto.cs", "Proyecto.Domain/DTO/ProveedorDetalleDto.cs",
    "Proyecto.Infraestructure/Implementation/RepuestoRepository.cs", "Proyecto.Infraestructure/Interfaces/IRepuestoRepository.cs",
    "Proyecto.Infraestructure/Implementation/ProveedorRepository.cs", "Proyecto.Infraestructure/Interfaces/IProveedorRepository.cs",
    "Proyecto.Application/Implementation/RepuestoService.cs", "Proyecto.Application/Interfaces/IRepuestoService.cs",
    "Proyecto.Application/Implementation/ProveedorService.cs",
    "Proyecto.WebAPI/Controllers/RepuestoController.cs", "Proyecto.WebAPI/Controllers/ProveedorController.cs")

Commit "feat: add technician and specialty management" @(
    "Proyecto.Domain/Validaciones", "Proyecto.Domain/DTO/Tecnico*", "Proyecto.Domain/DTO/Especialidad*",
    "Proyecto.Domain/DTO/SolicitudEspecialidadTecnico*", "Proyecto.Domain/DTO/UbicacionListaDto.cs",
    "Proyecto.Domain/DTO/TipoMantenimientoListaDto.cs",
    "Proyecto.Infraestructure/Implementation/TecnicoRepository.cs", "Proyecto.Infraestructure/Interfaces/ITecnicoRepository.cs",
    "Proyecto.Infraestructure/Implementation/EspecialidadRepository.cs", "Proyecto.Infraestructure/Interfaces/IEspecialidadRepository.cs",
    "Proyecto.Infraestructure/Implementation/CatalogoRepository.cs", "Proyecto.Infraestructure/Interfaces/ICatalogoRepository.cs",
    "Proyecto.Application/Implementation/TecnicoService.cs", "Proyecto.Application/Interfaces/ITecnicoService.cs",
    "Proyecto.Application/Implementation/EspecialidadService.cs", "Proyecto.Application/Interfaces/IEspecialidadService.cs",
    "Proyecto.Application/Implementation/CatalogoService.cs", "Proyecto.Application/Interfaces/ICatalogoService.cs",
    "Proyecto.WebAPI/Controllers/TecnicosController.cs", "Proyecto.WebAPI/Controllers/EspecialidadesController.cs",
    "Proyecto.WebAPI/Controllers/CatalogosController.cs")

Commit "feat: add asset, category and order computed fields" @(
    "Proyecto.Domain/DTO", "Proyecto.Domain/Entities",
    "Proyecto.Infraestructure/Implementation", "Proyecto.Infraestructure/Interfaces",
    "Proyecto.Application", "Proyecto.WebAPI/Controllers")

Commit "feat: seed it data center catalog with edge cases" @("Proyecto.Infraestructure/Data/Seed")

Commit "feat: add spare parts and seed data migration" @("Proyecto.Infraestructure/Migrations")

Commit "test: add business rule tests and update readme" @("Proyecto.WebAPI.Tests", "README.md", "database")

Paso "Lo que queda sin commit"
git status --short
$resto = git status --porcelain
if ($resto) {
    Write-Host "Hay archivos fuera de los grupos anteriores. No se suben; revísalos con Claude si hace falta." -ForegroundColor Yellow
}

Paso "Commits de la rama $rama"
git log --oneline "origin/$base..HEAD"

if ((Read-Host "`n¿Hacer push de $rama a GitHub? (s/n)") -ne "s") { Write-Host "Listo sin push. Cuando quieras: git push -u origin $rama"; exit 0 }
git push -u origin $rama
if ($LASTEXITCODE -ne 0) { Fallo "El push falló. Si es por permisos, pídele a Austin que te agregue como colaborador del repo." }
Write-Host "`nListo. Abre $($repo -replace '\.git$','')/pull/new/$rama para crear el Pull Request." -ForegroundColor Green
