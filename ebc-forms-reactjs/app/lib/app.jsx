"use strict";

class App extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      lastWordWritten: '',
      lastWordRead: null,
      error: null,
      loading: false
    };
  }

  onChange = (event) => {
    this.setState({ lastWordWritten: event.target.value });
  };

  // set last word
  set = () => {
    this.setState({loading: true});
    fetch(this.props.postUrl, { 
      method: "POST", 
      headers: { "Content-Type": "application/json; charset=utf-8" }, 
      body: JSON.stringify({ lastWord: this.state.lastWordWritten }) 
    })
    .then(response => this.setState({ loading: false }))
    .catch(error => this.setState({ error: error, loading: false }));
    return false;
  }

  // get last word
  get = () => {
    this.setState({ loading: true });
    fetch(this.props.getUrl, { 
      method: "GET", 
      headers: { "Content-Type": "application/json; charset=utf-8" }
    })
    .then(result => result.text())
    .then(result => {this.setState({lastWordRead: result, loading: false})})
    .catch(error => this.setState({error: error, loading: false}));
    return false;
  }

  render() {
    // report errors
    if (this.state.error) {
      var error = (
        <div class="row">
          <div class="column">
            <h5>{this.state.error}</h5>
          </div>
        </div>
      );
    }
    // main UI
    return (
      <div>
        <div class={`ui ${this.state.loading ? "active" : ""} dimmer`}>
          <div class="ui loader"></div>
        </div>
        <div class="ui middle aligned center aligned grid" style={{marginTop:"20px"}}>
          <div class="column">
            <div class="row">
              <h2 class="ui teal header">
                  <div class="content">
                      <i>ebc-forms-reactjs</i> demo
                  </div>
              </h2>
            </div>
            <div class="row" style={{ marginTop: "40px" }}>
              <a href={this.props.formUrl} target="_blank">
                <button class="ui primary button" style={{ marginRight: "20px" }}>
                  set with <em>Microsoft Form</em>
                </button>
              </a>                
            </div>
            <div class="row" style={{ marginTop: "10px" }}>
              <h4>&mdash; OR &mdash;</h4>
            </div>
            <div class="row" style={{ marginTop: "10px" }}>
              <div class="ui labeled input">
                <div class="ui label">
                  What's the last word?
                </div>
                <input type='text' value={this.state.lastWordWritten} onChange={this.onChange}></input>              
              </div>
              <button class="ui primary button" style={{ marginLeft: "20px" }} onClick={this.set}>
                set
              </button>
            </div>
            <div class="row" style={{ marginTop: "40px" }}>
              <button class="ui primary button" style={{ marginRight: "20px" }} onClick={this.get}>
                get
              </button>
              <div class="ui labeled input">
                <div class="ui label">
                  Last word fetched:
                </div>
                <input type='text' disabled value={this.state.lastWordRead}></input>
              </div>
            </div>
            {error}
          </div>
        </div>
      </div>
    );
  }
}