using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl.GitHub.Helpers;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl
{
    public class GitHubDsl
    {

        public void VerifyRepositoryExists()
        {
            _repositoryClient.VerifyRepositoryExists();
        }
        private readonly ReadmeClient _readmeClient;
        private readonly FileClient _fileClient;
        private readonly WorkflowClient _workflowClient;
        private readonly PagesClient _pagesClient;
        private readonly DockerComposeClient _dockerComposeClient;
        private readonly RepositoryClient _repositoryClient;
        private readonly PackageClient _packageClient;

        public GitHubDsl(GithubClient client)
        {
            _readmeClient = new ReadmeClient(client);
            _fileClient = new FileClient(client);
            _workflowClient = new WorkflowClient(client);
            _pagesClient = new PagesClient(client);
            _dockerComposeClient = new DockerComposeClient(client);
            _repositoryClient = new RepositoryClient(client);
            _packageClient = new PackageClient(client);
        }

        public void VerifyReadmeHasBadges(string systemLanguage, string systemTestLanguage)
        {
            _readmeClient.VerifyReadmeHasBadges(systemLanguage, systemTestLanguage);
        }

        public void VerifyPathsExist(string systemLanguage, string systemTestLanguage)
        {
            _fileClient.VerifyPathsExist(systemLanguage, systemTestLanguage);
        }

        public void VerifyWorkflowsPass(string systemLanguage, string systemTestLanguage)
        {
            _workflowClient.VerifyWorkflowsPass(systemLanguage, systemTestLanguage);
        }

        public void VerifyPagesEnabled()
        {
            _pagesClient.VerifyPagesEnabled();
        }

        public void VerifyDockerComposeImage(string systemLanguage, string systemTestLanguage)
        {
            _dockerComposeClient.VerifyDockerComposeImage(systemLanguage, systemTestLanguage);
        }

        public void VerifyPackagesExist(string systemLanguage)
        {
            _packageClient.VerifyPackagesExist(systemLanguage);
        }
    }
}