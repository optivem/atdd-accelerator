using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Executors
{
    internal class GitHubCommitPusher : BaseExecutor
    {
        public GitHubCommitPusher(Context context) : base(context)
        {
        }

        public override void Execute()
        {
            Directory.SetCurrentDirectory(_context.OutputPath);
            var remoteOriginResult = ProcessExecutor.RunProcess("git", "remote get-url origin");

            if (remoteOriginResult.IsError)
            {
                throw new ProcessException(remoteOriginResult, "Failed to get remote origin");
            }

            var statusResult = ProcessExecutor.RunProcess("git", "status --porcelain");

            if (statusResult.IsError)
            {
                throw new ProcessException(statusResult, "Failed to get git status");
            }

            ProcessExecutor.RunProcess("git", "add .");
            ProcessExecutor.RunProcess("git", "commit -m \"Final setup and configuration changes\"");

            var pushResult = ProcessExecutor.RunProcess("git", "push origin main");

            if (pushResult.IsError)
            {
                throw new ProcessException(pushResult, "Failed to push changes to remote");
            }
        }
    }
}
