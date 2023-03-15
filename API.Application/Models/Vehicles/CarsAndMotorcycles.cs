namespace API.Application.Models.Vehicles
{
    public class CarsAndMotorcycles : VehicleModel
    {
        public override decimal DaytimeCharge { get; set; } = 3m;

        public override decimal NighttimeCharge { get; set; } = 2m;

        public override int ParkingSpacesOccupied { get; set; } = 1;
    }
}
