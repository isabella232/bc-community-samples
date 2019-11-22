pragma solidity ^0.5.0;

import "./WeatherInsurance.sol";

/**
* @title LongWeatherInsurance
* Pays out if weather condition at expiration is above contract weather condition
*/
contract LongWeatherInsurance is WeatherInsurance {
   
    /**
    * @dev Constructor
    */
    constructor(string memory contractLocation, uint32 contractExpirationTime, int32 contractCondition, uint contractPremium) public {
      owner = msg.sender;
      location = contractLocation;
      expirationTime = contractExpirationTime;
      condition = contractCondition;
      minimumPremium = contractPremium;
    }

    function getIntrinsicValue(uint notional) public view returns (uint) {
      if (forecast > condition)
        return uint(forecast - condition) * notional;
      else
        return 0;
    }
}