using Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities
{
    public abstract class BaseExecutor
    {
        protected readonly Context _context;
        protected readonly ProcessExecutor _processExecutor;

        public BaseExecutor(Context context, ProcessExecutor processExecutor)
        { 
            _context = context;
            _processExecutor = processExecutor;
        }

        public abstract void Execute();

        // TODO: Add property

        protected ProcessException CreateException(ProcessResult result, string message)
        {
            return new ProcessException(_context, result, message);
        }

        protected ExecutionException CreateException(string message)
        {
            return new ExecutionException(_context, message);
        }
    }
}
