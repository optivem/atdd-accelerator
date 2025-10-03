using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using Serilog.Core;
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
            
            CheckRepositoryOwnerExists();
            CheckRepositoryNameDoesNotExist();
        }

        private void CheckGitInstalled()
        {
            TryExecute("git", "--version", "Git is not installed or not available in PATH. See installation instructions.");
        }

        private void CheckGitHubCliInstalled()
        {
            TryExecute("gh", "--version", "GitHub CLI (gh) is not installed or not available in PATH. See installation instructions.");
        }

        private void CheckDockerInstalled()
        {
            TryExecute("docker", "--version", "Docker is not installed or not available in PATH. See installation instructions.");
        }


        private void CheckGitHubAuthenticated()
        {
            var processResult = TryExecute("gh", "auth status", "You are not authenticated with GitHub CLI (gh). Please run 'gh auth login' to authenticate.");

            // Optionally, check output for explicit authentication confirmation
            if (!processResult.Output.Contains("Logged in to github.com", StringComparison.OrdinalIgnoreCase))
            {
                throw CreateException(processResult, "GitHub CLI (gh) is not authenticated. Please run 'gh auth login' to authenticate.");
            }
        }


        private void CheckRepositoryOwnerExists()
        {
            var processResult = ProcessExecutor.RunProcess("gh", $"api /users/{_context.RepositoryOwner}");

            if (processResult.IsError)
            {
                throw CreateException(processResult, $"--repository-owner '{_context.RepositoryOwner}' does not exist on GitHub.");
            }
        }


        private void CheckRepositoryNameDoesNotExist()
        {
            // Use GitHub CLI to check if the repository exists for the owner
            var processResult = ProcessExecutor.RunProcess("gh", $"api /repos/{_context.RepositoryOwner}/{_context.RepositoryName}");

            // If the process succeeded, the repository exists and we should throw
            if (processResult.IsSuccess)
            {
                throw CreateException(processResult, $"--repository-name '{_context.RepositoryName}' already exists for owner '{_context.RepositoryOwner}' on GitHub.");
            }
            // If the process failed, check if it's a 404 (not found), which means it's safe to proceed
            // Otherwise, propagate the error
            if (processResult.Errors != null && !processResult.Errors.Contains("Not Found", StringComparison.OrdinalIgnoreCase))
            {
                throw CreateException(processResult, $"Error checking if repository '{_context.RepositoryName}' exists for owner '{_context.RepositoryOwner}': {processResult.Errors}");
            }
        }

        private ProcessResult TryExecute(string command, string args, string errorMessage)
        {
            ProcessResult? processResult = null;

            try
            {
                processResult = ProcessExecutor.RunProcess(command, args);

                if (processResult.IsError)
                {
                    throw CreateException(processResult, errorMessage);
                }

                return processResult;
            }
            catch (Exception ex)
            {
                throw CreateException(errorMessage);
            }
        }

    }
}

