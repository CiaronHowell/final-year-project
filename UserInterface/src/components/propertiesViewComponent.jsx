import React from 'react';

import ElementPropertiesComponent from './elementPropertiesComponent';

class PropertiesViewComponent extends React.Component {
    constructor(props) {
        super(props);

        // We will use these variables to keep track of whether multiple
        // elements have been selected and which element has been selected
        this.state = {
            selectedElements: [],
            element: null
        };
    }

    componentDidMount() {
        const {
            modeler
        } = this.props;

        modeler.on('selection.changed', (e) => {
            this.setState({
                selectedElements: e.newSelection,
                element: e.newSelection[0]
            });
        });

        modeler.on('element.changed', (e) => {
            const {
                element: currentElement
            } = this.state;

            if (!currentElement) return;

            if (e.element.id === currentElement.id) {
                this.setState({
                    element: e.element
                });
            }
        });
    }

    render() {
        const {
            modeler
        } = this.props;
    
        const {
            selectedElements,
            element
        } = this.state;

        if (selectedElements.length === 1) {
            this.props.container.style.visibility = "visible";

            return <ElementPropertiesComponent modeler={ modeler } element={ element } />;
        }

        this.props.container.style.visibility = "hidden";
        return <span></span>;
    }
}

export default PropertiesViewComponent;