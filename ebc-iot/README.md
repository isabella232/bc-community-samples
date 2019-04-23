# Ethereum Blockchain Connector - Simulated IOT Device

In this example, we are going to simulate an IOT device that sends data to an IOT hub for temperature and humidity. It will monitor the incoming data stream via Azure Stream Analytics for a given range over a window of time (to filter anomalies).

If the data is outside the range we will log that event to an Ethereum based blockchain.

If you don't have an Azure subscription, [create a free account](https://azure.microsoft.com/free/) before you begin.

The following image illustrates the stages of the solution.

<include image of architecture>

## Description of components

### React.js IoT Device Simulator

A simple React application that has an interactive slider that can be used to modify the values of temperature and humidity.

### Node.js Backend

An Express.js application that uses the Node.js Device SDK for IoT Hub: https://github.com/Azure/azure-iot-sdk-node

### IOT Hub

An [IOT](https://docs.microsoft.com/en-gb/azure/iot-hub/) Hub to capture data from devices.

### Azure Stream Analytics

An [Azure Stream Analytics](https://docs.microsoft.com/en-gb/azure/stream-analytics/) to process data in real time

### Service Bus

A service bus to connect the output of the Azure Stream Analytics stage with a logic app that uses a Blockchain connector.

### Azure Logic App

The [Azure Logic App](https://docs.microsoft.com/en-gb/azure/logic-apps/) receives events from the Service bus, parse the information in a desired format and then logs the information in an smart contract located in the contract folder.

## Instructions

### IOT Hub

The first thing we need to do is to create the IOT Hub.

Please follow the instructions [Here](https://docs.microsoft.com/en-us/azure/iot-hub/tutorial-connectivity#create-an-iot-hub) to create it. Once we completed the creation, be sure to save the connection string credentials that we are going to need to be able to push data from out IOT device simulator.

### Backend

The backend uses [Express.js](https://expressjs.com/)

Go to the server folder and run

> npm install

Once completed, create replace in the .env file the value of your connection string for the IOT Hub that was created before.

Then, run

> npm start

We should be able to see the message 'App is listening on port 5000'

The backend has a single endpoint that can handle post requests in the '/message' route. 

Once the message is received, it will use the Azure IoT Node.js SDK to send a message to the IOT Hub. 

### React.js IOT Device Simulator

The demo client was built with [create-react-app](https://facebook.github.io/create-react-app/docs/adding-typescript) and typescript.

The client also uses [Ant library](https://ant.design/docs/react/introduce)

Go to the client folder and run 

> npm install 

> npm start

Then, open your browser and type http://localhost:3000/

The React application has 1 single component where we set the values of the slider to the internal state of the component. 

There is also a setInterval method that is sending the values of the sliders to the server via post, every 5000 miliseconds or 5 seconds.

### Summary

At this point you should be able to see in the express console values like:

'Request { temperature: 0, humidity: 0 }'

If you modify the slider, the values should change.



