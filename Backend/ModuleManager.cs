using FinalYearProject.Backend.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FinalYearProject.Backend
{
    public class ModuleManager
    {
        /// <summary>
        /// List of created instances
        /// </summary>
        private List<object> _createdInstances;

        private readonly string APP_DIRECTORY;


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
            Console.WriteLine(APP_DIRECTORY);

            if (!Directory.Exists(APP_DIRECTORY))
                Directory.CreateDirectory(APP_DIRECTORY);

            ModuleMethods = new Dictionary<string, Method>();
            _createdInstances = new();
        }

        public void LoadModules()
        {
            // Search Directory of DLLs
            string[] files = Directory.GetFiles(APP_DIRECTORY, "*.dll");
            if (files.Length == 0)
            {
                Console.WriteLine("No files located");
                return;
            }

            foreach (string file in files)
            {
                // Use LoadModule with the DLLs found
                LoadModule(file);
            }

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
                    Console.WriteLine(name);

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
        private static Dictionary<string, Type> GetParameterInfo(MethodInfo method)
        {
            // Create variable to return
            Dictionary<string, Type> parameters = new();
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                Console.WriteLine($"     {parameter.Name} ({parameter.ParameterType})");
                // Add parameter information to the dictionary
                parameters.Add(parameter.Name, parameter.ParameterType);
            }

            return parameters;
        }

        /// <summary>
        /// Runs the method
        /// </summary>
        /// <param name="methodName"></param>
        public void Run(string methodName)
        {
            if (!ModuleMethods.ContainsKey(methodName))
            {
                // TODO: Should I throw an error here or just log it
                Console.WriteLine("Method not found");
                return;
            }

            Console.WriteLine($"Running {methodName}");
            Method method = ModuleMethods[methodName];

            // TODO: Might need to store the instance permanently
            if (!_createdInstances.Exists(x => x.GetType() == method.InstanceType))
            {
                Console.WriteLine("Instance hit");
                _createdInstances.Add(Activator.CreateInstance(method.InstanceType));
            }

            // The constructor is hit when we create the instance
            method.MethodInfo.Invoke(_createdInstances.Find(instance => instance.GetType() == method.InstanceType), null);
        }

    }
}
