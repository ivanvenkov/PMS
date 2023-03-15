import React, { Dispatch, SetStateAction, useEffect, useState } from "react";
import "./createForm.css";
import { Button, Form } from "react-bootstrap";
import validateCar from "../validate/validate-car";
import { BASE_URL } from "../common/constants";
import CarType from "../types/carType";
import vehicleTypes from "../common/vehicle-types.enum";
import discountTypes from "../common/discount-types.enum";

const emptyCar = {
  regNumber: "",
  vehicleType: "",
  discount: "",
};

const CreateCarForm: React.FC<{
  carsList: CarType[];
  setCarsList: Dispatch<SetStateAction<CarType[]>>;
  setChangeAvailableSpaces: Dispatch<SetStateAction<boolean>>;
}> = ({ carsList, setCarsList, setChangeAvailableSpaces }) => {
  const [error, setError] = useState("");
  const [car, setCar] = useState(emptyCar);
  const [isCLientSet, setIsClientSet] = useState(false);
  const [clientList, setClientList] = useState<
    {
      vehicleRegistrationNumber: string;
      discount: number;
      vehicleType: number;
    }[]
  >([]);
  const [inputErrors, setInputErrors] = useState(emptyCar);
  console.log(car, "car");
  const isValid =
    [inputErrors.regNumber, inputErrors.vehicleType].every((v) => !v) &&
    [car.regNumber, car.vehicleType].every((v) => v);
  console.log(inputErrors, "inputErrors");
  console.log(car, "car");

  const handleInput = (prop: string, value: string) => {
    setInputErrors({
      ...inputErrors,
      [prop]: validateCar[prop as keyof typeof validateCar](value),
    });
    setCar({ ...car, [prop]: value });
  };

  const handleFormSubmit = (e: React.MouseEvent<HTMLElement>) => {
    e.preventDefault();
    setError("");
    if (isValid) {
      const { regNumber, vehicleType, discount } = car;

      fetch(`${BASE_URL}/parking/add-vehicle`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          regNumber,
          vehicleType: vehicleTypes[vehicleType as keyof typeof vehicleTypes],
          discount:
            discountTypes[discount as keyof typeof discountTypes] || null,
          timeOfAdmission: new Date(),
        }),
      })
        .then((res) => res.json())
        .then((res) => {
          if (res.message) {
            setError(res.message);
            console.log(res);
          } else {
            setCarsList([
              ...carsList,
              {
                vehicleRegistrationNumber: res.registrationNumber,
                timeOfEntry: res.timeOfEntry,
                discount: res.discount,
                accumulatedCharge: null,
                totalCharge: null,
              },
            ]);
            setChangeAvailableSpaces(true);
            setCar(emptyCar);
            console.log(res);
          }
        });
    }
  };

  useEffect(() => {
    fetch(`${BASE_URL}/parking/get-all-client-vehicles`, {
      method: "GET",
    })
      .then((res) => res.json())
      .then((res) => {
        if (res.error) {
          setError(res.error.message);
        } else {
          setClientList(res);
        }
      });
  }, []);

  useEffect(() => {
    let client = clientList.find(
      (client) =>
        client.vehicleRegistrationNumber === car.regNumber.toUpperCase()
    );
    
      let clientData = {};

    if (client && !isCLientSet) {
      Object.entries(discountTypes).forEach(([key, value]) => {
        if (client?.discount === value) {
          console.log( "discount");
          clientData = {...clientData, discount:key};
        }
      });
      
      console.log(client, "client");

      Object.entries(vehicleTypes).forEach(([key, value]) => {
        if (client?.vehicleType === value) {
          console.log( "vehicleType");
          clientData = {...clientData, vehicleType:key};
        }
      });
       setCar({
            ...car,
            ...clientData
          });
      setIsClientSet(true);
    }
  }, [car, clientList, isCLientSet]);

  return (
    <Form className="create-car-form">
      <div className="register-number">
        <Form.Group className={inputErrors.regNumber ? "error" : ""}>
          <Form.Control
            type="text"
            name="regNumber"
            placeholder="Enter Register Number"
            value={car.regNumber}
            onChange={(e) => handleInput(e.target.name, e.target.value)}
          />
          <Form.Label className="visible">{`Registration Number ${inputErrors.regNumber}`}</Form.Label>
        </Form.Group>
      </div>
      <div className="vehicle-type">
        <Form.Group className={inputErrors.vehicleType ? "error" : ""}>
          <Form.Control
            as="select"
            name="vehicleType"
            value={car.vehicleType}
            onChange={(e) => handleInput(e.target.name, e.target.value)}
          >
            <option value="">Select Car Type</option>
            {Object.keys(vehicleTypes).map((car) => (
              <option value={car} key={car}>
                {car}
              </option>
            ))}
          </Form.Control>
          <Form.Label className="visible">{`Car Type ${inputErrors.vehicleType}`}</Form.Label>
        </Form.Group>
      </div>
      <div className="discount">
        <Form.Group className={inputErrors.vehicleType ? "error" : ""}>
          <Form.Control
            as="select"
            name="discount"
            value={car.discount}
            onChange={(e) => handleInput(e.target.name, e.target.value)}
          >
            <option value="">Select Discount</option>
            {Object.keys(discountTypes).map((discount) => (
              <option value={discount} key={discount}>
                {discount}
              </option>
            ))}
          </Form.Control>
          <Form.Label className="visible">{`Discount ${inputErrors.discount}`}</Form.Label>
        </Form.Group>
      </div>

      <div className="submit-btn">
        {error && (
          <Form.Group className="error">
            <p>{`${error}`}</p>
          </Form.Group>
        )}
        <Button type="submit" onClick={handleFormSubmit} disabled={!isValid}>
          Create
        </Button>
      </div>
    </Form>
  );
};

export default CreateCarForm;
