using Optivem.AtddAccelerator.TemplateGenerator.Core.Executors;
using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator;

public class GenerateMonorepo
{
    private readonly Context _context;

    private readonly GitHubRepositoryTemplateGenerator _gitHubRepositoryTemplateGenerator;
    private readonly GitHubCommitPusher _gitHubCommitPusher;
    private readonly LocalLanguageFoldersCleaner _localLanguageFoldersCleaner;
    private readonly LocalReadmeBadgeUpdater _localReadmeBadgeUpdater;
    private readonly LocalDockerComposeUpdater _localDockerComposeUpdater;
    private readonly GitHubPagesEnabler _gitHubPagesEnabler;
    private readonly LocalRepositoryDeleter _localRepositoryDeleter;

    public GenerateMonorepo(Context context)
    {
        _gitHubRepositoryTemplateGenerator = new GitHubRepositoryTemplateGenerator(context);
        _gitHubCommitPusher = new GitHubCommitPusher(context);
        _localLanguageFoldersCleaner = new LocalLanguageFoldersCleaner(context);
        _localReadmeBadgeUpdater = new LocalReadmeBadgeUpdater(context);
        _localDockerComposeUpdater = new LocalDockerComposeUpdater(context);
        _gitHubPagesEnabler = new GitHubPagesEnabler(context);
        _localRepositoryDeleter = new LocalRepositoryDeleter(context);
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


            // Optionally: Call workflow triggers if needed
            // InvokeBuildWorkflows.WaitForBuildWorkflows(...);
            // InvokeSystemTestReleaseWorkflows.InvokeSystemTestWorkflows(...);
        }
        catch (Exception ex)
        {
            // Clean up any partially created directories when the script fails
            _localRepositoryDeleter.Execute();
        }
    }
}