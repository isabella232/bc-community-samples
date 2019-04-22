"use strict";

class App extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      lastWordWritten: '',
      lastWordRead: null,
      error: null
    };
  }

  onChange = (event) => {
    this.setState({ lastWordWritten: event.target.value });
  };

  // set last word
  set = () => {
    fetch(this.props.postUrl, { 
      method: "POST", 
      headers: { "Content-Type": "application/json; charset=utf-8" }, 
      body: JSON.stringify({ lastWord: this.state.lastWordWritten }) 
    })
    .catch(error => this.setState({ error: error }));
    return false;
  }

  // get last word
  get = () => {
    fetch(this.props.getUrl, { 
      method: "GET", 
      headers: { "Content-Type": "application/json; charset=utf-8" }
    })
    .then(result => result.text())
    .then(result => {this.setState({lastWordRead: result})})
    .catch(error => this.setState({error: error}));
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
        <div class="ui middle aligned center aligned grid" style={{marginTop:"20px"}}>
          <div class="row">
            <div class="column">
              <h2 class="ui teal header">
                  <div class="content">
                      <i>ebc-forms-reactjs</i> demo
                  </div>
              </h2>
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
          </div>
          <div class="row">                     
            <div class="column">
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
          </div>
          {error}
        </div>
      </div>
    );
  }
}