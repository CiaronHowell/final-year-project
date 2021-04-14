import React from 'react';

import BpmnModelerComponent from './components/bpmnModelerComponent';
import WorkflowControlsComponent from './components/workflowControlsComponent';
import NavBarComponent from './components/navBarComponent';

import './css/App.css';

class App extends React.Component {

  render() {
    return (
      <div className="App">
        <NavBarComponent/>
        <div id="main" className="content">
          <button id="openCloseNavButton">OpenClose</button>
          <WorkflowControlsComponent/>
          <BpmnModelerComponent/>
        </div>
      </div>
    );
  }
}

export default App;