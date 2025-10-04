using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Executors
{
    internal class GitHubPagesEnabler : BaseExecutor
    {
        public GitHubPagesEnabler(Context context, ProcessExecutor processExecutor) : base(context, processExecutor)
        {
        }

        public override void Execute()
        {
            var result = _processExecutor.RunProcess("gh", $"api -X POST \"repos/{_context.RepositoryPath}/pages\" -f \"source[branch]=main\" -f \"source[path]=/docs\"");
            
            if(result.IsError)
            {
                throw CreateException(result, $"Could not enable GitHub Pages");
            }
        }
    }
}
