using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions
{
    public class ProcessException : Exception
    {
        public ProcessException(ProcessResult result, string customMessage)
            : base(customMessage + "\n" + result.ToString())
        {
            Result = result;
            CustomMessage = customMessage;
        }

        public ProcessResult Result { get; }
        public string CustomMessage { get; }
    }
}
