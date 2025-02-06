using Kolos6.RequestModels;
using Kolos6.ResponseModels;

namespace Kolos6;

public interface ITripService
{
    Task<TripsResponse> GetTrips(int page, int pageSize);
    Task<ClientResponse> AssignClientToTrip(int idTrip, TripClientRequest request);
}