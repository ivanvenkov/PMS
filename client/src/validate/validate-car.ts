import { vehicleInput } from '../common/constants';
import discountTypes from '../common/discount-types.enum';
import vehicleTypes from '../common/vehicle-types.enum';

const validate = {
  regNumber: (value: string) =>
    vehicleInput.LICENSE_PLATE_REGEX.test(value) &&
    value.length >= vehicleInput.MIN_LICENSE_PLATE_LENGTH &&
    value.length <= vehicleInput.MAX_LICENSE_PLATE_LENGTH,
  discount: (value: string) => Object.keys(discountTypes).includes(value),
  vehicleType: (value: string) => Object.keys(vehicleTypes).includes(value)
};

const validateCar = {
  regNumber: (value: string) => {
    if (!value) {
      return ' is required!';
    }
    if (!validate.regNumber(value)) {
      return ' must be valid';
    }
    return '';
  },
  vehicleType: (value: string) => {
    if (!value) {
      return ' is required!';
    }
    if (!validate.vehicleType(value)) {
      return ' must be one of the listed';
    }
    return '';
  },
  discount: (value: string) => {
    if (!value) {
      return ' is required!';
    }
    if (!validate.discount(value)) {
      return ' must be one of the listed';
    }
    return '';
  }
};

export default validateCar;
