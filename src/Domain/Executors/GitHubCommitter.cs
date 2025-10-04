using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Domain.Executors
{
    internal class GitHubCommitter : BaseExecutor
    {
        public GitHubCommitter(Context context, ProcessExecutor processExecutor) : base(context, processExecutor)
        {
        }

        public override void Execute()
        {
            var commitMessage = "Template post-processing";

            _processExecutor.RunProcess("git", $"commit -m \"{commitMessage}\"");
        }
    }
}
