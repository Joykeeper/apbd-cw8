using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";

    public async Task<List<ClientTripDTO>> GetClientTripsWithDetails(int clientId)
    {
        var clientTrips = new List<ClientTripDTO>();

        string command = """
                         SELECT Trip.Name, Trip.Description, Trip.DateFrom, Trip.DateTo, Trip.MaxPeople, 
                                CT.RegisteredAt, CT.PaymentDate
                         FROM Trip
                         INNER JOIN Client_Trip AS CT ON CT.IdClient = @ClientId;
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("ClientId", clientId);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    clientTrips.Add(new ClientTripDTO()
                    {
                        Trip = new TripDTO(){ 
                            Name = reader.GetString(0), 
                            Description = reader.GetString(1),
                            DateFrom = reader.GetDateTime(2),
                            DateTo = reader.GetDateTime(3),
                            MaxPeople = reader.GetInt32(4),
                        },
                        RegisteredAt = reader.GetDateTime(5),
                        PaymentDate = reader.GetDateTime(6),
                    });
                }
            }
        }

        return clientTrips;
    }

    public async Task<int> CreateClient(ClientDTO client)
    {
        int newClientId = await GetBiggestClientId() + 1;
        
        string command = """
                         INSERT INTO Client (IdClient, FirstName, LastName, Email, Telephone, Pesel)
                         VALUES (@IdClient, @FirstName, @LastName, @Email, @Telephone, @Pesel);
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("IdClient", newClientId);
            cmd.Parameters.AddWithValue("FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("LastName", client.LastName);
            cmd.Parameters.AddWithValue("Email", client.Email);
            cmd.Parameters.AddWithValue("Telephone", client.Telephone);
            cmd.Parameters.AddWithValue("Pesel", client.Pesel);


            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                newClientId = -1;
            }
        }

        return newClientId;
    }

    public async Task<int> GetBiggestClientId()
    {
        int biggestClientId = -1;

        string command = """
                         SELECT max(idClient) FROM table
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            
            object result = await cmd.ExecuteScalarAsync();

            if (result != DBNull.Value)
            {
                biggestClientId = Convert.ToInt32(result);
            }
        }

        return biggestClientId;
    }

    public async Task<string> RegisterClientOnTrip(int clientId, int tripId)
    {
        var limitReached = await IsTripPeopleLimitReached(tripId);
        
        if (limitReached) return "Overload";
        
        string command = """
                         INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
                         VALUES (@IdClient, @IdTrip, @RegisteredAt);
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("IdClient", clientId);
            cmd.Parameters.AddWithValue("IdTrip", tripId);
            cmd.Parameters.AddWithValue("RegisteredAt", DateTime.Now);

            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Server Error";
            }
        }

        return "Success";
    }
    
    public async Task<bool> IsTripPeopleLimitReached(int tripId)
    {
        string command = """
                         SELECT COUNT(1) FROM Client_Trip WHERE IdTrip = @TripId
                         """;
        
        int currentNumberOfPeople = 0;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("TripId", tripId);


            object result = await cmd.ExecuteScalarAsync();

            if (result != DBNull.Value)
            {
                currentNumberOfPeople = Convert.ToInt32(result);
            }
        }

        
        command = " SELECT MaxPeople FROM Trip WHERE IdTrip = @TripId ";
        
        int maxPeopleForCurrentTrip = 0;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("TripId", tripId);


            object result = await cmd.ExecuteScalarAsync();

            if (result != DBNull.Value)
            {
                currentNumberOfPeople = Convert.ToInt32(result);
            }
        }
            
        var limitReached = currentNumberOfPeople >= maxPeopleForCurrentTrip;
            
        return limitReached;
    }

    public async Task<string> DeleteClientTripRegistration(int clientId, int tripId)
    {
        string result = "Success";
        
        string command = """
                         DELETE FROM Client_Trip WHERE IdClient = @ClientId And IdTrip = @TripId
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("ClientId", clientId);
            cmd.Parameters.AddWithValue("TripId", tripId);


            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = "Server Error";
            }
        }

        return result;
    }

    public async Task<bool> ClientExists(int clientId)
    {
        bool exists = false;
        
        string command = """
                         SELECT 1 FROM Client WHERE IdClient = @ClientId
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("IdClient", clientId);


            object result = await cmd.ExecuteScalarAsync();

            if (result != DBNull.Value)
            {
                exists = true;
            }
        }

        return exists;
    }

    public async Task<bool> TripExists(int tripId)
    {
        bool exists = false;
        
        string command = """
                         SELECT 1 FROM Trip WHERE IdTrip = @TripId
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("TripId", tripId);


            object result = await cmd.ExecuteScalarAsync();

            if (result != DBNull.Value)
            {
                exists = true;
            }
        }

        return exists;
    }

    public async Task<bool> RegistrationExists(int clientId, int tripId)
    {
        bool exists = false;
        
        string command = """
                         SELECT 1 FROM Client_Trip WHERE IdTrip = @TripId AND IdClient = @ClientId
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("TripId", tripId);
            cmd.Parameters.AddWithValue("ClientId", clientId);


            object result = await cmd.ExecuteScalarAsync();

            if (result != DBNull.Value)
            {
                exists = true;
            }
        }

        return exists;
    }
}