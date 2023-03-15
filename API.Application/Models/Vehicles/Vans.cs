namespace API.Application.Models.Vehicles
{
    public class Vans : VehicleModel
    {
        public override decimal DaytimeCharge { get; set; } = 6m;

        public override decimal NighttimeCharge { get; set; } = 4m;

        public override int ParkingSpacesOccupied { get; set; } = 2;
    }
}
