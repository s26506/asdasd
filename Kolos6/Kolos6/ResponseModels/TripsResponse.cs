namespace Kolos6.ResponseModels;

public class TripsResponse
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public List<TripResponse> Trips { get; set; }
}