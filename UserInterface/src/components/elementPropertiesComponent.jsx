import { useEffect } from 'react';

import { is, getBusinessObject } from 'bpmn-js/lib/util/ModelUtil'

// I HAVE USED THE MATERIAL-UI LIBRARY SPECIFICALLY FOR THE COMBOBOX
import TextField from '@material-ui/core/TextField'
import Autocomplete from '@material-ui/lab/Autocomplete';

function ElementPropertiesComponent(props) {
    console.log('Element updated')

    let {
        element,
        modeler,
        moduleInfo
    } = props;

    const moduleNames = Object.keys(moduleInfo);

    if (element.labelTarget) {
        element = element.labelTarget;
    }

    useEffect(() => {
        // Update the data
        if (is(element, 'bpmn:ServiceTask') && element.businessObject.name) {
            methodSelected(element.businessObject.name);
            console.log('hit useEffect');
        }
    })
  
    function updateName(name) {
        const modeling = modeler.get('modeling');
  
        console.log('changing name');
        modeling.updateLabel(element, name);
    }

    // Update parameters button will only be available if the 
    // method requires parameters so we don't need to handle not having 
    // any parameters
    async function updateParameters() {
        console.log('hit in updateParameters')
        const moddle = modeler.get('moddle');

        // Gets the business object so we can access the 
        const businessObject = getBusinessObject(element);

        // Retrieves the extension elements from the business object or creates a blank extensionElement 
        // to append to the element
        const extensionElements = businessObject.extensionElements || moddle.create('bpmn:ExtensionElements');
        // Gets the parameters fieldset so we can access the input elements
        const parentElement = document.getElementById('parameters');
        // Gets all input elements from the parameters fieldset
        const inputElements = parentElement.getElementsByTagName('input');
        // Gets the parameters the task needs
        const { Parameters } = moduleInfo[businessObject.name];
        // If the parameters have already been setup then we just needd to edit them
        if (extensionElements.values.length > 0) {
            for (const inputElement of inputElements) {
                editParameterValue(inputElement.name, inputElement.value)
            }
        
            // We've updated the parameters so we can just return out of this method
            return;
        }

        // If the extensionElement has no parameters then we need to create them
        for (const inputElement of inputElements) {
            const parameterName = inputElement.name;

            const method = moddle.create('method:parameter');
            method.name = parameterName;
            method.value = inputElement.value;
            method.type = Parameters[parameterName];
            extensionElements.get('values').push(method);
        }

        const modeling = modeler.get('modeling');
        modeling.updateProperties(element, {
            extensionElements
        });
    }

    function editParameterValue(parameterName, value) {
        const businessObject = getBusinessObject(element);

        console.log(businessObject);

        if (!businessObject.extensionElements) return;

        // Find the extension element with the correct parameter name then set the value
        const extensionElements = businessObject.extensionElements.values;
        for (const extensionElement of extensionElements) {
            if (extensionElement.name === parameterName) {
                extensionElement.value = value;
            }
        }
    }
  
    async function makeServiceTask() {
        const bpmnReplace = modeler.get('bpmnReplace');
  
        bpmnReplace.replaceElement(element, {
            type: 'bpmn:ServiceTask'
        });
    }

    function methodSelected(moduleName) {
        // Get the parameters from the module info 
        const { Parameters } = moduleInfo[moduleName];
        const parameterKeys = Object.keys(Parameters);
        // If there is no parameters then we will
        if (parameterKeys.length === 0) {
            // If the method doesn't have parameters then 
            //we should hide the parameter button fieldset
            parameterDisplay(false);
            console.log('hit in no entries');
            return;
        }

        // Remove all kids from fieldset element 
        removeExistingParameterElements();
        
        for (const parameterName of parameterKeys) {
            addParameterElement(parameterName, Parameters[parameterName]);
        }

        parameterDisplay(true);
    }

    function parameterDisplay(show) {
        const parameterElements = document.getElementsByClassName("parameterDisplay")
        for (const parameterElement of parameterElements) {
            parameterElement.style.visibility = show ? "visible" : "hidden";
        }
    }

    function removeExistingParameterElements() {
        const parentElement = document.getElementById('parameters');
        console.log(parentElement);

        while (parentElement.lastChild) {
            parentElement.removeChild(parentElement.lastChild);
        }
    }

    function addParameterElement(parameterName, parameterType) {
        const parentElement = document.getElementById('parameters');

        const nameLabel = document.createElement('label');
        nameLabel.innerText = parameterName;
        parentElement.appendChild(nameLabel);

        const parameterInput = document.createElement('input');
        parameterInput.name = parameterName;
        // Try to get prexisting value from element or give empty string
        console.log(getParameterValue(parameterName));
        parameterInput.value = getParameterValue(parameterName) || "";
        parentElement.appendChild(parameterInput);

        const typeLabel = document.createElement('label');
        typeLabel.innerText = parameterType;
        parentElement.appendChild(typeLabel);

        parentElement.appendChild(document.createElement('br'));
    }

    function getParameterValue(parameterName) {
        console.log(element);
        const businessObject = getBusinessObject(element);

        console.log(businessObject);

        if (!businessObject.extensionElements) return;

        // TODO: Return the value of the parameter name
        const extensionElements = businessObject.extensionElements.values;
        for (const extensionElement of extensionElements) {
            if (extensionElement.name === parameterName) {
                return extensionElement.value;
            }
        }
    }

    // If it's a task element then give the option of selecting a method
    if (is(element, 'bpmn:Task') && !is(element, 'bpmn:ServiceTask')) {
        console.log('is a task still');
        return (
            <div className="element-properties" key={ element.id }>
                <fieldset>
                    <label>id</label>
                    <span>{ element.id }</span>
                </fieldset>

                <fieldset>
                    <Autocomplete 
                        options={moduleNames}
                        onChange={(event, methodName) => {
                            updateName(methodName);
                            if (!is(element, 'bpmn:ServiceTask')) makeServiceTask();
                        }}
                        renderInput={(params) => <TextField {...params} label="Task"/>}
                    />
                </fieldset>
            </div>
        );
    }
    // If it's not a service task then just display the ID
    else if (!is(element, 'bpmn:ServiceTask')) {
        return (
            <div className="element-properties" key={ element.id }>
                <fieldset>
                    <label>id</label>
                    <span>{ element.id }</span>
                </fieldset>
            </div>
        );
    }

    return (
        <div className="element-properties" key={ element.id }>
            <fieldset>
                <label>id</label>
                <span>{ element.id }</span>
            </fieldset>

            <fieldset>
                <Autocomplete 
                    options={moduleNames}
                    onChange={(event, methodName) => {
                        updateName(methodName);
                        methodSelected(methodName)
                    }}
                    value={element.businessObject.name}
                    renderInput={(params) => <TextField {...params} label="Method"/>}
                />
            </fieldset>

            <fieldset id="parameters" className="parameterDisplay"/>

            <fieldset id="parameterButtonContainer" className="parameterDisplay">
                <button onClick={() => {
                    updateParameters()
                }}>
                    Save Parameters
                </button>
            </fieldset>
            
        </div>
    );
}

export default ElementPropertiesComponent;