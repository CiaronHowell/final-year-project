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

        // Add the event handlers only once
        this.addNavButtonEventHandlers();
        this.addWindowMessageEventHandlers();
    }

    componentWillUnmount() {
        this.modeler.destroy();
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
            this.loadDiagram();
        });

        const newButton = document.getElementById('newButton');
        newButton.addEventListener('click', async (event) => {
            event.preventDefault()

            console.log("created event listener for new button clicks");
            this.modeler.createDiagram();
        });
    }

    addWindowMessageEventHandlers() {
        // Wait for reply with XML to load diagram
        window.external.receiveMessage(async (message) => {
            // Split the message into command and value
            var command = message.split(",");

            // If the message isn't the command that we want then we just 
            //  return nothing
            if (command[0] !== "loadDiagramFunc") return;

            // Display diagram
            await this.modeler.importXML(command[1])

            console.log(this.count++);
        });
    }

    async saveDiagram() {
        // Save the current diagram
        const { xml } = await this.modeler.saveXML({ format: true });
        console.log(xml)

        // Send the command plus the diagram in XML form
        window.external.sendMessage("saveFunc,".concat(xml));
    }

    loadDiagram() {
        // Send command and let the backend handle it
        window.external.sendMessage("openFunc");
    }

    render() {
        return (
          <div id="canvas" className="react-bpmn-diagram-container" ref={ this.containerRef }></div>
        );
    }
}

export default BpmnModelerComponent;