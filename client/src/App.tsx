import { useState, useEffect } from 'react';
import { Button } from 'react-bootstrap';
import './App.css';
import { BASE_URL } from './common/constants';
import getDate from './common/utils/getDate';
import CreateCarForm from './components/Create-form';
import CarType from './types/carType';

const App: React.FC = () => {
  const [createMode, setCreateMode] = useState(false);
  const [carsList, setCarsList] = useState<CarType[] | []>([]);
  const [error, setError] = useState('');
  const [availableSpaces, setAvailableSpaces] = useState(1);
  const [changeAvailableSpaces, setChangeAvailableSpaces] = useState(false);

  const deleteCarHandler = (vehicleRegistrationNumber: string) => {
    fetch(`${BASE_URL}/parking/remove-vehicle`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        vehicleRegistrationNumber        
      })
    })
      .then((res) => res.json())
      .then((res) => {
        if (res.error) {
          setError(res.error.message);
        } else {
          setCarsList(
            carsList.filter((car) => car.vehicleRegistrationNumber !== vehicleRegistrationNumber)
          );
          setChangeAvailableSpaces(true);
        }
      });
  };

  useEffect(() => {
    fetch(`${BASE_URL}/parking/get-all-parked-vehicles`, {
      method: 'GET'
    })
      .then((res) => res.json())
      .then((res) => {
        if (res.error) {
          setError(res.error.message);
        } else {
          setCarsList(res);
        }
      });
  }, []);

  useEffect(() => {
    fetch(`${BASE_URL}/parking/get-available-spaces`, {
      method: 'GET'
    })
      .then((res) => res.json())
      .then((res) => {
        if (res.error) {
          setError(res.error.message);
        } else {
          setAvailableSpaces(res.availableSpaces);
          setChangeAvailableSpaces(false);
        }
      });
  }, [changeAvailableSpaces]);

  return (
    <div className="container">
      <div className="header">Parking Management System</div>
      {error ? <div className="error">{error}</div> : <></>}
      <div className="cars-container">
      <>
        <div >
          Parking spaces available: {availableSpaces}
        </div>
      
        <Button
          className="create-btn"
          onClick={() => setCreateMode(!createMode)}
          disabled={availableSpaces <= 0}
        >
          {createMode ? 'Close Form' : 'Add Car'}
        </Button>
        {createMode ? (
          <div className="create-form card">
            <CreateCarForm carsList={carsList} setCarsList={setCarsList} setChangeAvailableSpaces={setChangeAvailableSpaces}/>
          </div>
        ) : (
          <></>
        )}
        </>

        <ul className="car-list">
          {carsList?.map(
            ({
              vehicleRegistrationNumber,
              timeOfEntry,
              accumulatedCharge,
              discount,
              totalCharge
            }) => (
              <li className="car card" key={vehicleRegistrationNumber}>
                <div className="content">
                  <div className="reg-number">Vehicle Number: {vehicleRegistrationNumber}</div>
                  <div className="time">{getDate(timeOfEntry)}</div>
                  <div className="cost">
                    <table>
                      <tr>
                        <td>Charge</td>
                        <td>{`${(accumulatedCharge!/1).toFixed(2)} BGN`}</td>
                      </tr>
                      <tr>
                        <td>Discount</td>
                        <td>{`${((discount ? discount:0)/1).toFixed(2) || 0} BGN`}</td>
                      </tr>
                      <tr>
                        <td>Net Charge</td>
                        <td>{`${(totalCharge!/1).toFixed(2)} BGN`}</td>
                      </tr>
                    </table>
                  </div>
                </div>
                <div className="delete-btn">
                  <Button onClick={() => deleteCarHandler(vehicleRegistrationNumber)}>
                    <i className="fa fa-times"></i>
                  </Button>
                </div>
              </li>
            )
          )}
        </ul>
      </div>
    </div>
  );
};

export default App;
