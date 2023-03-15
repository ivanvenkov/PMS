namespace API.Application.Models.Vehicles
{
    internal class BusAndTrucks : VehicleModel
    {
        public override decimal DaytimeCharge { get; set; } = 12m;

        public override decimal NighttimeCharge { get; set; } = 8m;

        public override int ParkingSpacesOccupied { get; set; } = 3;
    }
}
