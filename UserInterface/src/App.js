import React from 'react';

import BpmnModelerComponent from './components/bpmnModelerComponent';
import FlowchartControlsComponent from './components/flowchartControlsComponent';
import NavBarComponent from './components/navBarComponent';

import './css/App.css';

class App extends React.Component {

  render() {
    return (
      <div className="App">
        <NavBarComponent/>
        <div id="main" className="content">
          <button id="openCloseNavButton">OpenClose</button>
          <FlowchartControlsComponent/>
          <BpmnModelerComponent/>
        </div>
      </div>
    );
  }
}

export default App;