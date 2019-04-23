const express = require('express')
const router = express.Router()

const Message = require('azure-iot-device').Message;
const connect = require('./client-connection');

router.post('/', async (req, res) => {
    console.log("Request", req.body)
    const clientConnection = await connect();
    const { temperature, humidity } = req.body;
    const time = new Date();
    console.log("Temperature, humidity", temperature, humidity)
    const data = JSON.stringify({ temperature, humidity, time });
    const message = new Message(data);

    console.log('Sending message: ' + message.getData());

    clientConnection.sendEvent(message, printResultFor('Send telemetry'));
    res.sendStatus(200);
})

function printResultFor(op) {
    return function printResult(err, res) {
        if (err) console.log(op + ' error: ' + err.toString());
        if (res) console.log(op + ' status: ' + res.constructor.name);
    };
}

module.exports = router