using System.Data;
using Kolos6.RequestModels;

namespace Kolos6;

public static class Configuration
{
    public static void RegisterEndpoints(this IEndpointRouteBuilder app)
    {
        // Pobieranie listy wycieczek
        app.MapGet("api/trips", async (ITripService service, int? page, int? pageSize) =>
        {
            try
            {
                var result = await service.GetTrips(page ?? 1, pageSize ?? 10);
                return Results.Ok(result);
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        });

        // Usuwanie klienta
        app.MapDelete("api/clients/{idClient:int}", async (IClientService service, int idClient) =>
        {
            try
            {
                await service.DeleteClient(idClient);
                return Results.NoContent();
            }
            catch (DataException e)
            {
                return Results.Conflict(e.Message);
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        });

        // Przypisanie klienta do wycieczki
        app.MapPost("api/trips/{idTrip:int}/clients", async (ITripService service, int idTrip, TripClientRequest request) =>
        {
            try
            {
                var response = await service.AssignClientToTrip(idTrip, request);
                return Results.Created($"/api/trips/{idTrip}/clients/{response.IdClient}", response);
            }
            catch (DataException e)
            {
                return Results.Conflict(e.Message);
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        });
    }
}
