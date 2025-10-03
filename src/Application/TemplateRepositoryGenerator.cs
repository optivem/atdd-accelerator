using Microsoft.Extensions.Logging;
using Optivem.AtddAccelerator.TemplateGenerator.Core.Executors;
using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using Optivem.AtddAccelerator.TemplateGenerator.Domain.Executors;
using Optivem.AtddAccelerator.TemplateGenerator.Presentation.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Application
{
    public class TemplateRepositoryGenerator
    {
        private readonly Context _context;
        private readonly ILogger<TemplateRepositoryGenerator> _logger;

        private readonly GitHubPreconditionsChecker _gitHubPreconditionsChecker;
        private readonly GitHubRepositoryTemplateGenerator _gitHubRepositoryTemplateGenerator;
        private readonly GitHubCommitPusher _gitHubCommitPusher;
        private readonly LocalLanguageFoldersCleaner _localLanguageFoldersCleaner;
        private readonly LocalReadmeBadgeUpdater _localReadmeBadgeUpdater;
        private readonly LocalDockerComposeUpdater _localDockerComposeUpdater;
        private readonly GitHubPagesEnabler _gitHubPagesEnabler;
        private readonly LocalRepositoryDeleter _localRepositoryDeleter;
        private readonly GitHubCommitWorkflowWaiter _gitHubCommitWorkflowWaiter;
        private readonly GitHubTestReleaseWorkflowWaiter _gitHubTestReleaseWorkflowWaiter;

        public TemplateRepositoryGenerator(Context context, ILogger<TemplateRepositoryGenerator> logger)
        {
            _context = context;
            _logger = logger;
            _gitHubPreconditionsChecker = new GitHubPreconditionsChecker(context);
            _gitHubRepositoryTemplateGenerator = new GitHubRepositoryTemplateGenerator(context);
            _gitHubCommitPusher = new GitHubCommitPusher(context);
            _localLanguageFoldersCleaner = new LocalLanguageFoldersCleaner(context);
            _localReadmeBadgeUpdater = new LocalReadmeBadgeUpdater(context);
            _localDockerComposeUpdater = new LocalDockerComposeUpdater(context);
            _gitHubPagesEnabler = new GitHubPagesEnabler(context);
            _localRepositoryDeleter = new LocalRepositoryDeleter(context);
            _gitHubCommitWorkflowWaiter = new GitHubCommitWorkflowWaiter(context);
            _gitHubTestReleaseWorkflowWaiter = new GitHubTestReleaseWorkflowWaiter(context);
        }

        public async Task GenerateAsync()
        {
            try
            {
                _logger.LogInformation("Checking preconditions...");
                _gitHubPreconditionsChecker.Execute();
                _gitHubRepositoryTemplateGenerator.Execute();
                _logger.LogInformation("Repository {RepositoryName} was created.", _context.RepositoryName);
                
                
                _localLanguageFoldersCleaner.Execute();
                _localReadmeBadgeUpdater.Execute();
                _localDockerComposeUpdater.Execute();

                var commitMessage = "Template post-processing";
                ProcessExecutor.RunProcess("git", $"commit -m \"{commitMessage}\"");

                _gitHubPagesEnabler.Execute();
                _gitHubCommitPusher.Execute();
                _logger.LogInformation("Finished post-processing.");

                _logger.LogInformation("Waiting for Commit Stage to complete. This may take several minutes. Please wait...");
                _gitHubCommitWorkflowWaiter.Execute();
                _logger.LogInformation("Triggering Acceptance Stage and Release Stages. This may take several minutes. Please wait...");
                _gitHubTestReleaseWorkflowWaiter.Execute();
                _logger.LogInformation("Workflows have been successfully triggered.");
            }
            finally
            {
                _localRepositoryDeleter.Execute();
            }
        }
    }
}
