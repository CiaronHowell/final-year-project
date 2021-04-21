/////////// NPM Imports ////////////////
import React from 'react';

////////// React Components ///////////////
import BpmnModelerComponent from './components/bpmnModelerComponent';
import WorkflowControlsComponent from './components/workflowControlsComponent';
import ContextProvider from './components/contextComponent';

////////// CSS Imports ////////////
import './css/App.css';

class App extends React.Component {

    render() {
        return (
            <div className="App">
                <div id="main" className="content">
                <ContextProvider>
                    <WorkflowControlsComponent/>
                    <BpmnModelerComponent/>
                </ContextProvider>
                
                </div>
            </div>
        );
    }
}

export default App;