pragma solidity ^0.5.0;

/**
 * @title WeatherInsurance
 * @dev Base Class
 * This is the base class for Weather Insurance Contracts and should not be used directly
 */
 contract WeatherInsurance {
    address payable public owner;
    address public operator;
    string public location;
    uint32 public expirationTime; // UTC Unix timestamp
    int32 public temperature; // Temperature "strike" in F
    uint32 public valuationTime; // UTC Unix timestamp
    int32 public forecast; // Temperature forecast at valuation time in F
    uint32 public forecastRisk; // Variability of forecast - standard deviation as percent of temperature [%]
    mapping(address => uint64) public positions;
    uint256 public minimumPremium;
    address[] private users;
   
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
      require(msg.sender == owner || msg.sender == operator, "Not authorized.");
      _;
    }

    /**************
    * Events
    ***************/

    event InsuranceBought(address user, uint64 notional, uint256 premium);
    event InsurancePaid(address user, uint64 notional, uint256 amount);

    /**************
    * Restricted
    ***************/

    function getUsers() external view onlyOwnerOrOperator returns (address[] memory) {
      return users;
    }

    function setMinimumPremium(uint256 premium) external onlyOwner {
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
      uint64 notional = positions[user];
      uint256 amountToPay = getIntrinsicValue(notional);
      // Clear position
      positions[user] = 0;
      // Pay if needed
      if (amountToPay > 0) {
        user.transfer(amountToPay);
        emit InsurancePaid(user, notional, amountToPay);
      }
    }

    function isSettled() public view onlyOwnerOrOperator returns(bool) {
      for (uint i = 0; i < users.length; i++) {
        if (positions[users[i]] > 0)
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

    function getIntrinsicValue(uint64 notional) public view returns (uint256);

    function getPremium(uint64 notional) public view returns (uint256) {
      return getIntrinsicValue(notional) + notional * uint256(forecastRisk) / 100 + minimumPremium;
    }

    function buyInsurance(uint64 notional) payable external {
      require(valuationTime > 0, "Contract not active.");
      require(expirationTime >= valuationTime, "Contract expired.");
      require(notional > 0, "Notional must be greater than zero.");
      require(msg.value >= getPremium(notional), "Insufficient premium.");

      // Register user
      if (positions[msg.sender] == 0)
        users.push(msg.sender);

      positions[msg.sender] += notional;
      emit InsuranceBought(msg.sender, notional, msg.value);
    }

    // Fallback - used by owner to fund contract
    function fund() payable external {
    }

}