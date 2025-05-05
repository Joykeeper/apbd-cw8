using Microsoft.Build.Framework;

namespace Tutorial8.Models.DTOs;

public class TripDTO
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<CountryDTO>? Countries { get; set; }
}

public class CountryDTO
{
    public string Name { get; set; }
}