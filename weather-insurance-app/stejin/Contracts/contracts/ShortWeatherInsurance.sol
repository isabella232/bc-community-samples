pragma solidity ^0.5.0;

import "./WeatherInsurance.sol";

/**
* @title ColdWeatherInsurance
* Pays out if weather condition measure at expiration is below contract condition "strike" measure
*/
contract ShortWeatherInsurance is WeatherInsurance {
   
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
      if (condition > forecast)
        return uint(condition - forecast) * notional;
      else
        return 0;
    }
}