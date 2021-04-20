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

    function updateParameters() {
        
    }
  
    async function updateParameter(parameterName, value, type) {

        const moddle = modeler.get('moddle');

        // Gets the business object so we can access the 
        const businessObject = getBusinessObject(element);

        // Retrieves the extension elements from the business object or creates a blank extensionElement 
        // to append to the element
        const extensionElements = businessObject.extensionElements || moddle.create('bpmn:ExtensionElements');

        const method = moddle.create('method:parameter');
        method.name = parameterName;
        method.value = value
        method.type = type;
        extensionElements.get('values').push(method);

        const modeling = modeler.get('modeling');
        modeling.updateProperties(element, {
            extensionElements
        });

        console.log(extensionElements);
        console.log(test);

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
        const info = moduleInfo[moduleName];
        console.log(info);
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
                            methodSelected(methodName);
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