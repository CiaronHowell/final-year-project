import './css/App.css';
import BpmnModelerComponent from './components/bpmnModelerComponent';
import FlowchartControlsComponent from './components/flowchartControlsComponent';
import NavBarComponent from './components/navBarComponent';

function App() {
  return (
    <div className="App">
      <NavBarComponent/>
      <div id="main" class="content">
        <button id="openCloseNavButton">OpenClose</button>
        <FlowchartControlsComponent/>
        <BpmnModelerComponent/>
      </div>
    </div>
  );
}

export default App;