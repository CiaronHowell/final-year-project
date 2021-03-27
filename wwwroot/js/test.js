// import BpmnModeler from 'bpmn-js/lib/Modeler';

// var bpmnJS = new BpmnModeler({
//   additionalModules: [
//     resizeAllModule,
//     colorPickerModule,
//     nyanDrawModule,
//     nyanPaletteModule
//   ]
// });

var bpmnJS = new BpmnJS({
    container: '#canvas'
});

async function openDiagram(bpmnXML) {

    // import diagram
    try {

        await bpmnJS.importXML(bpmnXML);

        // access modeler components
        var canvas = bpmnJS.get('canvas');
        var overlays = bpmnJS.get('overlays');


        // zoom to fit full viewport
        canvas.zoom('fit-viewport');

        // attach an overlay to a node
        overlays.add('SCAN_OK', 'note', {
            position: {
                bottom: 0,
                right: 0
            },
            html: '<div class="diagram-note">Mixed up the labels?</div>'
        });

        // add marker
        canvas.addMarker('SCAN_OK', 'needs-discussion');
    } catch (err) {

        console.error('could not import BPMN 2.0 diagram', err);
    }
}

$.get("https://cdn.staticaly.com/gh/bpmn-io/bpmn-js-examples/dfceecba/starter/diagram.bpmn", openDiagram, 'text');