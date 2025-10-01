using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities
{
    public class ProcessException : Exception
    {
        public ProcessException(ProcessResult result, string customMessage)
        {
            Result = result;
            CustomMessage = customMessage;
        }

        public ProcessResult Result { get; }
        public string CustomMessage { get; }
    }
}
