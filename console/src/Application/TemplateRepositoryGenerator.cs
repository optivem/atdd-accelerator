using Optivem.AtddAccelerator.TemplateGenerator.Core.Executors;
using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Application
{
    internal class TemplateRepositoryGenerator
    {
        private readonly GitHubRepositoryTemplateGenerator _gitHubRepositoryTemplateGenerator;
        private readonly GitHubCommitPusher _gitHubCommitPusher;
        private readonly LocalLanguageFoldersCleaner _localLanguageFoldersCleaner;
        private readonly LocalReadmeBadgeUpdater _localReadmeBadgeUpdater;
        private readonly LocalDockerComposeUpdater _localDockerComposeUpdater;
        private readonly GitHubPagesEnabler _gitHubPagesEnabler;
        private readonly LocalRepositoryDeleter _localRepositoryDeleter;
        private readonly GitHubCommitWorkflowWaiter _gitHubCommitWorkflowWaiter;
        private readonly GitHubTestReleaseWorkflowWaiter _gitHubTestReleaseWorkflowWaiter;

        public TemplateRepositoryGenerator(Context context)
        {
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
                _gitHubRepositoryTemplateGenerator.Execute();
                _localLanguageFoldersCleaner.Execute();
                _localReadmeBadgeUpdater.Execute();
                _localDockerComposeUpdater.Execute();

                var commitMessage = "Template post-processing";
                ProcessExecutor.RunProcess("git", $"commit -m \"{commitMessage}\"");

                _gitHubPagesEnabler.Execute();
                _gitHubCommitPusher.Execute();

                _gitHubCommitWorkflowWaiter.Execute();
                _gitHubTestReleaseWorkflowWaiter.Execute();
            }
            finally
            {
                _localRepositoryDeleter.Execute();
            }
        }
    }
}
