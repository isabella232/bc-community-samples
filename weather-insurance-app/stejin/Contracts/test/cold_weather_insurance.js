//jshint ignore: start

const InsuranceContract = artifacts.require('./ColdWeatherInsurance.sol');

contract('ColdWeatherInsurance', function(accounts) {
  let testContract;
  const owner = accounts[0];
  const operator = accounts[1];
  const user1 = accounts[2];
  const user2 = accounts[3];
  const user3 = accounts[4];
  const user4 = accounts[5];

  it('should be deployed', async () => {
    testContract = await InsuranceContract.new(operator, 'London', 1580472000000, 32, 1000000);

    assert(testContract.address !== undefined, 'Contract not deployed');
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

    assert.equal(premium, 4330002000000, 'New premium not correct ' + premium.toString());
  });

  it('should be able to set high forecast', async () => {
    await testContract.updateForecast(1580299200000, 35, 33);

    const premium = await testContract.getPremium(1000000000000);

    assert.equal(premium, 330002000000, 'New premium not correct ' + premium.toString());
  });

  it('should be able to buy insurance', async () => {
    await testContract.buyInsurance(1000000000000, {from: user1, value: 330002000000});

    const users = await testContract.getUsers();

    assert.equal(users.length, 1, 'Number of users not correct');

    const balance = await web3.eth.getBalance(testContract.address);

    assert.equal(balance, 330002000000, 'Contract balance not correct');

    const position = await testContract.positions(user1);

    assert.equal(1000000000000, position, 'Position not correct');
  
  });

  it('should be able to pay out insurance', async () => {
    await testContract.updateForecast(1580472000000, 30, 33);

    const position = await testContract.positions(user1);

    const intrinsicValue = await testContract.getIntrinsicValue(position);

    assert.equal(intrinsicValue, 2000000000000, 'Intrinsic value not correct');

    await testContract.fund({value: 1669998000000});

    await testContract.payOut(user1);

    const balance = await web3.eth.getBalance(testContract.address);

    assert.equal(balance, 0, 'Contract balance not correct');
  
  });

  it('should be able to terminate contract once expired and closed', async () => {
    await testContract.destroy();
  });

});