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

        public static void SendMessage(string command, string args = null)
        {
            command += string.IsNullOrWhiteSpace(args) ? "" : $"!,!{args}";

            CurrentWindow.SendWebMessage(command);
        }
    }
}
