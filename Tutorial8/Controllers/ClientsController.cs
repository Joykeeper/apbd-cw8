using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly IClientsService _clientsService;

    public ClientsController(IClientsService clientsService)
    {
        _clientsService = clientsService;
    }
    
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTrips([FromRoute] int id)
    {   
        var clientExists = await _clientsService.ClientExists(id);

        if (!clientExists) return NotFound("Client not found");
        
        
        var trips = await _clientsService.GetClientTripsWithDetails(id);
        
        
        if (trips.Count == 0) return NotFound("Client has not been registered to any of the trips");
        
        return Ok(trips);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] ClientDTO client)
    {
        var newClientId = await _clientsService.CreateClient(client);
        
        if (newClientId == -1) return StatusCode(500, "Failed to create client");
        
        return StatusCode(201, newClientId);
    }
    
    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClientOnTrip([FromRoute] int id,[FromRoute] int tripId)
    {
        var clientExists = await _clientsService.ClientExists(id);

        if (!clientExists) return NotFound("Client not found");
        
        var trip = await _clientsService.TripExists(tripId);

        if (!trip) return NotFound("Trip not found");
        
        var message = await _clientsService.RegisterClientOnTrip(id, tripId);
        
        if (message == "Success") return Ok("Successfully registered the client on a trip");
        if (message == "Overload") return Conflict("Cannot register on a trip, max number of people reached");
        if (message == "Server Error") return StatusCode(500, "Cannot register on a trip");
        
        return BadRequest(message);
    }

    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> DeleteClientTripRegistration([FromRoute] int id, [FromRoute] int tripId)
    {
        var registrationExists = await _clientsService.RegistrationExists(id, tripId);
        
        if (!registrationExists) return NotFound("Client has not been registered to the provided trip");
        
        var message = await _clientsService.DeleteClientTripRegistration(id, tripId);
        
        if (message == "Success") return Ok("Successfully unregistered the client from the trip");
        if (message == "Server Error") return StatusCode(500, "Failed to unregister the client");
        
        return BadRequest(message);
    }
}