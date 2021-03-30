import React from 'react';
import BpmnModeler from 'bpmn-js/lib/Modeler';

import 'bpmn-js/dist/assets/diagram-js.css';
import 'bpmn-js/dist/assets/bpmn-font/css/bpmn.css';
import '../css/diagram.css';

class BpmnModelerComponent extends React.Component {

    constructor() {
        super();

        this.containerRef = React.createRef();
    }
    
    async componentDidMount() {
        // Gets current container
        const container = this.containerRef.current;

        this.modeler = new BpmnModeler({
            container: container
        });

        await this.modeler.createDiagram();

        // access modeler components
        var canvas = this.modeler.get('canvas');

        // zoom to fit full viewport
        canvas.zoom('fit-viewport');

        this.addNavButtonEventHandlers();
    }

    addNavButtonEventHandlers() {
        const saveButton = document.getElementById('saveButton');
        saveButton.addEventListener('click', async (event) => {
            event.preventDefault()

            console.log("created event listener for save button clicks");
            this.saveDiagram();
        });

        const loadButton = document.getElementById('loadButton');
        loadButton.addEventListener('click', async (event) => {
            event.preventDefault()

            console.log("created event listener for load button clicks");

        });

        const newButton = document.getElementById('newButton');
        newButton.addEventListener('click', async (event) => {
            event.preventDefault()

            console.log("created event listener for new button clicks");

        });
    }

    componentWillUnmount() {
        this.modeler.destroy();

    }

    async saveDiagram() {
        // Get XML from the diagram
        // Save XML into file using name parameter
        // User will set name when using where to save

        const { xml } = await this.modeler.saveXML({ format: true });
        console.log(xml)

        window.external.sendMessage("saveFunc,".concat(xml));

    }

    openDiagram() {
        // Open file picker window
        // Load diagram

        // Send command to backend
        window.external.sendMessage("openFunc");

        // Wait for reply with XML to load diagram
        window.external.receiveMessage(async function (message) {
            // Split the message into command and value
            var command = message.split(",");

            // If the message isn't the command that we want then we just 
            //  return nothing
            if (command[0] !== "openFunc") return;

            // Display diagram
            await this.modeler.importXML(command[1])
        });
    }

    render() {
        return (
          <div id="canvas" className="react-bpmn-diagram-container" ref={ this.containerRef }></div>
        );
    }
}

export default BpmnModelerComponent;