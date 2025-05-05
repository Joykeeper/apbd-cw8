using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";
    
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command = "SELECT IdTrip, Name, Description, DateFrom, DateTo, MaxPeople FROM Trip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    trips.Add(new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        DateFrom = reader.GetDateTime(3),
                        DateTo = reader.GetDateTime(4),
                        MaxPeople = reader.GetInt32(5),
                    });
                }
            }
        }
        

        return trips;
    }

    public async Task<List<CountryDTO>> GetTripCountries(int tripId)
    {
        var countries = new List<CountryDTO>();

        string command = """
                         SELECT Name FROM Country
                         INNER JOIN Country_Trip AS CT ON Country.IdCOuntry = CT.IdCountry
                         WHERE CT.IdTrip = @TripId;
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("TripId", tripId);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    countries.Add(new CountryDTO()
                    {
                        Name = reader.GetString(0),
                    });
                }
            }
        }

        return countries;
    }

    // public async Task<List<ClientDTO>> GetClients()
    // {
    //     var clients = new List<ClientDTO>();
    //
    //     string command = "SELECT IdClient, FistName, LastName, Email, Telephone, Pesel FROM Client";
    //     
    //     using (SqlConnection conn = new SqlConnection(_connectionString))
    //     using (SqlCommand cmd = new SqlCommand(command, conn))
    //     {
    //         await conn.OpenAsync();
    //
    //         using (var reader = await cmd.ExecuteReaderAsync())
    //         {
    //             while (await reader.ReadAsync())
    //             {
    //                 int idOrdinal = reader.GetOrdinal("IdClient");
    //                 clients.Add(new ClientDTO()
    //                 {
    //                     Id = reader.GetInt32(idOrdinal),
    //                     FirstName = reader.GetString(1),
    //                     LastName = reader.GetString(2),
    //                     Email = reader.GetString(3),
    //                     Telephone = reader.GetString(4),
    //                     Pesel = reader.GetString(5),
    //                 });
    //             }
    //         }
    //     }
    //     
    //
    //     return clients;
    // }
}