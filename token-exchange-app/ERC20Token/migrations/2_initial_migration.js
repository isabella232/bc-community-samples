var ERC20TokenRingaile = artifacts.require("ERC20TokenRingaile");

module.exports = function(deployer) {
  deployer.deploy(ERC20TokenRingaile, "RingaileToken", "Rin", 18, 1000);
};