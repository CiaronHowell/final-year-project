import React from 'react';
import ReactDOM from 'react-dom';

import TaskPropertiesViewComponent from '../components/taskPropertiesViewComponent';

class TaskInfoPanel {

    constructor(parameters) {
        const {
            modeler,
            container,
            moduleInfo
        } = parameters;

        // Render the task properties view 
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
