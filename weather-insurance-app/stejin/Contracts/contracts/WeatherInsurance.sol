pragma solidity ^0.5.0;

/**
 * @title WeatherInsurance
 * @dev Base Class
 * This is the base class for Weather Insurance Contracts and should not be used directly
 */
 contract WeatherInsurance {
    address payable public owner;
    mapping(address => bool) public operators;
    string public location;
    uint32 public expirationTime; // UTC Unix timestamp
    int32 public condition; // Weather condition, e.g. temperature, huminidity etc "strike" in F
    uint32 public valuationTime; // UTC Unix timestamp
    int32 public forecast; // Weather condition forecast at valuation time in F
    uint32 public forecastRisk; // Variability of forecast - e.g. standard deviation as percent of temperature in K [%]
    mapping(address => Position) internal positions;
    uint public minimumPremium;
    address[] private users;

    struct Position {
      uint notional;
      uint premium;
    }
   
    /**
    * @dev Constructor
    */
    constructor() public {
      owner = msg.sender;
    }

    /**************
    * Modifiers
    ***************/

    modifier onlyOwner() {
      require(msg.sender == owner, "Not authorized.");
      _;
    }

    modifier onlyOwnerOrOperator() {
      require(msg.sender == owner || operators[msg.sender], "Not authorized.");
      _;
    }

    /**************
    * Events
    ***************/

    event InsuranceBought(address user, uint notional, uint premium);
    event InsurancePaid(address user, uint notional, uint amount);

    /**************
    * Restricted
    ***************/

    function addOperator(address operatorAddress) external onlyOwner {
      operators[operatorAddress] = true;
    }

    function removeOperator(address operatorAddress) external onlyOwner {
      operators[operatorAddress] = false;
    }

    function getUsers() external view onlyOwnerOrOperator returns (address[] memory) {
      return users;
    }

    function setMinimumPremium(uint premium) external onlyOwner {
      require(premium > 0, "Premium must be greater than zero.");
      minimumPremium = premium;
    }

    function updateForecast(uint32 currentTime, int32 currentForecast, uint32 currentRisk) external onlyOwnerOrOperator {
      valuationTime = currentTime;
      forecast = currentForecast;
      forecastRisk = currentRisk;
    }

    function payOut(address payable user) external onlyOwner {
      require(valuationTime >= expirationTime, "Contract still active.");
      uint notional = positions[user].notional;
      uint amountToPay = getIntrinsicValue(notional);
      // Clear position
      positions[user].notional = 0;
      // Pay if needed
      if (amountToPay > 0) {
        user.transfer(amountToPay);
        emit InsurancePaid(user, notional, amountToPay);
      }
    }

    function isSettled() public view onlyOwnerOrOperator returns(bool) {
      for (uint i = 0; i < users.length; i++) {
        if (positions[users[i]].notional > 0)
          return false;
      }
      return true;
    }

    // return remaining balance to owner
    function destroy() external onlyOwner {
      require(valuationTime >= expirationTime, "Contract still active.");
      require(isSettled(), "All positions must be closed.");
      selfdestruct(owner);
    }

    /**************
    * Public
    ***************/

    function getPosition(address user) external view returns (uint, uint) {
      return (positions[user].notional, positions[user].premium);
    }

    function getIntrinsicValue(uint notional) public view returns (uint);

    function getPremium(uint notional) public view returns (uint) {
      return getIntrinsicValue(notional) + notional * forecastRisk / 100 + minimumPremium;
    }

    function buyInsurance(uint notional) payable external {
      require(valuationTime > 0, "Contract not active.");
      require(expirationTime >= valuationTime, "Contract expired.");
      require(notional > 0, "Notional must be greater than zero.");
      require(msg.value >= getPremium(notional), "Insufficient premium.");

      // Register user
      if (positions[msg.sender].notional == 0)
        users.push(msg.sender);

      positions[msg.sender].notional += notional;
      positions[msg.sender].premium += msg.value;
      emit InsuranceBought(msg.sender, notional, msg.value);
    }

    // Fallback - used by owner to fund contract
    function() external payable {}

}