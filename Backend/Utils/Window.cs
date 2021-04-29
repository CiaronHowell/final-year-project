using PhotinoNET;

namespace FinalYearProject.Backend.Utils
{
    /// <summary>
    /// Window
    /// </summary>
    public class Window
    {
        /// <summary>
        /// Current Window to allow access to send message in Managers
        /// </summary>
        public static PhotinoWindow CurrentWindow { get; set; }

        /// <summary>
        /// Sends a message to the GUI
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="args">Arguments</param>
        public static void SendMessage(string command, string args = null)
        {
            // Append args if args isn't null or just append nothing
            command += string.IsNullOrWhiteSpace(args) ? "" : $"!,!{args}";

            CurrentWindow.SendWebMessage(command);
        }
    }
}
