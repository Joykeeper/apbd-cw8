using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Tutorial8.Models.DTOs;

public class ClientDTO
{
    public int? Id { get; set; }
    [MinLength(3), StringLength(100)]
    public required string FirstName { get; set; }
    [MinLength(3), StringLength(100)]
    public required string LastName { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    [MinLength(7), StringLength(100)]
    public required string Telephone { get; set; }
    [MinLength(11), StringLength(11)]
    public required string Pesel { get; set; }
    public List<ClientTripDTO>? Trips { get; set; }
}

public class ClientTripDTO
{
    public required TripDTO Trip { get; set; }
    
    public DateTime RegisteredAt { get; set; }
    public DateTime PaymentDate { get; set; }
}