using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl.GitHub.Helpers;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl
{
    public class GitHubDsl
    {
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

        public void VerifyReadmeHasBadges(Language systemLanguage, Language systemTestLanguage)
        {
            _readmeClient.VerifyReadmeHasBadges(systemLanguage, systemTestLanguage);
        }

        public void VerifyPathsExist(Language systemLanguage, Language systemTestLanguage)
        {
            _fileClient.VerifyPathsExist(systemLanguage, systemTestLanguage);
        }

        public void VerifyWorkflowsPass(Language systemLanguage, Language systemTestLanguage)
        {
            _workflowClient.VerifyWorkflowsPass(systemLanguage, systemTestLanguage);
        }

        public void VerifyPagesEnabled()
        {
            _pagesClient.VerifyPagesEnabled();
        }

        public void VerifyDockerComposeImage(Language systemLanguage, Language systemTestLanguage)
        {
            _dockerComposeClient.VerifyDockerComposeImage(systemLanguage, systemTestLanguage);
        }

        public void DeleteRepository()
        {
            _repositoryClient.DeleteRepository();
        }

        public void VerifyRepositoryExists()
        {
            _repositoryClient.VerifyRepositoryExists();
        }

        public void VerifyPackagesExist(Language systemLanguage)
        {
            _packageClient.VerifyPackagesExist(systemLanguage);
        }
    }
}