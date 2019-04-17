pragma solidity ^0.5.0;

import "./WeatherInsurance.sol";

/**
* @title ColdWeatherInsurance
* Pays out if temperature at expiration is below contract temperature
*/
contract ColdWeatherInsurance is WeatherInsurance {
   
    /**
    * @dev Constructor
    */
    constructor(address contractOperator, string memory contractLocation, uint32 contractExpirationTime, int32 contractTemperature, uint256 contractPremium) public {
      owner = msg.sender;
      operator = contractOperator;
      location = contractLocation;
      expirationTime = contractExpirationTime;
      temperature = contractTemperature;
      minimumPremium = contractPremium;
    }

    function getIntrinsicValue(uint64 notional) public view returns (uint256) {
      if (temperature > forecast)
        return uint256(temperature - forecast) * notional;
      else
        return 0;
    }
}