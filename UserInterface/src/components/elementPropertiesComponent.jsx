// I HAVE USED THE MATERIAL-UI LIBRARY SPECIFICALLY FOR THE COMBOBOX
import TextField from '@material-ui/core/TextField'
import Autocomplete from '@material-ui/lab/Autocomplete';

import { is, getBusinessObject } from 'bpmn-js/lib/util/ModelUtil'

export default function ElementPropertiesComponent(props) {
    let {
        element,
        modeler,
        moduleInfo
    } = props;

    const moduleNames = Object.keys(moduleInfo);

    if (element.labelTarget) {
        element = element.labelTarget;
    }
  
    function updateName(name) {
        const modeling = modeler.get('modeling');
  
        modeling.updateLabel(element, name);
    }

    // Update parameters button will only be available if the 
    // method requires parameters so we don't need to handle not having 
    // any parameters
    async function updateParameters() {
        const moddle = modeler.get('moddle');

        // Gets the business object so we can access the 
        const businessObject = getBusinessObject(element);

        // Retrieves the extension elements from the business object or creates a blank extensionElement 
        // to append to the element
        const extensionElements = businessObject.extensionElements || moddle.create('bpmn:ExtensionElements');

        const { Parameters } = moduleInfo[businessObject.name];

        const parentElement = document.getElementById('parameters');
        const inputElements = parentElement.getElementsByTagName('input');
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

        const { xml } = await modeler.saveXML({ format: true });
        console.log(xml)
    }
  
    async function makeServiceTask() {
        const bpmnReplace = modeler.get('bpmnReplace');
  
        bpmnReplace.replaceElement(element, {
            type: 'bpmn:ServiceTask'
        });
    }

    function methodSelected(moduleName) {
        // TODO: Remove all kids from element 
        removeExistingParameterElements();

        // {"Parameters":{"enterTheNumber":"System.Int32","enterName":"System.String"}
        // TODO: Append an input for each parameter to the fieldset, display label name and label type left and right

        const { Parameters } = moduleInfo[moduleName];
        console.log(Parameters);
        const parameterKeys = Object.keys(Parameters);

        if (parameterKeys === null) return;

        for (const parameterName of parameterKeys) {
            addParameterElement(parameterName, Parameters[parameterName])
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
        // valueInput.value = //TODO: Get prexisting value from element
        parentElement.appendChild(parameterInput);

        const typeLabel = document.createElement('label');
        typeLabel.innerText = parameterType;
        parentElement.appendChild(typeLabel);

        parentElement.appendChild(document.createElement('br'));
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
                            // methodSelected(methodName);
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

    // methodSelected(element.businessObject.name);

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
                        methodSelected(methodName);
                    }}
                    value={element.businessObject.name}
                    renderInput={(params) => <TextField {...params} label="Method"/>}
                />
            </fieldset>

            <fieldset id="parameters">

            </fieldset>

            <fieldset>
                <button onClick={() => {
                    updateParameters()
                }}>
                    Save Parameters
                </button>
            </fieldset>
        </div>
    );
}