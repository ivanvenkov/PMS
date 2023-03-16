using API.Database.Enums;

namespace API.Application.Models.Vehicles
{

    public class VehicleModel 
    {  
        public string RegistrationNumber { get; set; }
        public VehicleType? VehicleType { get; set; }
        public DiscountTypes? Discount { get; set; }
        public string VehicleTypeDescription { get; set; }
        public DateTime? TimeOfEntry { get; set; }
        public virtual decimal DaytimeCharge { get; set; }
        public virtual decimal NighttimeCharge { get; set; }     
        public virtual int ParkingSpacesOccupied { get; set; } 
    }
}