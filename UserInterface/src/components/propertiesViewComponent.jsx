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
            // Get the current element from the state
            const {
                element: currentElement
            } = this.state;

            // Return if there is no new selection
            if (e.newSelection.length === 0) return;

            console.log(currentElement);

            // Return if the current element isn't null and 
            //the current element is the same as the new selection
            if (currentElement 
                && currentElement.id === e.newSelection[0].id) return;

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

            console.log(e.element);

            // Set the element if the element has changed type
            if (e.element.id === currentElement.id 
                && currentElement.type !== e.element.type) {
                console.log('hit inside element changed if');

                this.setState({
                    element: e.element
                });
            }
        });
    }

    render() {
        const {
            modeler,
            moduleInfo
        } = this.props;
    
        const {
            selectedElements,
            element
        } = this.state;

        if (selectedElements.length === 1) {
            this.props.container.style.visibility = "visible";

            return <ElementPropertiesComponent modeler={ modeler } element={ element } moduleInfo={ moduleInfo } />;
        }

        this.props.container.style.visibility = "hidden";
        return <span></span>;
    }
}

export default PropertiesViewComponent;