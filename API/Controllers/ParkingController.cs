using API.Application.Managers;
using API.Application.Models;
using API.Application.Models.Vehicles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ParkingController : BaseApiController
    {
        private readonly IParkingManager parkingManager;
        public ParkingController(IParkingManager parkingManager) => (this.parkingManager) = parkingManager;

        /// <summary>
        /// Gets the available parking spaces.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-available-spaces")]
        public async Task<ActionResult<AvailableSpacesResponse>> GetAvailableParkingSpaces() 
            => await this.parkingManager.GetAvailableParkingSpaces();

        /// <summary>
        /// Gets all the vehicles with discounts.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all-client-vehicles")]
        public async Task<ActionResult<List<GetAllVehiclesResponse>>> GetAllVehicles() 
            => await this.parkingManager.GetAllRegularClientsVehilces();

        /// <summary>
        /// Gets all parked vehicles.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all-parked-vehicles")]
        public async Task<ActionResult<List<GetAllParkedVehiclesResponse>>> GetAllParkedVehicles()
            => await this.parkingManager.GetAllParkedVehicles();

        /// <summary>
        /// Returns the accumulated charge for a vehicle by registration number.
        /// </summary>
        /// <param name="regNumber"></param>
        /// <returns></returns>
        [HttpGet("get-accumulated-charge-byRegNumber")]
        public async Task<ActionResult<GetAccumulatedChargeResponse>> GetAccumulatedChargeByRegistrationNumber(string regNumber)
            => await this.parkingManager.GetAccumulatedChargeByRegistrationNumber(regNumber);

        /// <summary>
        /// Registers that a vehicle has entered the parking.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add-vehicle")]
        public async Task<ActionResult<VehicleModel>> AddVehicleToParking(AdmitVehicleRequest request)
        {
            var vehicle = await this.parkingManager.AddVehicle(request);
            return StatusCode(201, vehicle);
        }

        /// <summary>
        /// Registers that a vehicle has left the parking.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("remove-vehicle")]
        public async Task<ActionResult<RemoveVehicleResponse>> RemoveVehicleFromParking(RemoveVehicleRequest request)
            => await this.parkingManager.RemoveVehicle(request);                    
    }
}
