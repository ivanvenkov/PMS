interface CarType {
  vehicleRegistrationNumber: string;
  timeOfEntry: string | Date;
  accumulatedCharge: number | null;
  discount: number | null;
  totalCharge: number | null;
}

export default CarType;
