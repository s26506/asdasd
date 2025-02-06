using System.Data;
using Kolos6.Context;
using Kolos6.Models;
using Kolos6.RequestModels;
using Kolos6.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Kolos6;

public class TripService : ITripService
{
    private readonly MasterContext _context;

    public TripService(MasterContext context)
    {
        _context = context;
    }

    public async Task<TripsResponse> GetTrips(int page, int pageSize)
    {
        var query = _context.Trips
            .OrderByDescending(t => t.DateFrom)
            .Include(t => t.CountryTrips)
            .ThenInclude(ct => ct.IdCountryNavigation)
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.IdClientNavigation);

        var totalTrips = await query.CountAsync();
        var trips = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new TripsResponse
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = (int)Math.Ceiling((double)totalTrips / pageSize),
            Trips = trips.Select(t => new TripResponse
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.CountryTrips.Select(ct => new CountryResponse
                {
                    Name = ct.IdCountryNavigation.Name
                }).ToList(),
                Clients = t.ClientTrips.Select(ct => new ClientResponse
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            }).ToList()
        };
    }

    public async Task<ClientResponse> AssignClientToTrip(int idTrip, TripClientRequest request)
    {
        // Sprawdź, czy klient istnieje
        var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == request.Pesel);

        // Znajdź wycieczkę
        var trip = await _context.Trips.FindAsync(idTrip);
        if (trip == null || trip.DateFrom <= DateTime.Now)
        {
            throw new DataException("Trip does not exist or has already started.");
        }

        // Sprawdź, czy klient już uczestniczy w tej wycieczce (bez operatora ?.)
        if (existingClient != null)
        {
            var isClientOnTrip = await _context.ClientTrips
                .AnyAsync(ct => ct.IdClient == existingClient.IdClient && ct.IdTrip == idTrip);

            if (isClientOnTrip)
            {
                throw new DataException("Client is already registered for this trip.");
            }
        }

        // Utwórz nowego klienta
        var newClient = new Client
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Telephone = request.Telephone,
            Pesel = request.Pesel
        };

        _context.Clients.Add(newClient);
        await _context.SaveChangesAsync();

        // Przypisz klienta do wycieczki
        _context.ClientTrips.Add(new ClientTrip
        {
            IdClient = newClient.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = request.PaymentDate
        });

        await _context.SaveChangesAsync();

        // Zwróć dane klienta
        return new ClientResponse
        {
            IdClient = newClient.IdClient,
            FirstName = newClient.FirstName,
            LastName = newClient.LastName
        };
    }


}