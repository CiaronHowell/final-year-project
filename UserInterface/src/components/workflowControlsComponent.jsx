import React from 'react';

import { Context } from '../components/contextComponent';

class WorkflowControlsComponent extends React.Component {
    // Setting the context of the component to the shared context
    // so we can share data with other components
    static contextType = Context;

    constructor() {
        super();

        // Make sure that sendMessage and receiveMessage exist
        // when the frontend is started without the Photino context.
        // I.e. using React's `npm run start` command and hot reload.
        if (typeof(window.external.sendMessage) !== 'function') {
            window.external.sendMessage = (message) => console.log("Emulating sendMessage.\nMessage sent: " + message);
        }

        if (typeof(window.external.receiveMessage) !== 'function') {
            window.external.receiveMessage = (delegate) => {
                let message = 'Simulating message from backend.';
                delegate(message);
            };

            window.external.receiveMessage((message) => console.log("Emulating receiveMessage.\nMessage received: " + message));
        }
    }

    componentDidMount() {
        this.addWindowMessageEventHandlers();
    }

    addWindowMessageEventHandlers() {
        // Event listener for play messages
        window.external.receiveMessage((message) => {
            var command = message.split(",");

            if (command[0] !== "playReply") return;

            // Do stuff for play
            if (command[1] === "success") {
                this.context.setRunning(false);

            } else {
                // Disable play button and enable pause button
            }
        });

        // Event listener for pause messages
        window.external.receiveMessage((message) => {
            var command = message.split(",");

            if (command[0] !== "pauseReply") return;

            // Do stuff for play
        });

        // Event listener for stop messages
        window.external.receiveMessage((message) => {
            var command = message.split(",");

            if (command[0] !== "stopReply") return;

            // Do stuff for play if successful

            // Disable editing until completion or 
        });
    }

    playWorkflow() {
        this.context.setRunning(true);

        window.external.sendMessage('playWorkflowFunc');
    }

    pauseWorkflow() {
        window.external.sendMessage('pauseWorkflowFunc');
    }

    stopWorkflow() {
        window.external.sendMessage('stopWorkflowFunc');
    }

    render() {
        console.log(this.context.state.running);

        return (
            <div id="workflowControls">
                <button id="playButton" onClick={() => { this.playWorkflow() }}>
                    Play
                </button>
                <button id="pauseButton" onClick={() => { this.pauseWorkflow() }}>
                    Pause
                </button>
                <button id="stopButton" onClick={() => { this.stopWorkflow() }}>
                    Stop
                </button>
            </div>
        );
    }
}

export default WorkflowControlsComponent;