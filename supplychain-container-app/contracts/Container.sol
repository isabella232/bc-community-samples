pragma solidity ^0.5.4;

import "openzeppelin-solidity/contracts/ownership/Ownable.sol";

// The minimalist design of a container's smart contract.
// Storage:
//      Contract stores all possible items (pallet, box, item) in its own storage.
//      Items are stored in simple tree structure container->pallets->boxs->items. The structure allows passing all nodes in both directions.
//      It is possible to retrieve all items from a container as well as retrieve all parent for specific item up to the root element(container).
//      The structure is append-only.
// Events:
//      The contract contains a number of events which allows tracing of contract's changing without doing any calls to the contract.
//      Log memory is cheap, it allows optimize gas usage as well.
contract Container is Ownable {
    struct Node {
        uint256 parentId;
        uint256[] childs;
    }
    
    struct Leaf {
        uint256 id;
        uint256 parentId;
    }

    event LogPallet(uint256 indexed _index);
    event LogBox(uint256 indexed _palletIndex, uint256 indexed _index);
    event LogItem(uint256 indexed _boxIndex, uint256 indexed _index, uint256 indexed _id);
    event LogTrace(address indexed _from, string _msg);

    uint256 private palletIndex = 0;
    Node[] private pallets;
    mapping(uint256 => address) palletOwners;

    uint256 private boxIndex = 0;
    Node[] private boxes;

    uint256 private itemIndex = 0;
    Leaf[] private items;

    /**
        @notice Adds pallet to the container
        @dev MUST emit LogPallet event on success
        @param _owner       Pallet's owner, the field cannot be empty
        @return             Index of created pallet
    */
    function addPallet(address _owner) external onlyOwner returns(uint256 index) {
        require(_owner != address(0x0), "_owner must be non-zero.");

        pallets.push(Node({
            parentId: 0,
            childs: new uint256[](0)
        }));
        index = palletIndex;
        palletOwners[index] = _owner;
        palletIndex++;

        emit LogPallet(index);
    }

    /**
        @notice Adds box linked to specified pallet
        @dev MUST emit LogBox event on success
        @param _palletIndex     Pallet's index, pallet should exists
        @return                 Index of created box
    */
    function addBox(uint256 _palletIndex) external onlyOwner returns(uint256 index) {
        require(_palletIndex < palletIndex);

        boxes.push(Node({
            parentId: _palletIndex,
            childs: new uint256[](0)
        }));
        pallets[_palletIndex].childs.push(boxIndex);
        index = boxIndex;
        boxIndex++;

        emit LogBox(_palletIndex, index);
    }

    /**
        @notice Adds item linked to specified box
        @dev MUST emit LogItem event on success
        @param _boxIndex    Box's index, box should exists
        @param _id          Item's identifier
        @return             Index of created item
    */
    function addItem(uint256 _boxIndex, uint256 _id) external onlyOwner returns(uint256 index) {
        require(_boxIndex < boxIndex);

        items.push(Leaf({
            id: _id,
            parentId: _boxIndex
        }));
        boxes[_boxIndex].childs.push(itemIndex);
        index = itemIndex;
        itemIndex++;

        emit LogItem(_boxIndex, index, _id);
    }

    /**
        @notice Adds data to the log. It allows store dynamic data.
    */
    function trace(string memory _msg) public {
        emit LogTrace(msg.sender, _msg);
    }

    function getPalletsCount() public view returns(uint256) {
        return palletIndex;
    }

    function getBoxesCount() public view returns(uint256) {
        return boxIndex;
    }

    function getItemsCount() public view returns(uint256) {
        return itemIndex;
    }

    function getPallet(uint256 _index) public view returns(uint256, uint256[] memory, address) {
        require(_index < palletIndex);
        return (pallets[_index].parentId, pallets[_index].childs, palletOwners[_index]);
    }

    function getBox(uint256 _index) public view returns(uint256, uint256[] memory) {
        require(_index < boxIndex);
        return (boxes[_index].parentId, boxes[_index].childs);
    }
    
    function getItem(uint256 _index) public view returns(uint256, uint256) {
        require(_index < boxIndex);
        return (items[_index].id, items[_index].parentId);
    }
}