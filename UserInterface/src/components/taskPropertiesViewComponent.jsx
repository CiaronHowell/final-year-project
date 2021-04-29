import React from 'react';

import TaskPropertiesComponent from './taskPropertiesComponent';

class TaskPropertiesViewComponent extends React.Component {
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
        // Get the diagram modeler from the properties
        const {
            modeler
        } = this.props;

        // Selection event handler
        modeler.on('selection.changed', (event) => {
            // Get the current element from the state
            const {
                element: currentElement
            } = this.state;

            // Return if there is no new selection
            if (event.newSelection.length === 0) return;

            // Return if the current element isn't null and 
            //the current element is the same as the new selection
            if (currentElement 
                && currentElement.id === event.newSelection[0].id) return;

            // Set the state to the new element
            this.setState({
                selectedElements: event.newSelection,
                element: event.newSelection[0]
            });
        });

        // Element changed handler
        modeler.on('element.changed', (event) => {
            const {
                element: currentElement
            } = this.state;

            // Don't do anything if the current element is null
            if (!currentElement) return;

            console.log(event.element);

            // Set the element if the element has changed type
            if (event.element.id === currentElement.id 
                && currentElement.type !== event.element.type) {
                console.log('hit inside element changed if');

                this.setState({
                    element: event.element
                });
            }
        });
    }

    render() {
        const {
            modeler,
            moduleInfo
        } = this.props;
    
        // Get the selectedElements and elements out of the current state
        const {
            selectedElements,
            element
        } = this.state;

        // If the selected element is one or more then set the task panel to visible
        if (selectedElements.length >= 1) {
            // Set the container to visible
            this.props.container.style.visibility = "visible";

            // Render the task properties component
            return <TaskPropertiesComponent modeler={ modeler } element={ element } moduleInfo={ moduleInfo } />;
        }

        this.props.container.style.visibility = "hidden";
        return <span></span>;
    }
}

export default TaskPropertiesViewComponent;