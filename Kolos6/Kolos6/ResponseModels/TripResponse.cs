namespace Kolos6.ResponseModels;

public class TripResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<CountryResponse> Countries { get; set; }
    public List<ClientResponse> Clients { get; set; }
}