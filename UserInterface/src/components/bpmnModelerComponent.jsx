import React from 'react';
import BpmnModeler from 'bpmn-js/lib/Modeler';

// Imports the moddle json file 
import parameterModdleExtension from '../moddle/parameters';

import 'bpmn-js/dist/assets/diagram-js.css';
import 'bpmn-js/dist/assets/bpmn-font/css/bpmn.css';
import '../css/diagram.css';

import PropertiesPanel from '../scripts/propertiesPanel';

import { Context } from '../components/contextComponent';
import { is } from 'bpmn-js/lib/util/ModelUtil';

class BpmnModelerComponent extends React.Component {
    static contextType = Context;

    constructor() {
        super();

        this.containerRef = React.createRef();
        this.dirtyFlag = false;
    }

    componentDidMount() {
        // Send initialise flag to backend to get classes and methods
        this.initialiseBPMN()

        // Add the event handlers only once
        // this.addNavButtonEventHandlers();
        this.addWindowMessageEventHandlers();
    } 

    componentDidUpdate(){
        // If the component updated then we need to check if it was due to the context values being changed
        console.log(this.context.state.diagramName)
        console.log(this.context.state.running)
        // If we are about to start a run and the user has made some edits,
        // we want the user to make sure they've saved 
        if (this.context.state.running && this.dirtyFlag) {
            this.saveDiagram()
        }
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

        this.modeler.on('element.changed', (e) => {
            this.dirtyFlag = true;
        });

        await this.modeler.createDiagram();

        // access modeler components
        const canvas = this.modeler.get('canvas');

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
            const command = message.split(/!,!(.+)/);

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
            const command = message.split("!,!");

            // If the message isn't the command that we want then we just 
            //  return nothing
            if (command[0] !== "loadDiagramFunc") return;

            console.log(command[1]);

            // Display diagram
            await this.modeler.importXML(command[1])

            if (command[2] !== "") {
                // Set the name of the diagram
                this.context.setDiagramName(command[2]);
            }
        });

        // Highlight the current task
        window.external.receiveMessage((message) => {
            const command = message.split("!,!");

            if (command[0] !== "currentTask") return;

            const modeling = this.modeler.get('modeling');
            const elementRegistry = this.modeler.get('elementRegistry');

            this.revertColorForTasks(elementRegistry, modeling);

            const elementId = command[1];

            if (!elementId) return;
            
            const element = elementRegistry.get(elementId);
            modeling.setColor(element, {
                stroke: 'green'
            });
        })

        // Highlight the current task
        window.external.receiveMessage((message) => {
            const command = message.split("!,!");

            if (command[0] !== "saveDiagramReply") return;

            if (command[1] === "success") {
                this.context.setDiagramName(command[2]);
            }

            console.log(command[1]);
        })
    }

    revertColorForTasks(elementRegistry, modeling) {
        elementRegistry.forEach(element => {
            modeling.setColor(element, {
                stroke: 'black'
            });
        });
    }

    async saveDiagram() {
        console.log("Attempting to save");

        // Save the current diagram
        const { xml } = await this.modeler.saveXML({ format: true });
        console.log(xml)

        // Send the command plus the diagram in XML form
        window.external.sendMessage("saveFunc!,!".concat(xml));

        this.dirtyFlag = false;
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

        window.external.sendMessage("newDiagramFunc");

        // Clear the diagram name
        this.context.setDiagramName("");
    }

    render() {
        return (
            <div id="canvas" className="react-bpmn-diagram-container" ref={ this.containerRef }>
                <label id="diagramName">{ this.context.state.diagramName }</label>
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