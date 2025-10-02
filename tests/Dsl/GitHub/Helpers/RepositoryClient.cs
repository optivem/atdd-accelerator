using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using static Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process.ProcessResultAssertions;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl.GitHub.Helpers
{
    public class RepositoryClient
    {
        private readonly GithubClient _client;
        private readonly string _repositoryPath;

        public RepositoryClient(GithubClient client)
        {
            _client = client;
            _repositoryPath = client.GetRepositoryPath();
        }

        public void VerifyRepositoryExists()
        {
            var result = _client.ViewRepository();
            result.ShouldSucceed($"Repository '{_client.GetRepositoryPath()}' should exist.");
        }

        public void DeleteRepository()
        {
            var result = _client.DeleteRepository();
            result.ShouldSucceed($"Failed to delete repository '{_client.GetRepositoryPath()}'.");
        }
    }
}