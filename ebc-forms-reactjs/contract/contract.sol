  pragma solidity ^0.4.18;

  contract LastWord {

    string lastWord;

    constructor(string memory initialLastWord) public {
        lastWord = initialLastWord;
    }

    function set(string memory newLastWord) public returns (bool) {
        lastWord = newLastWord;
        return true;
    }

    function get() public view returns (string memory) {
      return lastWord;
    }
  }