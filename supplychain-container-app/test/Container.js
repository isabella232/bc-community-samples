const { shouldFail, expectEvent } = require('openzeppelin-test-helpers');
const { expect } = require('chai');

const Container = artifacts.require('Container.sol');

let container;

contract('Container', (accounts) => {
    describe('Pallets', function () {
        before(async () => {
            container = await Container.new({ from: accounts[0] });
        });

        it('should add pallet', async () => {
            // act
            const { logs } = await container.addPallet({ from: accounts[0] });

            // assert
            expectEvent.inLogs(logs, 'LogPallet');
            expect(await container.getPalletsCount()).to.be.bignumber.equal('1');
        });

        it('should not add pallet by not owner', async () => {
            // act-assert
            await shouldFail.reverting(container.addPallet({ from: accounts[1] }));
        });
    });

    describe('Boxes', function () {
        before(async () => {
            container = await Container.new({ from: accounts[0] });
        });

        it('should not add box for non existing pallet', async () => {
            // act-assert
            await shouldFail.reverting(container.addBox(0, { from: accounts[0] }));
        });

        it('should add box', async () => {
            // arrange
            await container.addPallet({ from: accounts[0] });

            // act
            const { logs } = await container.addBox(0, { from: accounts[0] });

            // assert
            expectEvent.inLogs(logs, 'LogBox');
            expect(await container.getBoxesCount()).to.be.bignumber.equal('1');
        });

        it('should not add box by not owner', async () => {
            // arrange
            await container.addPallet({ from: accounts[0] });

            // act-assert
            await shouldFail.reverting(container.addBox(0, { from: accounts[1] }));
        });
    })

    describe('Items', function () {
        before(async () => {
            container = await Container.new({ from: accounts[0] });
        });

        it('should not add item for non existing box', async () => {
            // act-assert
            await shouldFail.reverting(container.addItem(0, 1, { from: accounts[1] }));
        });

        it('should add item', async () => {
            // arrange
            await container.addPallet({ from: accounts[0] });
            await container.addBox(0, { from: accounts[0] })

            // act
            const { logs } = await container.addItem(0, 1, { from: accounts[0] });

            // assert
            expectEvent.inLogs(logs, 'LogItem');
            expect(await container.getItemsCount()).to.be.bignumber.equal('1');
        });

        it('should not add item by not owner', async () => {
            // arrange
            await container.addPallet({ from: accounts[0] });
            await container.addBox(0, { from: accounts[0] })

            // act-assert
            await shouldFail.reverting(container.addItem(0, 1, { from: accounts[1] }));
        });
    });
});
