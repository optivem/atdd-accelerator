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

        public BaseExecutor(Context context)
        { 
            _context = context; 
        }

        public abstract void Execute();

        // TODO: Add property
    }
}
