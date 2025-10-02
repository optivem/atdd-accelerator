using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Domain.Executors
{
    internal class GitHubPreconditionsChecker : BaseExecutor
    {
        public GitHubPreconditionsChecker(Context context) : base(context)
        {
        }

        public override void Execute()
        {
            CheckGitHubCli();

            // check gh
            // check git
        }

        private void CheckGitHubCli()
        {
            try
            {
                var processResult = ProcessExecutor.RunProcess("gh", "--version");

                if (processResult.IsError)
                {
                    throw CreateException(processResult, "GitHub CLI (gh) is not installed or not available in PATH. See installation instructions");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("GitHub CLI (gh) is not installed or not available in PATH. See installation instructions at https://cli.github.com/manual/installation", ex);
            }
        }
    }
}

