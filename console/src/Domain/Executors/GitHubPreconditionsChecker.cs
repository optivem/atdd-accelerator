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
            CheckGitInstalled();
            CheckGitHubCliInstalled();
            CheckDockerInstalled();

            CheckGitHubAuthenticated();
        }


        private void CheckGitHubCliInstalled()
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

        private void CheckGitInstalled()
        {
            try
            {
                var processResult = ProcessExecutor.RunProcess("git", "--version");
                if (processResult.IsError)
                {
                    throw CreateException(processResult, "Git is not installed or not available in PATH. See installation instructions");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Git is not installed or not available in PATH. See installation instructions at https://git-scm.com/book/en/v2/Getting-Started-Installing-Git", ex);
            }
        }

        private void CheckDockerInstalled()
        {
            try
            {
                var processResult = ProcessExecutor.RunProcess("docker", "--version");
                if (processResult.IsError)
                {
                    throw CreateException(processResult, "Docker is not installed or not available in PATH. See installation instructions");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Docker is not installed or not available in PATH. See installation instructions at https://docs.docker.com/get-docker/", ex);
            }
        }

        private void CheckGitHubAuthenticated()
        {
            // Use GitHub CLI to check authentication status
            var processResult = ProcessExecutor.RunProcess("gh", "auth status");

            if (processResult.IsError || processResult.ExitCode != 0)
            {
                throw CreateException(processResult, "You are not authenticated with GitHub CLI (gh). Please run 'gh auth login' to authenticate.");
            }

            // Optionally, check output for explicit authentication confirmation
            if (!processResult.Output.Contains("Logged in to github.com", StringComparison.OrdinalIgnoreCase))
            {
                throw CreateException(processResult, "GitHub CLI (gh) is not authenticated. Please run 'gh auth login' to authenticate.");
            }
        }

    }
}

