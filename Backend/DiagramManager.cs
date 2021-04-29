using FinalYearProject.Backend.Utils;
using FinalYearProject.Backend.Utils.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace FinalYearProject.Backend
{
    public class DiagramManager
    {
        /// <summary>
        /// Stores current diagram ready to be converted when 
        /// workflow is started
        /// </summary>
        private string CurrentDiagram { get; set; }

        /// <summary>
        /// Save diagram in directory selected in File Dialog
        /// </summary>
        /// <param name="xml"></param>
        public void SaveDiagram(string xml, out string name, out bool cancelled)
        {
            // Configure dialog
            SaveFileDialog saveDiagramDialog = new()
            {
                Title = "Save a Diagram",
                Filter = "Diagram|*.bpmn",
                InitialDirectory = AppDirectories.APP_DIRECTORY
            };

            // Setting name just in case the user cancels the dialog
            name = "";

            DialogResult result = saveDiagramDialog.ShowDialog();
            // Open the file dialog to let user select where they want to save diagram
            if (result == DialogResult.OK)
            {
                Debug.WriteLine($"SaveDiagramTest: {saveDiagramDialog.FileName}");

                Stream diagramStream = saveDiagramDialog.OpenFile();
                if (diagramStream != null)
                {
                    // Write XML string to file
                    using (StreamWriter writer = new(diagramStream))
                    {
                        writer.Write(xml);
                    }

                    diagramStream.Close();
                }

                name = Path.GetFileNameWithoutExtension(saveDiagramDialog.FileName);

                // Update current diagram
                CurrentDiagram = xml;
            }

            // Set whether user cancelled the dialog box
            cancelled = result == DialogResult.Cancel;
        }

        /// <summary>
        /// Gets diagram XML using File Dialog
        /// </summary>
        /// <returns>XML </returns>
        public string GetDiagramXML(out string name)
        {
            // Configure dialog
            OpenFileDialog openDiagramDialog = new()
            {
                Title = "Open a Diagram",
                Filter = "Diagram|*.bpmn",
                InitialDirectory = AppDirectories.APP_DIRECTORY                
            };

            name = "";
            string diagram = string.Empty;
            // Open the file dialog to let user select the diagram they want to load
            if (openDiagramDialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = openDiagramDialog.OpenFile();
                if (stream != null)
                {
                    using (StreamReader reader = new(stream))
                    {
                        diagram = reader.ReadToEnd();
                    }

                    stream.Close();
                }

                if (string.IsNullOrWhiteSpace(diagram))
                    throw new Exception("Diagram is blank");

                name = Path.GetFileNameWithoutExtension(openDiagramDialog.FileName);
            }

            // Update current diagram
            CurrentDiagram = diagram;

            return diagram;
        }

        /// <summary>
        /// Clears the "cached" diagram
        /// </summary>
        public void ClearDiagram()
        {
            CurrentDiagram = "";
        }

        /// <summary>
        /// Parses the current diagram from XML to List
        /// </summary>
        /// <returns>Returns the workflow</returns>
        public List<WorkflowMethod> ParseCurrentDiagramXML()
        {
            XmlDocument diagram = new();
            // Parse string to XML
            diagram.LoadXml(CurrentDiagram);

            // Create namespace manager so we can use prefixes to search for elements
            XmlNamespaceManager nsManager = new(diagram.NameTable);
            nsManager.AddNamespace("bpmn", "http://www.omg.org/spec/BPMN/20100524/MODEL");
            nsManager.AddNamespace("method", "http://Method");

            // Gets the total number of elements in the diagram
            int numberOfElements = diagram.DocumentElement.SelectSingleNode("//bpmn:process", nsManager).ChildNodes.Count;
            // Gets the total number of arrows in the diagram
            int numberOfArrowElements = diagram.DocumentElement.SelectNodes("//bpmn:sequenceFlow", nsManager).Count;

            // Removing the arrow elements gives us all of the feasible elements
            int feasibleElements = numberOfElements - numberOfArrowElements;

            // Get start event by searching through the XML document for the element.
            XmlNode startElement = diagram.DocumentElement.SelectSingleNode("//bpmn:startEvent", nsManager);
            // Get the id of the next element that the start element is connected to
            string nextElementId = startElement.SelectSingleNode("bpmn:outgoing", nsManager).InnerText;

            List<WorkflowMethod> workflow = new();
            // Loop through feasible elements
            for (int i=0; i < feasibleElements; i++)
            {
                XmlNode currentElement = GetNextElement(diagram, nsManager, nextElementId);
                // If the inner text is null, assume that we are at the end of the workflow and leave it blank
                //so it breaks at the next if statement
                nextElementId = currentElement.SelectSingleNode("bpmn:outgoing", nsManager)?.InnerText;

                if (string.IsNullOrWhiteSpace(nextElementId)) break;

                Debug.WriteLine(currentElement.Name);
                // We only use serviceTasks to run methods
                if (currentElement.Name != "bpmn:serviceTask") continue;
                
                // The extension element contains the parameter info; name, type and value
                XmlNode extensionElement = currentElement.SelectSingleNode("./bpmn:extensionElements", nsManager);

                Dictionary<string, ParameterDetails> parameterList = new();
                // If the extension element isn't null then we can get all of the parameter info
                if (extensionElement != null)
                {
                    foreach (XmlNode parameterElement in extensionElement.ChildNodes)
                    {
                        parameterList.Add(
                            parameterElement.Attributes["name"].Value,
                            new ParameterDetails(
                                parameterElement.Attributes["value"].Value,
                                parameterElement.Attributes["type"].Value
                            )
                        );
                    }
                }

                string methodId = currentElement.Attributes["id"].Value;
                string methodName = currentElement.Attributes["name"].Value;

                workflow.Add(
                    new WorkflowMethod(
                        methodId,
                        methodName, 
                        new Parameters(parameterList))
                    );

                Debug.WriteLine($"{methodId} {methodName}");
            }

            return workflow;
        }

        /// <summary>
        /// Gets the next element 
        /// </summary>
        /// <param name="diagram"></param>
        /// <param name="nsManager"></param>
        /// <param name="outgoingId"></param>
        /// <param name="nextOutgoingId"></param>
        /// <returns>Returns the element connected to the given element</returns>
        private static XmlNode GetNextElement(
            XmlDocument diagram, 
            XmlNamespaceManager nsManager, 
            string outgoingId)
        {
            XmlNode arrowElement = 
                diagram.DocumentElement.SelectSingleNode($"//bpmn:sequenceFlow[@id='{outgoingId}']", nsManager);

            // Gets the element that the arrow is pointing at
            outgoingId = arrowElement.Attributes["targetRef"].Value;
            
            return diagram.DocumentElement.SelectSingleNode($"//*[@id='{outgoingId}']", nsManager);
        }
    }
}