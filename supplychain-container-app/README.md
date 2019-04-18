# supplychain-container-app
The minimalist design of a container's smart contract.

## Motivation
The main idea to create the minimalist design of a smart contract which defines shipping container with all items inside in the container, with tracing of container location and some extra data from sensors.

The design is suitable in case that shipping container is minimal abstraction during transportation. It means in case of transshipment there is not content changing in the shipping container.

## Key objects
 
1. Storage

    Contract stores all possible items (pallet, box, item) in its own storage. Items are stored in simple tree structure container->pallets->boxs->items. The structure allows passing all nodes in both directions. It is possible to retrieve all items from a container as well as retrieve all parent for specific item up to the root element(container). The structure is append-only.

2. Events

    The contract contains a number of events which allows tracking of contract's changing without doing any calls to the contract.

    Log memory is cheaper than storage, it allows optimize gas usage as well.

3. Trace

    It is data which extends information about shipping container. The data are container's coordinates, state of the container (loaded, shipped, arrived, unloaded) and extra information from internal/external oracles (temperature, co2, etc). You can imagine it as an event log, append-only storage with heterogeneous data. In the design I use log for this type of data. The log is cheaper than regular storage, it does not allow to manage data. It allows to retrieve all contract's events and mutch it to timestamps of blocks. In this way it is easily possible to recreate history.

## Gas usage

The contract optimized by storage and gas usage. In the case of internal storage usage, it spends less gas, in comparison to external contracts execution.

## Getting started

In order to play with the contract you need:
1. install truffle
    ```
    npm i truffle -g
    ```
2. install [ganache](https://truffleframework.com/ganache)
3. clone the repo & open project's dir
4. install packages
    ```
    npm i
    ```
5. compile contracts
    ```
    truffle compile
    ```
6. run tests
    ```
    truffle test
    ```