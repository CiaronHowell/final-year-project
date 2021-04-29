import React from 'react';

import { Context } from '../components/contextComponent';

class WorkflowControlsComponent extends React.Component {
    // Setting the context of the component to the shared context
    // so we can share data with other components
    static contextType = Context;

    componentDidMount() {
        this.addWindowMessageEventHandlers();
    }

    addWindowMessageEventHandlers() {
        // Event listener for play messages
        window.external.receiveMessage((message) => {
            var command = message.split("!,!");

            if (command[0] !== "playReply") return;

            if (command[1] === "success") {
                this.context.setRunning(true);
                this.context.setPaused(false);
                // Make stop button available
                this.context.setDisableStop(false);
            }
        });

        // Event listener for pause messages
        window.external.receiveMessage((message) => {
            var command = message.split("!,!");

            if (command[0] !== "pauseReply") return;

            if (command[1] === "success") {
                this.context.setRunning(false);
                this.context.setPaused(true);

            }
        });

        // Event listener for stop messages
        window.external.receiveMessage((message) => {
            var command = message.split("!,!");

            if (command[0] !== "stopReply") return;

            if (command[1] === "success") {
                this.context.setRunning(false);
                this.context.setPaused(false);
                this.context.setDisableStop(true);
            }
        });
    }

    playWorkflow() {
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
                <button 
                    id="playButton" 
                    onClick={() => { this.playWorkflow() }} 
                    disabled={this.context.state.running}
                >
                    Play
                </button>
                <button 
                    id="pauseButton" 
                    onClick={() => { this.pauseWorkflow() }} 
                    disabled={!this.context.state.running || this.context.state.paused}
                >
                    Pause
                </button>
                <button 
                    id="stopButton" 
                    onClick={() => { this.stopWorkflow() }} 
                    disabled={this.context.state.disableStop}
                >
                    Stop
                </button>
            </div>
        );
    }
}

export default WorkflowControlsComponent;