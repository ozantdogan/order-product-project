namespace OTD.Core.Models.Responses
{
    public class VehicleDto
    {
        public Guid VehicleId { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public string? Color { get; set; }
        public int Horsepower { get; set; }
    }
}
