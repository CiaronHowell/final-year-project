import React from 'react';

class LoggerDisplayComponent extends React.Component {

    componentDidMount() {
        // Add the event handlers only once
        // this.addNavButtonEventHandlers();
        this.addWindowMessageEventHandlers();
    }

    componentWillUnmount() {
        this.modeler.destroy();
    }

    render() {
        return (
            <div id="logger" className="react-bpmn-diagram-container" ref={ this.containerRef }>
                {/* Put info here */}
            </div>
        );
    }
}

export default LoggerDisplayComponent;