using System;

namespace FinalYearProject.Backend.Utils
{
    public class AppDirectories
    {
        /// <summary>
        /// App Directory
        /// </summary>
        public static readonly string APP_DIRECTORY;

        /// <summary>
        /// DLL Directory
        /// </summary>
        public static string DLL_DIRECTORY;

        /// <summary>
        /// Supporting DLLs Directory
        /// </summary>
        public static string SUPPORTING_DLLS_DIRECTORY;

        /// <summary>
        /// Static constructor
        /// </summary>
        static AppDirectories()
        {
            APP_DIRECTORY =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\FinalYearProject";

            DLL_DIRECTORY = APP_DIRECTORY + "\\Modules";

            SUPPORTING_DLLS_DIRECTORY = DLL_DIRECTORY + "\\Supporting Libraries";
        }
    }
}
