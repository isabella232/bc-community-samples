pragma solidity =0.5.0;
pragma experimental ABIEncoderV2;

contract MailingList {
    address public owner = msg.sender;

    mapping(string => bool) HashedMailAddresses;

    string[] HashedMailAddressesArray; // for quick enumeration of added mail addresses

    event HashedMailAddressAdded(string hashedMailAddress);

    // to be compliant with GDPR, only hashed mail addresses should be added
    function addMailAddress(string memory hashedMailAddress) public {
        require(HashedMailAddresses[hashedMailAddress] == false); // check duplication
        HashedMailAddresses[hashedMailAddress] = true;
        HashedMailAddressesArray.push(hashedMailAddress);
        emit HashedMailAddressAdded(hashedMailAddress);
    }

    function getHashedMailAddressesArray() public view returns(string[] memory) {
        return HashedMailAddressesArray;
    }
}
