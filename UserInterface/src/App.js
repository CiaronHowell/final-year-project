import React from 'react';

////////// React Components ///////////////
import BpmnModelerComponent from './components/bpmnModelerComponent';
import WorkflowControlsComponent from './components/workflowControlsComponent';

////////// CSS Imports ////////////
import './css/App.css';

class App extends React.Component {

  render() {
    return (
      <div className="App">
        <div id="main" className="content">
          <WorkflowControlsComponent/>
          <BpmnModelerComponent/>
        </div>
      </div>
    );
  }
}

export default App;