import React from 'react';
import BpmnModeler from 'bpmn-js/lib/Modeler';

// Imports the moddle json file 
import parameterModdleExtension from '../moddle/parameters';

import 'bpmn-js/dist/assets/diagram-js.css';
import 'bpmn-js/dist/assets/bpmn-font/css/bpmn.css';
import '../css/diagram.css';

import PropertiesPanel from '../scripts/propertiesPanel';

import {Context} from '../components/contextComponent';

class BpmnModelerComponent extends React.Component {
    constructor() {
        super();

        this.containerRef = React.createRef();
    }

    componentDidMount() {
        // // Send initialise flag to backend to get classes and methods
        // window.external.sendMessage("loadModdlesFunc");
        this.initialiseBPMN()

        // Add the event handlers only once
        // this.addNavButtonEventHandlers();
        this.addWindowMessageEventHandlers();
    }

    async initialiseBPMN() {
        // Gets current container
        const container = this.containerRef.current;
        this.modeler = new BpmnModeler({
            container: container,
            moddleExtensions: {
                method: parameterModdleExtension
            },
            keyboard: {
                bindTo: document.body
            }
        });

        await this.modeler.createDiagram();

        // access modeler components
        var canvas = this.modeler.get('canvas');

        // zoom to fit full viewport
        canvas.zoom('fit-viewport');

        // Tell backend to send us the moduleInfo
        window.external.sendMessage('loadModuleInfoFunc');
    }

    componentWillUnmount() {
        this.modeler.destroy();
    }

    addWindowMessageEventHandlers() {
        // Wait for module info
        window.external.receiveMessage((message) => {
            var command = message.split(/,(.+)/);

            if (command[0] !== "loadModuleInfoReply") return;

            console.log(this.modeler);
            const moduleInfo = JSON.parse(command[1]);
            // Never used as it's just to create properties panel
            const panel = new PropertiesPanel({
                container: document.querySelector('#properties-panel'),
                modeler: this.modeler,
                moduleInfo
            });
        });

        // Wait for reply with XML to load diagram
        window.external.receiveMessage(async (message) => {
            // Split the message into command and value
            var command = message.split(",");

            console.log('hit')

            // If the message isn't the command that we want then we just 
            //  return nothing
            if (command[0] !== "loadDiagramFunc") return;

            console.log(command[1]);

            // Display diagram
            await this.modeler.importXML(command[1])

            console.log(this.modeler);
        });

        // Highlight the current task
        window.external.receiveMessage((message) => {
            var command = message.split(",");

            if (command[0] !== "currentTask") return;

            console.log(command[1]);
        })
    }

    async saveDiagram() {
        console.log("Attempting to save");

        // Save the current diagram
        const { xml } = await this.modeler.saveXML({ format: true });
        console.log(xml)

        // Send the command plus the diagram in XML form
        window.external.sendMessage("saveFunc,".concat(xml));
    }

    loadDiagram() {
        console.log("Attempting to load");
                
        // Send command and let the backend handle it
        window.external.sendMessage("openFunc");
    }

    newDiagram() {
        console.log("Attempting to create new diagram");

        // Added into it's own function for future use
        this.modeler.createDiagram();
    }

    render() {
        return (
            <div id="canvas" className="react-bpmn-diagram-container" ref={ this.containerRef }>
                <Context.Consumer>
                    {(context) => (
                        <p>{(context.state.running) && <p>Yeet</p> }</p>)}
                </Context.Consumer>
                <div id="diagramControls">
                    <button onClick={ () =>  { this.saveDiagram() } }>
                        Save
                    </button>
                    <button onClick={ () =>  { this.loadDiagram() } }>
                        Load
                    </button>
                    <button onClick={ () =>  { this.newDiagram() } }>
                        New
                    </button>
                </div>
                <div id="properties-panel" className="center"/>
            </div>
        );
    }
}

export default BpmnModelerComponent;