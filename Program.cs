using System;
using System.Diagnostics;
using FinalYearProject.Backend;
using PhotinoNET;

namespace FinalYearProject
{
    class Program
    {
        private static QueueManager QueueManager { get; set; }

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

            // Need to instantiate queue manager before we wait for the window to close
            QueueManager = new();

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
                        string diagram = DiagramManager.GetDiagramXML();

                        // Send diagram XML to the GUI
                        window.SendWebMessage($"loadDiagramFunc,{diagram}");
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
                        DiagramManager.SaveDiagram(command[1]);

                        window.OpenAlertWindow("Saving Diagram", "Saved diagram successfully.");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);

                        window.OpenAlertWindow("Saving Diagram", "Failed to save diagram.");
                    }
                    break;

                case "playWorkflowFunc":
                    if (!QueueManager.HasQueue)
                    {
                        // TODO: Add code to load queue
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

                default:
                    throw new ArgumentOutOfRangeException("Received an unknown command");
            }

        }
    }
}
