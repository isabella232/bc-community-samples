//jshint ignore: start

const InsuranceContract = artifacts.require('./LongWeatherInsurance.sol');

contract('HotWeatherInsurance', function(accounts) {
  let testContract;
  const owner = accounts[0];
  const operator = accounts[1];
  const user1 = accounts[2];
  const user2 = accounts[3];
  const user3 = accounts[4];
  const user4 = accounts[5];

  it('should be deployed', async () => {
    testContract = await InsuranceContract.new('London', 1580472000000, 32, 1000000);

    assert(testContract.address !== undefined, 'Contract not deployed');
  });

  it('should be able to fund contract', async () => {
    
    await testContract.sendTransaction({from: owner, value: 2000000});

    const balance = await web3.eth.getBalance(testContract.address);

    assert.equal(balance, 2000000, 'Balance not correct');
  });

  it('should be able to set minimum premium', async () => {
    const oldPremium = await testContract.minimumPremium();

    await testContract.setMinimumPremium(2000000);

    const newPremium = await testContract.minimumPremium();

    assert.isFalse(oldPremium === newPremium, 'Old and new premium should be different');
    assert.equal(newPremium, 2000000, 'New premium not correct');
  });

  it('should be able to set low forecast', async () => {
    await testContract.updateForecast(1580299200000, 28, 33);

    const premium = await testContract.getPremium(1000000000000);

    assert.equal(premium, 330002000000, 'New premium not correct ' + premium.toString());
  });

  it('should be able to set high forecast', async () => {
    await testContract.updateForecast(1580299200000, 35, 33);

    const premium = await testContract.getPremium(1000000000000);

    assert.equal(premium, 3330002000000, 'New premium not correct ' + premium.toString());
  });

  it('should be able to buy insurance', async () => {
    await testContract.buyInsurance(1000000000000, {from: user1, value: 3330002000000});

    const users = await testContract.getUsers();

    assert.equal(users.length, 1, 'Number of users not correct');

    const balance = await web3.eth.getBalance(testContract.address);

    assert.equal(balance, 3330004000000, 'Contract balance not correct');

    const position = await testContract.getPosition(user1);

    assert.equal(1000000000000, position[0], 'Position not correct');
  
  });

  it('should be able to pay out insurance', async () => {
    await testContract.updateForecast(1580472000000, 30, 33);

    const position = await testContract.getPosition(user1);

    const intrinsicValue = await testContract.getIntrinsicValue(position[0]);

    assert.equal(intrinsicValue, 0, 'Intrinsic value not correct');

    // Nothing will be paid
    await testContract.payOut(user1);

    const balance = await web3.eth.getBalance(testContract.address);

    assert.equal(balance, 3330004000000, 'Contract balance not correct');
  
  });

  it('should be able to terminate contract once expired and closed', async () => {
    await testContract.destroy();
  });

});