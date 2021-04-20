import ReactDOM from 'react-dom';
import React from 'react';

import PropertiesViewComponent from '../components/propertiesViewComponent';


export default class PropertiesPanel {

  constructor(options) {

    const {
      modeler,
      container,
      moduleInfo
    } = options;

    ReactDOM.render(
      <PropertiesViewComponent modeler={ modeler } container={ container } moduleInfo={ moduleInfo } />,
      container
    );
  }
}


