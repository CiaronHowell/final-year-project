using FinalYearProject.Backend.Utils;
using FinalYearProject.Backend.Utils.Structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace FinalYearProject.Backend
{
    /// <summary>
    /// Module Manager
    /// </summary>
    public class ModuleManager
    {
        /// <summary>
        /// Dirty flag for loading modules
        /// </summary>
        private bool _loadingModules;

        /// <summary>
        /// List of created instances
        /// </summary>
        private List<object> _createdInstances;

        /// <summary>
        /// Dictionary of loaded "Module" methods
        /// </summary>
        public Dictionary<string, Method> ModuleMethods { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ModuleManager()
        {
            // We need to create the app directory for the DLL files
            if (!Directory.Exists(AppDirectories.APP_DIRECTORY))
                Directory.CreateDirectory(AppDirectories.APP_DIRECTORY);

            if (!Directory.Exists(AppDirectories.DLL_DIRECTORY))
                Directory.CreateDirectory(AppDirectories.DLL_DIRECTORY);

            if (!Directory.Exists(AppDirectories.SUPPORTING_DLLS_DIRECTORY))
                Directory.CreateDirectory(AppDirectories.SUPPORTING_DLLS_DIRECTORY);

            ModuleMethods = new Dictionary<string, Method>();
            _createdInstances = new();

            // Load libraries before Modules
            LoadSupportingLibraries();

            LoadModules();
        }

        private void LoadSupportingLibraries()
        {
            // Search directory for dlls
            string[] files = Directory.GetFiles(AppDirectories.SUPPORTING_DLLS_DIRECTORY, "*.dll");
            if (files.Length == 0)
            {
                Debug.WriteLine("No files located");
                _loadingModules = false;
                return;
            }

            foreach (string file in files)
            {
                // Load supporting library to Assembly
                Assembly.LoadFrom(file);
            }
        }

        /// <summary>
        /// Load all DLL files located in app directory
        /// </summary>
        private void LoadModules()
        {
            _loadingModules = true;

            // Search Directory of DLLs
            string[] files = Directory.GetFiles(AppDirectories.DLL_DIRECTORY, "*.dll");
            if (files.Length == 0)
            {
                Debug.WriteLine("No files located");
                _loadingModules = false;
                return;
            }

            foreach (string file in files)
            {
                // Use LoadModule with the DLLs found
                LoadModule(file);
            }

            _loadingModules = false;
        }

        /// <summary>
        /// Load a single module
        /// </summary>
        /// <param name="dllName">Name of DLL to load from</param>
        private void LoadModule(string dllName)
        {
            // Load the DLL 
            Assembly assembly = Assembly.LoadFile(dllName);
            // Get all the types (in this case it will be classes) within the DLL
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                // Skip if the type is private
                if (!type.IsPublic)
                    continue;

                // Get the methods of the type
                MethodInfo[] infos = type.GetMethods(
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.InvokeMethod |
                    BindingFlags.DeclaredOnly);
                foreach (MethodInfo methodInfo in infos)
                {
                    string methodName = $"{type.Name}.{methodInfo.Name}";
                    Debug.WriteLine(methodName);

                    ModuleMethods.Add(
                        methodName,
                        new Method(
                            methodInfo,
                            GetParameterInfo(methodInfo),
                            type)
                        );
                }

            }
        }

        /// <summary>
        /// Gets parameter info from method
        /// </summary>
        /// <param name="method">Method info</param>
        /// <returns>Dictionary of the parameter name and parameter type</returns>
        private static Dictionary<string, string> GetParameterInfo(MethodInfo method)
        {
            // Create variable to return
            Dictionary<string, string> parameters = new();
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                Debug.WriteLine($"     {parameter.Name} ({parameter.ParameterType})");
                // Add parameter information to the dictionary
                parameters.Add(parameter.Name, parameter.ParameterType.ToString());
            }

            return parameters;
        }

        /// <summary>
        /// Runs the method
        /// </summary>
        /// <param name="methodName"></param>
        public void Run(string methodName, Parameters parameters)
        {
            if (!ModuleMethods.ContainsKey(methodName))
            {
                Debug.WriteLine("Method not found");
                return;
            }

            Debug.WriteLine($"Running {methodName}");

            Method method = ModuleMethods[methodName];
            // We need to make sure that only one instance of the class is created for all methods
            if (!_createdInstances.Exists(activatedInstance => activatedInstance.GetType() == method.InstanceType))
            {
                // We create an instance of the class and store it
                _createdInstances.Add(Activator.CreateInstance(method.InstanceType));
            }

            object[] inputParameters = null;
            if (parameters.ParameterList != null)
            {
                int length = parameters.ParameterList.Count;
                // Initialise the array to contain the parameters
                inputParameters = new object[length];

                int count = 0;
                // Go through 
                foreach (var element in parameters.ParameterList)
                {
                    // convert to their respective types 
                    Type parameterType = Type.GetType(element.Value.Type);
                    // Get the converter for the type
                    TypeConverter converter = TypeDescriptor.GetConverter(parameterType);

                    object parsedParameterValue = converter.ConvertFromString(element.Value.Value);

                    inputParameters[count++] = parsedParameterValue;
                    Debug.WriteLine(element.Value.Value);
                }
            }

            try
            {
                // We use the activated instance to run the method. We will also wait for invoked method to finish
                method.MethodInfo.Invoke(
                    _createdInstances.Find(activatedInstance => activatedInstance.GetType() == method.InstanceType),
                    inputParameters);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occured from invoked method: {ex}");
            }
        }

        /// <summary>
        /// Gets the module info as a json string 
        /// to make it easier to send to GUI
        /// </summary>
        /// <returns>ModuleInfo JSON as string</returns>
        public string ModuleInfoAsJSONString()
        {
            // Wait for the modules to load first 
            if (_loadingModules)
                SpinWait.SpinUntil(() => !_loadingModules);

            return JsonConvert.SerializeObject(ModuleMethods);
        }

    }
}
