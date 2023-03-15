using API.Application.Models;
using API.Application.Models.Vehicles;

namespace API.Application.Managers
{
    public interface IParkingManager
    {
        Task<AvailableSpacesResponse> GetAvailableParkingSpaces();

        Task<VehicleModel> AddVehicle(AdmitVehicleRequest request);

        Task<GetAccumulatedChargeResponse> GetAccumulatedChargeByRegistrationNumber(string regNumber);

        Task<RemoveVehicleResponse> RemoveVehicle(RemoveVehicleRequest request);

        Task<List<GetAllParkedVehiclesResponse>> GetAllParkedVehicles();
        Task<List<GetAllVehiclesResponse>> GetAllRegularClientsVehilces();
    }
}