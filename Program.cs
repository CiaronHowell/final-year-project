using System;
using System.Diagnostics;
using PhotinoNET;

namespace HelloWorldVue
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Window title declared here for visibility
            string windowTitle = "Final Year Project";

            // Define the PhotinoWindow options. Some handlers 
            // can only be registered before a PhotinoWindow instance
            // is initialized. Currently there are three handlers
            // that must be defined here.
            Action<PhotinoWindowOptions> windowConfiguration = options =>
            {
                // Window creating and created handlers can only be
                // registered during initialization of a PhotinoWindow instance.
                // These handlers are fired before and after the native constructor
                // method is called.
                options.WindowCreatingHandler += (object sender, EventArgs args) =>
                {
                    var window = (PhotinoWindow)sender; // Instance is not initialized at this point. Class properties are not set yet.
                    Console.WriteLine($"Creating new PhotinoWindow instance.");
                };

                options.WindowCreatedHandler += (object sender, EventArgs args) =>
                {
                    var window = (PhotinoWindow)sender; // Instance is initialized. Class properties are now set and can be used.
                    Console.WriteLine($"Created new PhotinoWindow instance with title {window.Title}.");
                };
            };

            var window = new PhotinoWindow(windowTitle, windowConfiguration)
                .Resize(50, 50, "%")
                .Center()
                .UserCanResize(true)
                // Most event handlers can be registered after the
                // PhotinoWindow was instantiated by calling a registration 
                // method like the following RegisterWebMessageReceivedHandler.
                // This could be added in the PhotinoWindowOptions if preferred.
                .RegisterWebMessageReceivedHandler(MessageHandler)
                .Load("wwwroot/index.html");

            //DiagramManager.SaveDiagram("test");

            //Debug.WriteLine($"DiagramManager call: {DiagramManager.GetDiagramXML()}");

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
                    string diagram = DiagramManager.GetDiagramXML();

                    window.SendWebMessage($"loadDiagramFunc,{diagram}");
                    break;

                case "saveFunc":
                    DiagramManager.SaveDiagram(command[1]);
                    break;

                case "test":
                    window.SendWebMessage(command[1]);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Received an unknown command");
            }

        }
    }
}
