// I HAVE USED THE MATERIAL-UI LIBRARY SPECIFICALLY FOR THE COMBOBOX
import TextField from '@material-ui/core/TextField'
import Autocomplete from '@material-ui/lab/Autocomplete';

import { is, getBusinessObject } from 'bpmn-js/lib/util/ModelUtil'

export default function ElementPropertiesComponent(props) {
    let option = ["test", "test1", "test2", "test3" ];

    let {
      element,
      modeler
    } = props;

    if (element.labelTarget) {
        element = element.labelTarget;
    }
  
    function updateName(name) {
        const modeling = modeler.get('modeling');
  
        modeling.updateLabel(element, name);
    }
  
    async function updateParameter(parameterName, value, type) {

        const moddle = modeler.get('moddle');

        // Gets the business object so we can access the 
        const businessObject = getBusinessObject(element);

        // Retrieves the extension elements from the business object or creates a blank extensionElement 
        // to append to the element
        const extensionElements = businessObject.extensionElements || moddle.create('bpmn:ExtensionElements');


        const test = moddle.create('method:parameter');
        test.name = parameterName;
        test.value = value
        test.type = type;
        extensionElements.get('values').push(test);

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

      option = [ "yeetus", "mcbeetus"];
    }
  
    return (
      <div className="element-properties" key={ element.id }>
        <fieldset>
          <label>id</label>
          <span>{ element.id }</span>
        </fieldset>

        <fieldset>
            <Autocomplete 
                options={option}
                // onChange
                renderInput={(params) => <TextField {...params} label="Class"/>}
            />
            <Autocomplete 
                options={option}
                // hidden={}
                renderInput={(params) => <TextField {...params} label="Method"/>}
            />
        </fieldset>

        {
          is(element, 'bpmn:ServiceTask') &&
            <fieldset>
              <label>topic (custom)</label>
              <input value={ element.businessObject.get('method:parameter') } onChange={ (event) => {
                updateParameter(event.target.value, "test", "String")
              } } />
            </fieldset>
        }

        <fieldset>
          <label>actions</label>
  
          {
            is(element, 'bpmn:Task') && !is(element, 'bpmn:ServiceTask') &&
              <button onClick={ makeServiceTask }>Make Service Task</button>
          }
        </fieldset>
      </div>
    );
  }