using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Proyecto.Domain.DTO.ResultadoPatron;
using Xunit;

namespace Proyecto.WebAPI.Tests;

public sealed class PruebasContratoActivo
{
    [Fact]
    public void ResultadoConservaLosNombresJsonPublicos()
    {
        var resultado = Resultado<int>.Paginado(7, 2, 10, 17);
        using var documento = JsonDocument.Parse(JsonSerializer.Serialize(resultado));
        var json = documento.RootElement;

        Assert.Equal(200, json.GetProperty("statusCode").GetInt32());
        Assert.Equal("Correcto", json.GetProperty("title").GetString());
        Assert.True(json.GetProperty("isSuccess").GetBoolean());
        Assert.Equal(7, json.GetProperty("data").GetInt32());
        Assert.Equal(2, json.GetProperty("pageNumber").GetInt32());
        Assert.Equal(10, json.GetProperty("pageSize").GetInt32());
        Assert.Equal(17, json.GetProperty("totalRecords").GetInt32());
        Assert.Equal(2, json.GetProperty("totalPages").GetInt32());
    }

    [Fact]
    public async Task EndpointActivo_PreservaRutasYContratoJson()
    {
        using var fabrica = new FabricaApi();
        await fabrica.SembrarAsync();
        using var cliente = fabrica.CreateClient();

        var solicitud = new
        {
            name = "Activo de contrato",
            description = "Prueba de compatibilidad",
            brand = "Marca",
            model = "Modelo",
            serialNumber = $"CONTRACT-{Guid.NewGuid():N}",
            acquiredOn = new DateTime(2025, 1, 1),
            purchaseCost = 1000m,
            usefulLifeMonths = 12,
            usageHours = 0,
            criticality = "Low",
            categoryId = 1,
            locationId = 1
        };

        using var respuesta = await cliente.PostAsJsonAsync("/api/assets", solicitud);

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        using var cuerpo = await JsonDocument.ParseAsync(await respuesta.Content.ReadAsStreamAsync());
        var datos = cuerpo.RootElement.GetProperty("data");
        Assert.Equal(solicitud.name, datos.GetProperty("name").GetString());
        Assert.True(datos.TryGetProperty("assetCode", out _));
        Assert.True(datos.TryGetProperty("categoryId", out _));
        Assert.True(datos.TryGetProperty("imageUrl", out _));
        Assert.False(datos.TryGetProperty("codigoActivo", out _));

        using var respuestaRutaTraducida = await cliente.GetAsync("/api/Activo");
        Assert.Equal(HttpStatusCode.NotFound, respuestaRutaTraducida.StatusCode);
    }
}
