using PhotinoNET;

namespace FinalYearProject.Backend.Utils
{
    /// <summary>
    /// Window
    /// </summary>
    public class Window
    {
        public static PhotinoWindow CurrentWindow { get; set; }

        public static void SendMessage(string command, string args = null)
        {
            command += string.IsNullOrWhiteSpace(args) ? "" : $",{args}";

            CurrentWindow.SendWebMessage(command);
        }

        // TODO: Add sendmessage that will use the current window
    }
}
