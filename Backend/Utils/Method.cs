using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FinalYearProject.Backend.Utils
{
    /// <summary>
    /// Struct to contain method info
    /// </summary>
    public struct Method
    {
        /// <summary>
        /// Constructor for method
        /// </summary>
        /// <param name="methodInfo">MethodInfo</param>
        /// <param name="parameters">Dictionary of parameter name and parameter type</param>
        /// <param name="instanceType">Type of the class(instance)</param>
        public Method(
            MethodInfo methodInfo,
            Dictionary<string, string> parameters,
            Type instanceType)
        {
            MethodInfo = methodInfo;
            Parameters = parameters;
            InstanceType = instanceType;
        }

        /// <summary>
        /// Method Info
        /// </summary>
        [JsonIgnore]
        public MethodInfo MethodInfo { get; private set; }

        /// <summary>
        /// Parameter names and respective types
        /// </summary>
        public Dictionary<string, string> Parameters { get; private set; }

        /// <summary>
        /// Instance type for creating instance
        /// </summary>
        [JsonIgnore]
        public Type InstanceType { get; private set; }
    }
}
