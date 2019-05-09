const Protocol = require('azure-iot-device-mqtt').Mqtt;
const Client = require('azure-iot-device').Client;
const client = Client.fromConnectionString(process.env.CONNECTION_STRING, Protocol);

const connect = async () => {
    return new Promise((resolve, reject) => {
        client.open((err) => {
            if (err) {
                console.log(chalk.red('Could not connect: ' + err));
                reject(err);
            } else {
                console.log('Client connected');
                resolve(client)
            }
        })
    })
}
module.exports = connect