using API.Application.Models;
using API.Application.Models.Discounts;
using API.Application.Models.Vehicles;
using API.Database;
using API.Database.Entities;
using API.Database.Enums;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace API.Application.Managers
{
    public class ParkingManager : IParkingManager
    {
        private const int parkingCapacity = 200;
        private readonly APIdbContext context;

        public ParkingManager(APIdbContext context) => this.context = context;

        public async Task<AvailableSpacesResponse> GetAvailableParkingSpaces()
        {
            var spacesOccupied = await this.context.Vehicles
                .Where(x=>x.TimeOfLeaving == null)
                .Select(x => x.ParkingSpacesOccupied)
                .SumAsync();
            var availableSpaces = parkingCapacity - spacesOccupied > 0 ? parkingCapacity - spacesOccupied : 0;

            return new AvailableSpacesResponse { AvailableSpaces = availableSpaces };
        }

        public async Task<List<GetAllParkedVehiclesResponse>> GetAllParkedVehicles()
        {
            var response = new List<GetAllParkedVehiclesResponse>();
            var vehicles =  await this.context.Vehicles
                .Where(x => x.TimeOfEntry != null && x.TimeOfLeaving == null)
                .ToListAsync();

            foreach (var vehicle in vehicles)
            {
                var accumulatedCharge = this.GetAccumulatedChargeByRegistrationNumber(vehicle.RegistrationNumber).Result.AccumulatedCharge;
                var discount = vehicle.Discounts != null ? this.GetDiscount(vehicle.Discounts.Value, accumulatedCharge) : 0m;
                var totalCharge = accumulatedCharge - discount;

                var vehicleResponse = new GetAllParkedVehiclesResponse
                {
                    RegistrationNumber = vehicle.RegistrationNumber,
                    TimeOfEntry = vehicle.TimeOfEntry,
                    AccumulatedCharge = accumulatedCharge,
                    Discount = discount,
                    TotalCharge = totalCharge
                };

                response.Add(vehicleResponse);
            }

            return response;
        }

        public async Task<List<GetAllVehiclesResponse>> GetAllRegularClientsVehilces()
        {
            var response = await this.context.Vehicles.Where(x => x.Discounts != null).ToListAsync();
            return response.Select(x => new GetAllVehiclesResponse
            {
                Discount = x.Discounts,
                RegistrationNumber = x.RegistrationNumber,
                VehicleType = x.VehicleType

            }).ToList();
        }

        public async Task<VehicleModel> AddVehicle(AdmitVehicleRequest request)
        {
            var currentVehicle = await this.context.Vehicles.FirstOrDefaultAsync(x => x.RegistrationNumber == request.RegNumber);

            var vehicleModel = GetVehicleType(Enum.GetName(typeof(VehicleType), request.VehicleType));

            if (currentVehicle != null && currentVehicle.Discounts == null)
            {
                throw new InvalidOperationException($"A vehicle with registration number '{request.RegNumber}' is already in.");
            }
            else if ((currentVehicle != null && currentVehicle.Discounts != null) && currentVehicle.TimeOfLeaving == null)
            {
                throw new InvalidOperationException($"A vehicle with registration number '{request.RegNumber}' is already in.");
            }
            else if ((currentVehicle != null && currentVehicle.Discounts != null) && currentVehicle.TimeOfLeaving != null)
            {
                currentVehicle.TimeOfEntry = request.TimeOfEntry;
                currentVehicle.TimeOfLeaving = null;
                request.Discount = request.Discount;
            }
            else
            {
                var newVehicle = new Vehicle
                {
                    RegistrationNumber = request.RegNumber.ToUpper(),
                    VehicleType = request.VehicleType,
                    TimeOfEntry = request.TimeOfEntry,
                    VehicleTypeDescription = Enum.GetName(typeof(VehicleType), request.VehicleType),
                    ParkingSpacesOccupied = vehicleModel.ParkingSpacesOccupied,
                    Discounts = request.Discount != null ? request.Discount : null
                };

                await this.context.Vehicles.AddAsync(newVehicle);
            }

            await this.context.SaveChangesAsync();

            return new VehicleModel
            {
                RegistrationNumber = request.RegNumber.ToUpper(),
                VehicleType = request.VehicleType,
                TimeOfEntry = request.TimeOfEntry,
                VehicleTypeDescription = Enum.GetName(typeof(VehicleType), request.VehicleType),
                ParkingSpacesOccupied = vehicleModel.ParkingSpacesOccupied,
                DaytimeCharge = vehicleModel.DaytimeCharge,
                NighttimeCharge = vehicleModel.NighttimeCharge,
                Discount = request.Discount
            };
        }

        public async Task<RemoveVehicleResponse> RemoveVehicle(RemoveVehicleRequest request)
        {
            var totalAccumulatedCharge = await this.GetAccumulatedChargeByRegistrationNumber(request.RegNumber);
            if (totalAccumulatedCharge == null)
                return null;

            int result;
            var vehicle = await this.context.Vehicles.FirstOrDefaultAsync(x => x.RegistrationNumber == request.RegNumber.ToUpper());
 
            if (vehicle.Discounts == null)
            {
                this.context.Vehicles.Remove(vehicle);
                result = await this.context.SaveChangesAsync();
            }
            else
            {
                if (vehicle.TimeOfLeaving != null)
                {
                    throw new InvalidOperationException($"Vehicle with registration number '{vehicle.RegistrationNumber}' is outside the parking.");
                }
                vehicle.TimeOfLeaving = DateTime.Now;
                result = await this.context.SaveChangesAsync();
            }

            if (result > 0)
            {
                return new RemoveVehicleResponse
                {
                    RegistrationNumber = request.RegNumber,
                    AccumulatedCharge = totalAccumulatedCharge.AccumulatedCharge,
                    TimeOfEntry = vehicle.TimeOfEntry
                };
            }
            else
            {
                throw new Exception("Problems removing vehicle from database");
            }
        }

        public async Task<GetAccumulatedChargeResponse> GetAccumulatedChargeByRegistrationNumber(string regNumber)
        {
            var vehicle = await this.context.Vehicles.FirstOrDefaultAsync(x => x.RegistrationNumber == regNumber.ToUpper());
            if (vehicle == null)
                throw new InvalidOperationException($"A vehicle with registration number '{regNumber}' has not been registered with the parking system.");

            VehicleModel currentVehicle;
            GetAccumulatedChargeResponse accumulatedChargeResponse = null;

            currentVehicle = GetVehicleType(vehicle.VehicleTypeDescription);
            if (currentVehicle == null)  // CHECK LOGIC HERE
            {
                return null;
            }
            currentVehicle.TimeOfEntry = vehicle.TimeOfEntry;
            currentVehicle.Discount = vehicle.Discounts;
            accumulatedChargeResponse = CalculateCharge(currentVehicle);
            accumulatedChargeResponse.TimeOfEntry = vehicle.TimeOfEntry;
            accumulatedChargeResponse.RegistrationNumber = vehicle.RegistrationNumber;

            return accumulatedChargeResponse;
        }

        private GetAccumulatedChargeResponse CalculateCharge(VehicleModel vehicle)
        {
            var timeOfEntry = (DateTime)vehicle.TimeOfEntry;
            var timeOfLeaving = DateTime.Now;
            var timeElapsed = timeOfLeaving - timeOfEntry;

            decimal accumulatedCharge = 0m;
            decimal discount;
            decimal totalCharge;


            bool isNightChargeApplicable = CheckIfNightChargeApplicable((DateTime)vehicle.TimeOfEntry, timeOfLeaving);
            if (isNightChargeApplicable)
            {
                var nightChargeHours = CheckNightChargePart((DateTime)vehicle.TimeOfEntry, timeOfLeaving);
                if (nightChargeHours > 0)
                {
                    //var accumulatedCharge = this.GetAccumulatedChargeByRegistrationNumber(vehicle.RegistrationNumber).Result.AccumulatedCharge;
                    //var discount = vehicle.Discounts != null ? this.GetDiscount(vehicle, accumulatedCharge) : 0m;
                    //var totalCharge = accumulatedCharge - discount;
                    if (timeElapsed.TotalHours > 24)
                    {
                        var fullDays = Math.Floor(timeElapsed.TotalHours / 24);
                        accumulatedCharge = (int)fullDays * (14 * vehicle.NighttimeCharge + 10 * vehicle.DaytimeCharge);
                        timeOfEntry = timeOfEntry.AddDays(fullDays);
                        nightChargeHours = this.CheckNightChargePart(timeOfEntry, timeOfLeaving);
                        timeElapsed = timeOfLeaving - timeOfEntry;
                    }

                    accumulatedCharge += nightChargeHours * vehicle.NighttimeCharge;
                    accumulatedCharge += CalculateHours(timeElapsed.TotalHours - nightChargeHours) * vehicle.DaytimeCharge;
                    discount = vehicle.Discount != null ? this.GetDiscount(vehicle.Discount.Value, accumulatedCharge) : 0m;
                    totalCharge = accumulatedCharge - discount;

                }
                else
                {
                    accumulatedCharge = CalculateHours(timeElapsed.TotalHours) * vehicle.NighttimeCharge;
                    discount = vehicle.Discount != null ? this.GetDiscount(vehicle.Discount.Value, accumulatedCharge) : 0m;
                    totalCharge = accumulatedCharge - discount;
                }
                return new GetAccumulatedChargeResponse
                {
                    AccumulatedCharge = accumulatedCharge,
                    Discount = discount,
                    TotalCharge = totalCharge,
                    DiscountType = Enum.GetName(typeof(DiscountTypes), vehicle.Discount.HasValue)
                };
            }
            else
            {
                accumulatedCharge = CalculateHours(timeElapsed.TotalHours) * vehicle.DaytimeCharge;
                discount = vehicle.Discount != null ? this.GetDiscount(vehicle.Discount.Value, accumulatedCharge) : 0m;
                totalCharge = accumulatedCharge - discount;

                return new GetAccumulatedChargeResponse
                {
                    AccumulatedCharge = accumulatedCharge,
                    Discount = discount,
                    TotalCharge = totalCharge,
                    DiscountType =  Enum.GetName(typeof(DiscountTypes), vehicle.Discount.HasValue)
                };
            }
        }

        private decimal GetChargeAfterDiscount(VehicleModel vehicle, decimal accumulatedCharge)
            => vehicle.Discount != null ? this.GetDiscount(vehicle.Discount.Value, accumulatedCharge) : 0m;


        private bool CheckIfNightChargeApplicable(DateTime timeOfEntry, DateTime timeOfLeaving)
        {
            // var timeElapsed = timeOfLeaving - timeOfEntry;

            var startNightCharge = TimeSpan.Parse("18:00");
            var endNightCharge = TimeSpan.Parse("08:00");

            if ((timeOfEntry.TimeOfDay >= startNightCharge || timeOfEntry.TimeOfDay < endNightCharge) || (timeOfLeaving.TimeOfDay >= startNightCharge || timeOfLeaving.TimeOfDay < endNightCharge) || (timeOfLeaving.Day != timeOfEntry.Day))
                return true;

            return false;
        }

        private int CheckNightChargePart(DateTime timeOfEntry, DateTime timeOfLeaving)
        {
            var startNightCharge = TimeSpan.Parse("18:00");
            var endNightCharge = TimeSpan.Parse("08:00");

            int nightChargeHours = 0;

            if (timeOfEntry.Day != timeOfLeaving.Day)
            {
                if (endNightCharge >= timeOfLeaving.TimeOfDay && (timeOfEntry.TimeOfDay >= endNightCharge && timeOfEntry.TimeOfDay < startNightCharge))
                {
                    var hours = timeOfLeaving.TimeOfDay.TotalHours + 6;
                    nightChargeHours += CalculateHours(hours);
                }
                else if (endNightCharge >= timeOfLeaving.TimeOfDay && (timeOfEntry.TimeOfDay >= endNightCharge && timeOfEntry.TimeOfDay >= startNightCharge))
                {
                    var midNight = TimeSpan.Parse("24:00");
                    var hours = ((midNight - timeOfEntry.TimeOfDay) + (timeOfLeaving.TimeOfDay - midNight)).TotalHours;
                    nightChargeHours += CalculateHours(hours);
                }
                else if (timeOfEntry.TimeOfDay < startNightCharge && timeOfLeaving.TimeOfDay > endNightCharge)
                {
                    nightChargeHours += 14;
                }
            }

            if (startNightCharge <= timeOfLeaving.TimeOfDay && timeOfEntry.TimeOfDay < startNightCharge)
            {
                var hours = (timeOfLeaving.TimeOfDay - startNightCharge).TotalHours;
                nightChargeHours += CalculateHours(hours);
            }
            else if (startNightCharge <= timeOfLeaving.TimeOfDay && timeOfEntry.TimeOfDay > startNightCharge)
            {
                var hours = (timeOfLeaving - timeOfEntry).TotalHours;
                nightChargeHours += CalculateHours(hours);
            }
            else if (endNightCharge >= timeOfLeaving.TimeOfDay && timeOfEntry.TimeOfDay < endNightCharge)
            {
                var hours = (timeOfLeaving - timeOfEntry).TotalHours;
                nightChargeHours += CalculateHours(hours);
            }
            else if (endNightCharge < timeOfLeaving.TimeOfDay && timeOfEntry.TimeOfDay < endNightCharge)
            {
                var hours = (endNightCharge - timeOfEntry.TimeOfDay).TotalHours;
                nightChargeHours += CalculateHours(hours);
            }

            return nightChargeHours;
        }

        // Nomenclature for discounts
        private decimal GetDiscount(DiscountTypes vehicle, decimal accumulatedCharge)
        {
            IDiscount currentDiscount;
            var discountType = Enum.GetName(typeof(DiscountTypes), vehicle);

            var discount = Assembly
                            .GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => typeof(IDiscount).IsAssignableFrom(t)
                            && t.IsClass
                            && t.Name.Contains(discountType)).FirstOrDefault();

            if (discount == null)
                throw new Exception("Not existing type of discount");

            currentDiscount = (IDiscount)Activator.CreateInstance(discount);
            return currentDiscount.Calculate(accumulatedCharge);
        }
        private VehicleModel GetVehicleType(string vehicleName)
        {
            VehicleModel currentVehicle;
            var vehicleType = Assembly
                            .GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => typeof(VehicleModel).IsAssignableFrom(t)
                            && t.IsClass
                            && t.Name.Contains(vehicleName)).FirstOrDefault();

            if (vehicleType == null)
                throw new Exception("Not existing type of vehicle");

            currentVehicle = (VehicleModel)Activator.CreateInstance(vehicleType);
            return currentVehicle;
        }

        private int CalculateHours(double hours) => (int)Math.Ceiling(hours);
    }
}
