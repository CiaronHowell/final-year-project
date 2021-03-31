using System;
using PhotinoNET;

namespace HelloWorldVue
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Window title declared here for visibility
            string windowTitle = "Photino.Vue Demo App";

            // Define the PhotinoWindow options. Some handlers 
            // can only be registered before a PhotinoWindow instance
            // is initialized. Currently there are three handlers
            // that must be defined here.
            Action<PhotinoWindowOptions> windowConfiguration = options =>
            {
                // Custom scheme handlers can only be registered during
                // initialization of a PhotinoWindow instance.
                // options.CustomSchemeHandlers.Add("app", (string url, out string contentType) =>
                // {
                //     contentType = "text/javascript";
                //     return new MemoryStream(Encoding.UTF8.GetBytes(@"
                //         (() =>{
                //             window.setTimeout(() => {
                //                 alert(`🎉 Dynamically inserted JavaScript.`);
                //             }, 1000);
                //         })();
                //     "));
                // });

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

            DiagramManager manager = new DiagramManager();
            manager.SaveDiagram("test");

            window.WaitForClose();

        }

        private static void MessageHandler(object sender, string message) {
            var window = (PhotinoWindow)sender;

            // The message argument is coming in from sendMessage.
            // "window.external.sendMessage(message: string)"
            string response = $"Received message: \"{message}\"";

            // Send a message back the to JavaScript event handler.
            // "window.external.receiveMessage(callback: Function)"
            window.SendWebMessage(response);
        }
    }
}
