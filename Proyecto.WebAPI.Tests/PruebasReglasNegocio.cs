using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Proyecto.WebAPI.Tests;

// Verifica las reglas de la rúbrica sobre los datos semilla (InMemory aplica HasData con EnsureCreated)
public sealed class PruebasReglasNegocio
{
    private static async Task<(HttpStatusCode Estado, JsonElement Cuerpo)> ObtenerAsync(HttpClient cliente, string ruta)
    {
        using var respuesta = await cliente.GetAsync(ruta);
        using var documento = await JsonDocument.ParseAsync(await respuesta.Content.ReadAsStreamAsync());
        return (respuesta.StatusCode, documento.RootElement.Clone());
    }

    [Fact]
    public async Task ErrorDeNegocio_DevuelveTraceIdYMarcaDeTiempo()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        var (estado, cuerpo) = await ObtenerAsync(cliente, "/api/assets/9999");

        Assert.Equal(HttpStatusCode.NotFound, estado);
        Assert.False(cuerpo.GetProperty("isSuccess").GetBoolean());
        Assert.False(string.IsNullOrWhiteSpace(cuerpo.GetProperty("traceId").GetString()));
        Assert.True(cuerpo.TryGetProperty("timestamp", out _));
    }

    [Fact]
    public async Task ListadoActivos_FiltraPorEstadoYRechazaEstadoInvalido()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        var (estado, cuerpo) = await ObtenerAsync(cliente, "/api/assets?status=UnderMaintenance");
        Assert.Equal(HttpStatusCode.OK, estado);
        Assert.All(cuerpo.GetProperty("data").EnumerateArray(),
            activo => Assert.Equal("UnderMaintenance", activo.GetProperty("status").GetString()));

        var (estadoInvalido, _) = await ObtenerAsync(cliente, "/api/assets?status=Roto");
        Assert.Equal(HttpStatusCode.BadRequest, estadoInvalido);
    }

    [Fact]
    public async Task DetalleActivo_ConteoDeOrdenesCoincideConElResumen()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        var (_, cuerpo) = await ObtenerAsync(cliente, "/api/assets/1");
        var datos = cuerpo.GetProperty("data");

        Assert.Equal(datos.GetProperty("maintenanceOrders").GetArrayLength(), datos.GetProperty("maintenanceOrderCount").GetInt32());
        Assert.True(datos.GetProperty("category").GetProperty("preventiveThresholdHours").GetInt32() > 0);
        Assert.True(datos.GetProperty("ageInMonths").GetInt32() >= 0);
    }

    [Fact]
    public async Task CambioEstado_ConOrdenAbierta_Responde409()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        // NET-0002 (id 4) tiene la orden OT-2026-0003 en Requested
        using var respuesta = await cliente.PatchAsJsonAsync("/api/assets/4/status", new { status = "OutOfService" });

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
    }

    [Fact]
    public async Task ActivoDadoDeBaja_NoSeListaPorDefectoYEditarloResponde404()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        // PC-0001 (id 7) está sembrado como Decommissioned
        var (_, listado) = await ObtenerAsync(cliente, "/api/assets");
        Assert.DoesNotContain(listado.GetProperty("data").EnumerateArray(),
            activo => activo.GetProperty("status").GetString() == "Decommissioned");

        var (_, bajas) = await ObtenerAsync(cliente, "/api/assets?status=Decommissioned");
        Assert.Contains(bajas.GetProperty("data").EnumerateArray(), activo => activo.GetProperty("id").GetInt32() == 7);

        using var baja = await cliente.DeleteAsync("/api/assets/7");
        Assert.Equal(HttpStatusCode.NotFound, baja.StatusCode);

        using var estado = await cliente.PatchAsJsonAsync("/api/assets/7/status", new { status = "Operational" });
        Assert.Equal(HttpStatusCode.NotFound, estado.StatusCode);
    }

    [Fact]
    public async Task FechaFutura_EsRechazadaPorModelState()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        using var respuesta = await cliente.PostAsJsonAsync("/api/assets", new
        {
            name = "Activo con fecha futura",
            acquiredOn = DateTime.UtcNow.AddDays(10),
            purchaseCost = 1000m,
            usefulLifeMonths = 12,
            usageHours = 0,
            criticality = "Low",
            categoryId = 1,
            locationId = 1
        });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task CrearOrden_ConTipoInexistente_Responde400()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        using var respuesta = await cliente.PostAsJsonAsync("/api/maintenance-orders", new
        {
            assetId = 1,
            maintenanceTypeId = 9999,
            title = "Orden de prueba",
            reason = "Tipo inexistente",
            priority = "Low"
        });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task ListadoOrdenes_FiltraPorActivo()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        var (estado, cuerpo) = await ObtenerAsync(cliente, "/api/maintenance-orders?assetId=1");
        Assert.Equal(HttpStatusCode.OK, estado);
        Assert.All(cuerpo.GetProperty("data").EnumerateArray(),
            orden => Assert.Equal("SRV-0001", orden.GetProperty("assetCode").GetString()));

        var (estadoInexistente, _) = await ObtenerAsync(cliente, "/api/maintenance-orders?assetId=9999");
        Assert.Equal(HttpStatusCode.NotFound, estadoInexistente);
    }

    [Fact]
    public async Task CrearOrden_ConActivoInexistente_Responde404()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        using var respuesta = await cliente.PostAsJsonAsync("/api/maintenance-orders", new
        {
            assetId = 9999,
            maintenanceTypeId = 1,
            title = "Orden de prueba",
            reason = "Activo inexistente",
            priority = "Low"
        });

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task RepuestosBajoMinimo_CoincidenConElIndicador()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        var (_, todos) = await ObtenerAsync(cliente, "/api/spare-parts");
        var (_, bajoMinimo) = await ObtenerAsync(cliente, "/api/spare-parts/below-minimum");

        var esperados = todos.GetProperty("data").EnumerateArray().Count(r => r.GetProperty("isBelowMinimum").GetBoolean());
        Assert.True(esperados > 0);
        Assert.Equal(esperados, bajoMinimo.GetProperty("data").GetArrayLength());
        Assert.All(todos.GetProperty("data").EnumerateArray(),
            repuesto => Assert.True(repuesto.GetProperty("shortage").GetInt32() >= 0));
    }

    [Fact]
    public async Task CrearRepuesto_ConStockNegativo_Responde400YCodigoDuplicado409()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        using var negativo = await cliente.PostAsJsonAsync("/api/spare-parts", new
        {
            code = "REP-TEST-01", name = "Repuesto de prueba", unitOfMeasure = "Unit", currentStock = -1, minimumStock = 1
        });
        Assert.Equal(HttpStatusCode.BadRequest, negativo.StatusCode);

        using var duplicado = await cliente.PostAsJsonAsync("/api/spare-parts", new
        {
            code = "REP-SSD-960", name = "Repuesto duplicado", unitOfMeasure = "Unit", currentStock = 1, minimumStock = 1
        });
        Assert.Equal(HttpStatusCode.Conflict, duplicado.StatusCode);
    }

    [Fact]
    public async Task Tecnicos_FiltroPorEspecialidadYConteoCoherente()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        var (_, filtrados) = await ObtenerAsync(cliente, "/api/technicians?specialtyId=1&isAvailable=true");
        Assert.All(filtrados.GetProperty("data").EnumerateArray(),
            tecnico => Assert.True(tecnico.GetProperty("isAvailable").GetBoolean()));

        // Laura Mora (id 1) tiene tres especialidades sembradas
        var (_, detalle) = await ObtenerAsync(cliente, "/api/technicians/1");
        var datos = detalle.GetProperty("data");
        Assert.Equal(3, datos.GetProperty("specialtyCount").GetInt32());
        Assert.Equal(3, datos.GetProperty("specialties").GetArrayLength());
    }
}
