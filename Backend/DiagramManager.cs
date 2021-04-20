using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace FinalYearProject.Backend
{
    public class DiagramManager
    {

        /// <summary>
        /// Save diagram in directory selected in File Dialog
        /// </summary>
        /// <param name="xml"></param>
        public static void SaveDiagram(string xml, out bool cancelled)
        {
            // Configure dialog
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Diagram|*.bpmn",
                FilterIndex = 1,
                Title = "Save a Diagram",
                RestoreDirectory = true
            };

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
            }

            cancelled = result == DialogResult.Cancel;
        }

        /// <summary>
        /// Gets diagram XML using File Dialog
        /// </summary>
        /// <returns>XML </returns>
        public static string GetDiagramXML()
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

                Debug.WriteLine($"GetDiagramTest: Diagram - {diagram}");
            }

            return diagram;
        }

        //TODO: Might need to add a newDiagram method to keep everything in check
    }
}