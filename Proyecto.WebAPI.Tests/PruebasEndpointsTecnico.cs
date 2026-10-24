using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Proyecto.WebAPI.Tests;

public sealed class PruebasEndpointsTecnico
{
    [Fact]
    public async Task EndpointsCatalogosYTecnicos_DevuelvenDatosIniciales()
    {
        using var factory = new FabricaApi();
        await factory.SembrarAsync();
        using var client = factory.CreateClient();

        foreach (var path in new[]
                 {
                     "/api/locations",
                     "/api/maintenance-types",
                     "/api/specialties",
                     "/api/technicians"
                 })
        {
            using var respuesta = await client.GetAsync(path);

            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
            using var body = await JsonDocument.ParseAsync(await respuesta.Content.ReadAsStreamAsync());
            Assert.True(body.RootElement.GetProperty("data").GetArrayLength() > 0, path);
        }

        using var technicianResponse = await client.GetAsync($"/api/technicians/{FabricaApi.TecnicoId}");
        Assert.Equal(HttpStatusCode.OK, technicianResponse.StatusCode);
    }

    [Fact]
    public async Task EndpointsEspecialidadTecnico_AsignarConsultarYQuitarEspecialidad()
    {
        using var factory = new FabricaApi();
        await factory.SembrarAsync();
        using var client = factory.CreateClient();
        var solicitud = new
        {
            certificationLevel = "Advanced",
            certifiedSince = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))
        };
        var assignmentPath =
            $"/api/technicians/{FabricaApi.TecnicoId}/specialties/{FabricaApi.EspecialidadId}";

        using var assignmentResponse = await client.PostAsJsonAsync(assignmentPath, solicitud);
        Assert.Equal(HttpStatusCode.Created, assignmentResponse.StatusCode);

        using var technicianResponse =
            await client.GetAsync($"/api/technicians/{FabricaApi.TecnicoId}");
        Assert.Equal(HttpStatusCode.OK, technicianResponse.StatusCode);
        using (var technicianBody =
               await JsonDocument.ParseAsync(await technicianResponse.Content.ReadAsStreamAsync()))
        {
            var specialties = technicianBody.RootElement
                .GetProperty("data")
                .GetProperty("specialties");
            Assert.Contains(
                specialties.EnumerateArray(),
                especialidad => especialidad.GetProperty("id").GetInt32() == FabricaApi.EspecialidadId);
        }

        using var removalResponse = await client.DeleteAsync(assignmentPath);
        // Con Resultado<T> el DELETE exitoso responde 200 (204 solo si la API no devuelve cuerpo)
        Assert.Equal(HttpStatusCode.OK, removalResponse.StatusCode);

        using var technicianAfterRemoval =
            await client.GetAsync($"/api/technicians/{FabricaApi.TecnicoId}");
        using var bodyAfterRemoval =
            await JsonDocument.ParseAsync(await technicianAfterRemoval.Content.ReadAsStreamAsync());
        Assert.Empty(bodyAfterRemoval.RootElement
            .GetProperty("data")
            .GetProperty("specialties")
            .EnumerateArray());
    }

    [Fact]
    public async Task EndpointsEspecialidadTecnico_RechazanAsignacionesInvalidasDuplicadasYAusentes()
    {
        using var factory = new FabricaApi();
        await factory.SembrarAsync();
        using var client = factory.CreateClient();
        var assignmentPath =
            $"/api/technicians/{FabricaApi.TecnicoId}/specialties/{FabricaApi.EspecialidadId}";

        using var invalidLevelResponse = await client.PostAsJsonAsync(assignmentPath, new
        {
            certificationLevel = "Expert",
            certifiedSince = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidLevelResponse.StatusCode);

        using var futureDateResponse = await client.PostAsJsonAsync(assignmentPath, new
        {
            certificationLevel = "Advanced",
            certifiedSince = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1))
        });
        Assert.Equal(HttpStatusCode.BadRequest, futureDateResponse.StatusCode);

        var validRequest = new
        {
            certificationLevel = "Basic",
            certifiedSince = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))
        };
        using var firstAssignment = await client.PostAsJsonAsync(assignmentPath, validRequest);
        Assert.Equal(HttpStatusCode.Created, firstAssignment.StatusCode);

        using var duplicateAssignment = await client.PostAsJsonAsync(assignmentPath, validRequest);
        Assert.Equal(HttpStatusCode.Conflict, duplicateAssignment.StatusCode);

        using var missingTechnician = await client.PostAsJsonAsync(
            $"/api/technicians/{FabricaApi.TecnicoId + 1}/specialties/{FabricaApi.EspecialidadId}",
            validRequest);
        Assert.Equal(HttpStatusCode.NotFound, missingTechnician.StatusCode);
    }
}
