using System.Data;
using Kolos6.Context;
using Microsoft.EntityFrameworkCore;

namespace Kolos6;

public class ClientService : IClientService
{
    private readonly MasterContext _context;

    public ClientService(MasterContext context)
    {
        _context = context;
    }

    public async Task DeleteClient(int idClient)
    {
        var client = await _context.Clients.Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
        {
            throw new DataException("Client not found.");
        }

        if (client.ClientTrips.Any())
        {
            throw new DataException("Client has active trips and cannot be deleted.");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }
}