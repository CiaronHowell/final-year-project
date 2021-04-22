using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalYearProject.Backend.Utils.Structs
{
    /// <summary>
    /// Struct to contain workflow method
    /// </summary>
    public struct WorkflowMethod
    {
        /// <summary>
        /// Workflow method constructor
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="parameters">Method parameters</param>
        public WorkflowMethod(string methodId, string methodName, Parameters parameters)
        {
            MethodId = methodId;
            MethodName = methodName;
            Parameters = parameters;
        }

        /// <summary>
        /// Method name
        /// </summary>
        public string MethodId { get; private set; }

        /// <summary>
        /// Method name
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Method parameters
        /// </summary>
        public Parameters Parameters { get; private set; }
    }

    /// <summary>
    /// Struct to help manage the parameters from the workflow
    /// </summary>
    public struct Parameters
    {
        /// <summary>
        /// Parameters Constructor
        /// </summary>
        /// <param name="parameterList"></param>
        public Parameters(Dictionary<string, ParameterDetails> parameterList)
        {
            ParameterList = parameterList;
        }

        /// <summary>
        /// List of parameters
        /// </summary>
        public Dictionary<string, ParameterDetails> ParameterList { get; private set; }
    }

    /// <summary>
    /// Struct to hold parameter details
    /// </summary>
    public struct ParameterDetails
    {
        /// <summary>
        /// Parameter Details Constructor
        /// </summary>
        /// <param name="value">Value of the parameter</param>
        /// <param name="type">Type of the parameter</param>
        public ParameterDetails(
            string value,
            string type)
        {
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Value of the parameter
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Type of the parameter
        /// </summary>
        public string Type { get; private set; }
    }
}
