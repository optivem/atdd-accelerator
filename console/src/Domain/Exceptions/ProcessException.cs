using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions
{
    public class ProcessException : ExecutionException
    {
        public ProcessException(Context context, ProcessResult result, string customMessage)
            : base(context, customMessage)
        {
            Result = result;
        }

        public ProcessResult Result { get; }
    }
}
