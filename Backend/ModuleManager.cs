using FinalYearProject.Backend.Utils.Structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        /// App Directory
        /// </summary>
        private readonly string APP_DIRECTORY;

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
            APP_DIRECTORY =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FinalYearProject";

            // We need to create the app directory for the DLL files
            if (!Directory.Exists(APP_DIRECTORY))
                Directory.CreateDirectory(APP_DIRECTORY);

            ModuleMethods = new Dictionary<string, Method>();
            _createdInstances = new();
        }

        /// <summary>
        /// Load all DLL files located in app directory
        /// </summary>
        public void LoadModules()
        {
            _loadingModules = true;

            // Search Directory of DLLs
            string[] files = Directory.GetFiles(APP_DIRECTORY, "*.dll", 
                new EnumerationOptions 
                { 
                    RecurseSubdirectories = true
                });
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
                foreach (MethodInfo info in infos)
                {
                    string name = $"{type.Name}.{info.Name}";
                    Debug.WriteLine(name);

                    ModuleMethods.Add(
                        name,
                        new Method(
                            info,
                            GetParameterInfo(info),
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
                // TODO: Should I throw an error here or just log it
                Debug.WriteLine("Method not found");
                return;
            }

            Debug.WriteLine($"Running {methodName}");
            Method method = ModuleMethods[methodName];

            // We need to make sure that only one instance of the class is created for all methods
            if (!_createdInstances.Exists(activatedInstance => activatedInstance.GetType() == method.InstanceType))
            {
                Debug.WriteLine("Instance hit");
                _createdInstances.Add(Activator.CreateInstance(method.InstanceType));
            }

            object[] parametersArray = null;
            if (parameters.ParameterList != null)
            {
                int length = parameters.ParameterList.Count;

                parametersArray = new object[length];

                int count = 0;
                foreach (var element in parameters.ParameterList)
                {
                    // convert to their respective types 
                    Type parameterType = Type.GetType(element.Value.Type);
                    // Get the converter for the type
                    TypeConverter converter = TypeDescriptor.GetConverter(parameterType);

                    object parsedParameterValue = converter.ConvertFromString(element.Value.Value);

                    parametersArray[count++] = parsedParameterValue;
                    Debug.WriteLine(element.Value.Value);
                }
            }

            // We use the activated instance to run the method 
            method.MethodInfo.Invoke(_createdInstances.Find(activatedInstance => activatedInstance.GetType() == method.InstanceType), parametersArray);
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
