import React, { Component } from 'react';

import { Slider, Row, Col } from 'antd';
import { SliderValue } from 'antd/lib/slider';

import 'antd/dist/antd.css';

import axios from "./axios"

const marks = {
  0: '0°C',
  25: '25°C',
  50: '50°C',
  100: {
    style: {
      color: '#f50',
    },
    label: <strong>100°C</strong>,
  },
};

interface State {
  humidity: number;
  temperature: number;
}

class App extends Component<{}, State> {

  constructor(props: {}) {
    super(props)
    this.state = {
      humidity: 0,
      temperature: 0
    }
  }

  componentDidMount() {
    setInterval(() => {
      axios.post('/message', {
        temperature: this.state.temperature,
        humidity: this.state.humidity
      })
        .then(function (response) {
          console.log("Data sent successfully")
          console.log(response);
        })
        .catch(function (error) {
          console.log("There was an error while sending the data")
          console.log(error);
        });

    }, 5000)
  }

  onTemperatureAfterChange = (value: SliderValue) => {
    console.log('onTemperatureAfterChange: ', value);
    this.setState({
      temperature: value as number
    })
  }

  onHumidityAfterChange = (value: SliderValue) => {
    console.log('onHumidityAfterChange: ', value);
    this.setState({
      humidity: value as number
    })
  }

  render() {
    const { humidity, temperature } = this.state;
    return (
      <div>
        <Row type="flex" justify="center" align="top">
          <h1>IoT Device Simulator</h1>
        </Row>
        <Row type="flex" justify="center" align="top">
          <Col span={12}>
            <h4>Temperature</h4>
            <Slider marks={marks} defaultValue={humidity} onAfterChange={this.onTemperatureAfterChange} />
            <h4>Humidity</h4>
            <Slider marks={marks} defaultValue={temperature} onAfterChange={this.onHumidityAfterChange} />
          </Col>
        </Row>
      </div>
    );
  }
}

export default App;
