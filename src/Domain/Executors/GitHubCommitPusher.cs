using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Executors
{
    internal class GitHubCommitPusher : BaseExecutor
    {
        public GitHubCommitPusher(Context context, ProcessExecutor processExecutor) : base(context, processExecutor)
        {
        }

        public override void Execute()
        {
            Directory.SetCurrentDirectory(_context.OutputPath);
            var remoteOriginResult = _processExecutor.RunProcess("git", "remote get-url origin");

            if (remoteOriginResult.IsError)
            {
                throw CreateException(remoteOriginResult, "Failed to get remote origin");
            }

            var statusResult = _processExecutor.RunProcess("git", "status --porcelain");

            if (statusResult.IsError)
            {
                throw CreateException(statusResult, "Failed to get git status");
            }

            _processExecutor.RunProcess("git", "add .");
            _processExecutor.RunProcess("git", "commit -m \"Final setup and configuration changes\"");

            var pushResult = _processExecutor.RunProcess("git", "push origin main");

            if (pushResult.IsError)
            {
                throw CreateException(pushResult, "Failed to push changes to remote");
            }
        }
    }
}
