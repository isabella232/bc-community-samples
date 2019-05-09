require('dotenv').config()

const express = require('express');
const bodyParser = require('body-parser')
const cors = require('cors')

const app = express();

app.use(cors())

app.use(bodyParser.urlencoded({ extended: false }))

app.use(bodyParser.json())

app.get('/', (req, res) => {
    res.send("Backend working!");
});

const messageRoutes = require('./routes')

app.use('/message', messageRoutes)

const port = process.env.PORT || 5000;

app.listen(port, () => {
    console.log('App is listening on port ' + port);
});