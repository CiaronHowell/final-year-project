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
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Diagram|*.bpmn",
                FilterIndex = 1,
                Title = "Save a Diagram",
                RestoreDirectory = true
            };

            // Setting name just in case the user cancels the dialog
            name = "";

            DialogResult result = saveFileDialog.ShowDialog();
            // Open the file dialog to let user select where they want to save diagram
            if (result == DialogResult.OK)
            {
                Debug.WriteLine($"SaveDiagramTest: {saveFileDialog.FileName}");

                Stream stream;
                if ((stream = saveFileDialog.OpenFile()) != null)
                {
                    // Write XML string to file
                    using (StreamWriter writer = new(stream))
                    {
                        writer.Write(xml);
                    }

                    stream.Close();
                }

                name = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);

                // Update current diagram
                CurrentDiagram = xml;
            }

            cancelled = result == DialogResult.Cancel;
        }

        /// <summary>
        /// Gets diagram XML using File Dialog
        /// </summary>
        /// <returns>XML </returns>
        public string GetDiagramXML(out string name)
        {
            // Configure dialog
            OpenFileDialog openFileDialog = new()
            {
                //InitialDirectory = "" // TODO: Add home directory
                Filter = "Diagram|*.bpmn",
                FilterIndex = 1,
                Title = "Open a Diagram",
                RestoreDirectory = true
            };

            name = "";
            string diagram = string.Empty;
            // Open the file dialog to let user select the diagram they want to load
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream;
                if ((stream = openFileDialog.OpenFile()) != null)
                {
                    using (StreamReader reader = new(stream))
                    {
                        diagram = reader.ReadToEnd();
                    }

                    stream.Close();
                }

                if (string.IsNullOrWhiteSpace(diagram))
                    throw new Exception("Diagram is blank");

                name = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                Debug.WriteLine($"GetDiagramTest: Diagram - {diagram}");
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

        public Dictionary<string, Parameters> ParseCurrentDiagramXML()
        {
            XmlDocument diagram = new();
            // Parse string to XML
            diagram.LoadXml(CurrentDiagram);

            // Create namespace manager so we can use prefixes to search for elements
            XmlNamespaceManager nsManager = new(diagram.NameTable);
            nsManager.AddNamespace("bpmn", "http://www.omg.org/spec/BPMN/20100524/MODEL");
            nsManager.AddNamespace("method", "http://Method");

            int numberOfElements = diagram.DocumentElement.SelectSingleNode("//bpmn:process", nsManager).ChildNodes.Count;
            int numberOfArrowElements = diagram.DocumentElement.SelectNodes("//bpmn:sequenceFlow", nsManager).Count;

            int feasibleElements = numberOfElements - numberOfArrowElements;

            // Get start event by searching through the XML document for the element.
            XmlNode startElement = diagram.DocumentElement.SelectSingleNode("//bpmn:startEvent", nsManager);
            string outgoing = startElement.SelectSingleNode("bpmn:outgoing", nsManager).InnerText;

            Dictionary<string, Parameters> workflow = new();
            for (int i=0; i < feasibleElements; i++)
            {
                XmlNode currentElement = GetNextElement(diagram, nsManager, outgoing);
                outgoing = startElement.SelectSingleNode("bpmn:outgoing", nsManager).InnerText;

                if (string.IsNullOrWhiteSpace(outgoing)) break;

                Debug.WriteLine(currentElement.Name);
                // We only use serviceTasks to run methods
                if (currentElement.Name != "bpmn:serviceTask") continue;
                
                XmlNode extensionElement = currentElement.SelectSingleNode("./bpmn:extensionElements", nsManager);

                Dictionary<string, ParameterDetails> parameterList = new();
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

                // store name
                string methodName = currentElement.Attributes["name"].Value;
                Debug.WriteLine(methodName);
                workflow.Add(methodName, new Parameters(parameterList));
            }

            return workflow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagram"></param>
        /// <param name="nsManager"></param>
        /// <param name="outgoingId"></param>
        /// <param name="nextOutgoingId"></param>
        /// <returns></returns>
        private XmlNode GetNextElement(
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