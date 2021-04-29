using System;
using System.Collections.Generic;
using System.Diagnostics;
using FinalYearProject.Backend;
using FinalYearProject.Backend.Utils;
using PhotinoNET;

namespace FinalYearProject
{
    class Program
    {
        /// <summary>
        /// Diagram Manager
        /// </summary>
        private static DiagramManager DiagramManager { get; set; }

        /// <summary>
        /// QueueManager
        /// </summary>
        private static QueueManager QueueManager { get; set; }

        /// <summary>
        /// Module Manager
        /// </summary>
        private static ModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Unique string to split up parameters sent to GUI
        /// </summary>
        private const string UNIQUE_SPLIT_STRING = "!,!";

        #region Backend Commands
        /// <summary>
        /// Constant string for "open diagram" function
        /// </summary>
        private const string OPEN_DIAGRAM_FUNCTION = "openFunc";

        /// <summary>
        /// Constant string for "save diagram" function
        /// </summary>
        private const string SAVE_DIAGRAM_FUNCTION = "saveFunc";

        /// <summary>
        /// Constant string for "new diagram" function
        /// </summary>
        private const string NEW_DIAGRAM_FUNCTION = "newDiagramFunc";

        /// <summary>
        /// Constant string for "play workflow" function
        /// </summary>
        private const string PLAY_WORKFLOW_FUNCTION = "playWorkflowFunc";

        /// <summary>
        /// Constant string for "pause workflow" function
        /// </summary>
        private const string PAUSE_WORKFLOW_FUNCTION = "pauseWorkflowFunc";

        /// <summary>
        /// Constant string for "stop workflow" function
        /// </summary>
        private const string STOP_WORKFLOW_FUNCTION = "stopWorkflowFunc";

        /// <summary>
        /// Constant string for "load module info" function
        /// </summary>
        private const string LOAD_MODULE_INFO_FUNCTION = "loadModuleInfoFunc";

        /// <summary>
        /// Constant string for "test" message
        /// </summary>
        private const string TEST = "test";
        #endregion

        #region GUI Commands
        /// <summary>
        /// Constant string for GUI "load diagram" function
        /// </summary>
        private const string GUI_LOAD_DIAGRAM_FUNCTION = "loadDiagramFunc";

        /// <summary>
        /// Constant string for GUI "save diagram" reply
        /// </summary>
        private const string GUI_SAVE_DIAGRAM_REPLY = "saveDiagramReply";

        /// <summary>
        /// Constant string for GUI "load module info" reply
        /// </summary>
        private const string GUI_LOAD_MODULE_INFO_REPLY = "loadModuleInfoReply";
        #endregion

        [STAThread]
        static void Main(string[] args)
        {
            // Create the container window and load the index page
            PhotinoWindow window = new PhotinoWindow(
                "Final Year Project", 
                (options) => 
                {
                    // This event handler will handle messages sent from the GUI
                    options.WebMessageReceivedHandler += MessageHandler; 
                })
                .Center()
                .Load("wwwroot/index.html");

            // Need to instantiate all managers before we wait for the window to close
            ModuleManager = new();
            QueueManager = new(ModuleManager);
            DiagramManager = new();

            // Allow other classes to send messages to the GUI
            Window.CurrentWindow = window;

            window.WaitForClose();
        }

        /// <summary>
        /// Message Handler for messages from GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private static void MessageHandler(object sender, string message) 
        {
            // Get the current window
            var window = (PhotinoWindow)sender;

            // Split the message into the command and args
            string[] command = message.Split(UNIQUE_SPLIT_STRING, 2);
            switch (command[0])
            {
                case OPEN_DIAGRAM_FUNCTION:
                    try
                    {
                        // Retrieve diagram
                        string diagramXML = DiagramManager.GetDiagramXML(out string diagramName);

                        // Send diagram XML to the GUI
                        window.SendWebMessage($"{GUI_LOAD_DIAGRAM_FUNCTION}{UNIQUE_SPLIT_STRING}{diagramXML}{UNIQUE_SPLIT_STRING}{diagramName}");
                        Debug.WriteLine(diagramXML);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error when trying to open diagram: {ex}");

                        window.OpenAlertWindow("Loading Diagram", "Failed to load diagram.");
                    }
                    break;
                    
                case SAVE_DIAGRAM_FUNCTION:
                    try
                    {
                        string diagramXML = command[1];
                        // Saving diagram xml where the user decides
                        DiagramManager.SaveDiagram(diagramXML, out string name, out bool cancelled);

                        if (!cancelled)
                        {
                            window.OpenAlertWindow("Saving Diagram", "Saved diagram successfully.");
                        }

                        // Send back whether the xml was saved or cancelled and what the name is (if not cancelled)
                        window.SendWebMessage($"{GUI_SAVE_DIAGRAM_REPLY}{UNIQUE_SPLIT_STRING}{(cancelled ? "cancelled" : "success")}{UNIQUE_SPLIT_STRING}{name}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error when trying to save diagram: {ex}");

                        window.OpenAlertWindow("Saving Diagram", "Failed to save diagram.");
                    }
                    break;

                case NEW_DIAGRAM_FUNCTION:
                    // Just clear the "cached" diagram
                    DiagramManager.ClearDiagram();

                    // Don't need to send an alert as the user should see that the diagram has been set
                    // to a new diagram
                    break;

                case PLAY_WORKFLOW_FUNCTION:
                    try
                    {
                        if (!QueueManager.HasQueue)
                        {
                            QueueManager.LoadQueue(DiagramManager.ParseCurrentDiagramXML());
                        }

                        QueueManager.StartQueue();

                        window.SendWebMessage($"playReply{UNIQUE_SPLIT_STRING}success");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);

                        window.OpenAlertWindow("Pausing Workflow", "Failed to pause workflow.");
                        window.SendWebMessage($"playReply{UNIQUE_SPLIT_STRING}failed");
                    }
                    break;

                case PAUSE_WORKFLOW_FUNCTION:
                    try
                    {
                        QueueManager.PauseQueue();

                        window.SendWebMessage($"pauseReply{UNIQUE_SPLIT_STRING}success");
                        window.OpenAlertWindow("Pausing Workflow", "Paused workflow successfully.");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);

                        window.SendWebMessage($"pauseReply{UNIQUE_SPLIT_STRING}failed");
                        window.OpenAlertWindow("Pausing Workflow", "Failed to pause workflow.");
                    }
                    break;

                case STOP_WORKFLOW_FUNCTION:
                    try
                    {
                        QueueManager.StopQueue();

                        window.SendWebMessage($"stopReply{UNIQUE_SPLIT_STRING}success");
                        window.OpenAlertWindow("Stopping Workflow", "Stopped workflow successfully.");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);

                        window.SendWebMessage($"stopReply{UNIQUE_SPLIT_STRING}failed");
                        window.OpenAlertWindow("Stopping Workflow", "Failed to stop workflow.");
                    }
                    break;

                case LOAD_MODULE_INFO_FUNCTION:
                    // Get module info such as methods and their parameters
                    string moduleInfo = ModuleManager.ModuleInfoAsJSONString();
                    
                    Debug.WriteLine(moduleInfo);

                    window.SendWebMessage($"{GUI_LOAD_MODULE_INFO_REPLY}{UNIQUE_SPLIT_STRING}{moduleInfo}");
                    break;

                case TEST:
                    window.SendWebMessage(message);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Received an unknown command");
            }

        }
    }
}
