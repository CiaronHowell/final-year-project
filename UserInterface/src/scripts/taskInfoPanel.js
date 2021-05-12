import React from 'react';
import ReactDOM from 'react-dom';

import TaskPropertiesViewComponent from '../components/taskPropertiesViewComponent';

class TaskInfoPanel {

    constructor(parameters) {
        // Split the parameters into the variables that have been passed down
        const {
            modeler,
            container,
            moduleInfo
        } = parameters;

        // Render the task properties view and pass the modeler, 
        //container and module info via the properties of the component
        ReactDOM.render(
            <TaskPropertiesViewComponent 
                modeler={ modeler } 
                container={ container } 
                moduleInfo={ moduleInfo } 
            />,
            container
        );
    }
}

export default TaskInfoPanel;
