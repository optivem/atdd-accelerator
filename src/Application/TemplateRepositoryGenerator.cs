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
        private readonly GitHubCommitter _githubCommitter;
        private readonly GitHubPagesEnabler _gitHubPagesEnabler;
        private readonly LocalRepositoryDeleter _localRepositoryDeleter;
        private readonly GitHubCommitWorkflowWaiter _gitHubCommitWorkflowWaiter;
        private readonly GitHubTestReleaseWorkflowWaiter _gitHubTestReleaseWorkflowWaiter;

        public TemplateRepositoryGenerator(Context context, ProcessExecutor processExecutor, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<TemplateRepositoryGenerator>();
            _gitHubPreconditionsChecker = new GitHubPreconditionsChecker(context, processExecutor);
            _gitHubRepositoryTemplateGenerator = new GitHubRepositoryTemplateGenerator(context, processExecutor);
            _gitHubCommitPusher = new GitHubCommitPusher(context, processExecutor);
            _localLanguageFoldersCleaner = new LocalLanguageFoldersCleaner(context, processExecutor);
            _localReadmeBadgeUpdater = new LocalReadmeBadgeUpdater(context, processExecutor);
            _localDockerComposeUpdater = new LocalDockerComposeUpdater(context, processExecutor);
            _githubCommitter = new GitHubCommitter(context, processExecutor);
            _gitHubPagesEnabler = new GitHubPagesEnabler(context, processExecutor);
            _localRepositoryDeleter = new LocalRepositoryDeleter(context, processExecutor);
            _gitHubCommitWorkflowWaiter = new GitHubCommitWorkflowWaiter(context, processExecutor);
            _gitHubTestReleaseWorkflowWaiter = new GitHubTestReleaseWorkflowWaiter(context, processExecutor);
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

                _githubCommitter.Execute();
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
