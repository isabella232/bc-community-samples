pragma solidity >=0.4.24;

import "../node_modules/openzeppelin-solidity/contracts/token/ERC20/ERC20.sol";
import "../node_modules/openzeppelin-solidity/contracts/token/ERC20/ERC20Detailed.sol";

contract ERC20TokenRingaile is ERC20, ERC20Detailed{

	constructor(string memory _name, string memory _symbol, uint8 _decimals, uint _initialSupply) 
    ERC20Detailed(_name, _symbol, _decimals) public {
        require(_initialSupply > 0, "INITIAL_SUPPLY has to be greater than 0");
        _mint(msg.sender, _initialSupply);
    }

    //event to listen on azure logic app
    event Transfer(address from, address to, uint256 supply);

    // transfer tokens from a given address to another given address
    function transferFrom(address from, address to, uint256 value) public returns (bool) {
    	emit Transfer(from, to, value);
        return super.transferFrom(from, to, value);
    }

}