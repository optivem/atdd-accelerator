using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities
{
    internal class ExecutionException : Exception
    {
        public ExecutionException(Context context, string customMessage)
        {
            Context = context;
            CustomMessage = customMessage;
        }

        public Context Context { get; }
        public string CustomMessage { get; }
    }
}
