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
    }

    componentWillUnmount() {
        this.modeler.destroy();
    }

    render() {
        return (
          <div id="canvas" className="react-bpmn-diagram-container" ref={ this.containerRef }></div>
        );
    }
}

export default BpmnModelerComponent;