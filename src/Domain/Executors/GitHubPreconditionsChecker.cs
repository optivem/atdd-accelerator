using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions;
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
        public GitHubPreconditionsChecker(Context context, ProcessExecutor processExecutor) : base(context, processExecutor)
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

            // Check if logged in to github.com
            if (!processResult.Output.Contains("Logged in to github.com", StringComparison.OrdinalIgnoreCase))
            {
                throw CreateException(processResult, "GitHub CLI authentication verification failed. Please ensure you are properly logged in with 'gh auth login'.");
            }

            // Verify required scopes are present
            CheckRequiredScopes(processResult.Output);
        }

        private void CheckRequiredScopes(string authStatusOutput)
        {
            var requiredScopes = new[] { "repo", "workflow", "read:org" };
            var missingScopes = new List<string>();

            // Find the token scopes line in the output
            var lines = authStatusOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var scopesLine = lines.FirstOrDefault(line => line.Contains("Token scopes:", StringComparison.OrdinalIgnoreCase));

            if (scopesLine == null)
            {
                throw CreateException("Unable to determine GitHub CLI token scopes. Please re-authenticate with 'gh auth login --scopes \"repo,workflow,read:org\"'.");
            }

            // Extract scopes from the line (format: "Token scopes: 'gist', 'read:org', 'repo', 'workflow'")
            var scopesPart = scopesLine.Split(':', 2).LastOrDefault()?.Trim();
            if (string.IsNullOrEmpty(scopesPart))
            {
                throw CreateException("Unable to parse GitHub CLI token scopes. Please re-authenticate with 'gh auth login --scopes \"repo,workflow,read:org\"'.");
            }

            // Handle both quoted and unquoted formats, remove quotes and split by comma
            var currentScopes = scopesPart.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(s => s.Trim().Trim('\'', '"').ToLowerInvariant())
                                         .ToHashSet();

            // Check each required scope
            foreach (var requiredScope in requiredScopes)
            {
                if (!currentScopes.Contains(requiredScope.ToLowerInvariant()))
                {
                    missingScopes.Add(requiredScope);
                }
            }

            // If any scopes are missing, throw an error
            if (missingScopes.Any())
            {
                var missingScopesText = string.Join(", ", missingScopes);
                var allRequiredScopes = string.Join(",", requiredScopes);
                throw CreateException($"GitHub CLI is missing required scopes: {missingScopesText}. Please re-authenticate with 'gh auth login --scopes \"{allRequiredScopes}\"'.");
            }
        }


        private void CheckRepositoryOwnerExists()
        {
            var processResult = _processExecutor.RunProcess("gh", $"api /users/{_context.RepositoryOwner}");

            if (processResult.IsError)
            {
                throw CreateException(processResult, $"--repository-owner '{_context.RepositoryOwner}' does not exist on GitHub.");
            }
        }


        private void CheckRepositoryNameDoesNotExist()
        {
            // Use GitHub CLI to check if the repository exists for the owner
            var processResult = _processExecutor.RunProcess("gh", $"api /repos/{_context.RepositoryOwner}/{_context.RepositoryName}");

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
            try
            {
                var processResult = _processExecutor.RunProcess(command, args);

                if (processResult.IsError)
                {
                    throw CreateException(processResult, errorMessage);
                }

                return processResult;
            }
            catch (ProcessException)
            {
                // Re-throw ProcessException without modification
                throw;
            }
            catch (ExecutionException)
            {
                // Re-throw ExecutionException without modification
                throw;
            }
            catch (Exception)
            {
                // For unexpected exceptions (like process not found), create a new exception
                throw CreateException(errorMessage);
            }
        }

    }
}

