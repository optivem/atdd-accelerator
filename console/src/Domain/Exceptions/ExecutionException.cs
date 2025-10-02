using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions
{
    public class ExecutionException : Exception
    {
        public ExecutionException(Context context, string customMessage)
            : base(customMessage)
        {
            Context = context;
            CustomMessage = customMessage;
        }

        public Context Context { get; }
        public string CustomMessage { get; }
    }
}
