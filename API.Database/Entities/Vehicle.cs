using API.Database.Enums;

namespace API.Database.Entities
{
    public class Vehicle 
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }

        public VehicleType VehicleType { get; set; }

        public string VehicleTypeDescription { get; set; }

        public DateTime? TimeOfEntry { get; set; }

        public DateTime? TimeOfLeaving { get; set; }

        public DiscountTypes? Discounts { get; set; }

        public  int ParkingSpacesOccupied { get; set; }       
    }
}
