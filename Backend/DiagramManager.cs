using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

public class DiagramManager 
{

    /// <summary>
    /// Save diagram in directory selected in File Dialog
    /// </summary>
    /// <param name="xml"></param>
    public static void SaveDiagram(string xml) 
    {
        // Configure dialog
        SaveFileDialog saveFileDialog = new()
        {
            Filter = "Diagram|*.bpmn",
            FilterIndex = 1,
            Title = "Save a Diagram",
            RestoreDirectory = true
        };

        // Open the file dialog to let user select where they want to save diagram
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
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
    }

    /// <summary>
    /// Gets diagram XML using File Dialog
    /// </summary>
    /// <returns>XML </returns>
    public static string GetDiagramXML() 
    {
        // Configure dialog
        OpenFileDialog openFileDialog = new OpenFileDialog
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

            Debug.WriteLine($"GetDiagramTest: Diagram - {diagram}");
        }

        return diagram;
    }

    //TODO: Might need to add a newDiagram method to keep everything in check
}