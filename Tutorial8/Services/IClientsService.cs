using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<ClientTripDTO>> GetClientTripsWithDetails(int clientId);
    
    Task<Int32> CreateClient(ClientDTO client);
    
    Task<String> RegisterClientOnTrip(int clientId, int tripId);
    
    Task<String> DeleteClientTripRegistration(int clientId, int tripId);
    
    Task<Boolean> ClientExists(int clientId);
    Task<Boolean> TripExists(int tripId);
    Task<Boolean> RegistrationExists(int clientId, int tripId);
}