import React from 'react';

class FlowchartControlsComponent extends React.Component {

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

        window.external.receiveMessage((message) => {
            var command = message.split(",");

            if (command[0] !== "test") return;

            alert(command[1]);
        });
    }

    callDotNet() {
        window.external.sendMessage('test,blah😋');

        console.log('blashhhh');
    }

    render() {
        return (
            <div id="controls">
                <button id="playButton" onClick={this.callDotNet}>Play</button>
                <button id="pauseButton">Pause</button>
                <button id="stopButton">Stop</button>
            </div>
        );
    }
}

export default FlowchartControlsComponent;