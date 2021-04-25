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

        [STAThread]
        static void Main(string[] args)
        {
            // Window title declared here for visibility
            string windowTitle = "Final Year Project";

            // Need to register these handlers before initialising the window
            Action<PhotinoWindowOptions> windowConfiguration = options =>
            {
                // This event handler is fired before the windows constructor is called
                options.WindowCreatingHandler += (object sender, EventArgs args) =>
                {
                    var window = (PhotinoWindow)sender; // Instance is not initialized at this point. Class properties are not set yet.
                    Console.WriteLine($"Creating new PhotinoWindow instance.");
                };

                // This event handler is fired after the windows constructor is called
                options.WindowCreatedHandler += (object sender, EventArgs args) =>
                {
                    var window = (PhotinoWindow)sender; // Instance is initialized. Class properties are now set and can be used.
                    Console.WriteLine($"Created new PhotinoWindow instance with title {window.Title}.");
                };

                // This event handler will handle messages sent from the GUI.
                // I could register this after initialising the window but seems neater here
                options.WebMessageReceivedHandler += MessageHandler;

                //options.WindowClosingHandler - If I need to 
            };

            var window = new PhotinoWindow(windowTitle, windowConfiguration)
                .Resize(50, 50, "%")
                .Center()
                .UserCanResize(true)
                .Load("wwwroot/index.html");

            // Need to instantiate queue manager and module manager
            // before we wait for the window to close
            ModuleManager = new();
            QueueManager = new(ModuleManager);
            DiagramManager = new();

            // Load Modules
            //ModuleManager.LoadModules();

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
            var window = (PhotinoWindow)sender;

            string[] command = message.Split(',');
            switch (command[0])
            {
                case "openFunc":
                    try
                    {
                        // Retrieve diagram
                        string diagram = DiagramManager.GetDiagramXML(out string name);

                        // Send diagram XML to the GUI
                        window.SendWebMessage($"loadDiagramFunc,{diagram},{name}");
                        Debug.WriteLine(diagram);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);

                        window.OpenAlertWindow("Loading Diagram", "Failed to load diagram.");
                    }
                    break;

                case "saveFunc":
                    try
                    {
                        DiagramManager.SaveDiagram(command[1], out string name, out bool cancelled);

                        if (!cancelled)
                        {
                            window.OpenAlertWindow("Saving Diagram", "Saved diagram successfully.");
                        }

                        window.SendWebMessage($"saveDiagramReply,{(cancelled ? "cancelled" : "success")}, {name}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);

                        window.OpenAlertWindow("Saving Diagram", "Failed to save diagram.");
                    }
                    break;

                case "newDiagramFunc":
                    // Just clear the "cached" diagram
                    DiagramManager.ClearDiagram();
                    break;

                case "playWorkflowFunc":
                    if (!QueueManager.HasQueue)
                    {
                        QueueManager.LoadQueue(DiagramManager.ParseCurrentDiagramXML());
                    }
                    
                    QueueManager.StartQueue();


                    break;

                case "pauseWorkflowFunc":
                    QueueManager.PauseQueue();

                    //window.SendWebMessage("pauseReply,success");

                    break;

                case "stopWorkflowFunc":
                    QueueManager.StopQueue();

                    //window.SendWebMessage("stopReply,success");

                    break;

                case "test":
                    window.SendWebMessage(message);
                    break;

                case "loadModuleInfoFunc":
                    string moduleInfo = ModuleManager.ModuleInfoAsJSONString();
                    
                    Debug.WriteLine(moduleInfo);

                    window.SendWebMessage($"loadModuleInfoReply,{moduleInfo}");
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Received an unknown command");
            }

        }
    }
}
