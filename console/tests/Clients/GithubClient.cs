using System.Net.Http;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process;
using static Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process.ProcessExecutor;
using static Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process.ProcessResultAssertions;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients
{
    public class GithubClient
    {
        private readonly string _owner;
        private readonly string _repoName;
        private readonly string _repositoryPath;

        public GithubClient(string owner, string repoName)
        {
            _owner = owner;
            _repoName = repoName;
            _repositoryPath = $"{owner}/{repoName}";
        }

        public string GetRepositoryPath() => _repositoryPath;

        public ProcessResult DeleteRepository()
        {
            return ExecuteProcess("gh", "repo", "delete", _repositoryPath, "--yes");
        }

        public ProcessResult ViewRepository()
        {
            return ExecuteProcess("gh", "repo", "view", _repositoryPath);
        }

        public ProcessResult ViewPath(string path)
        {
            return ExecuteProcess("gh", "api", $"/repos/{_repositoryPath}/contents/{path}");
        }

        public ProcessResult ViewPages()
        {
            return ExecuteProcess("gh", "api", $"/repos/{_repositoryPath}/pages");
        }

        public string GetFileContent(string filePath)
        {
            var result = ExecuteProcess("gh", "api",
                "-H", "Accept: application/vnd.github.raw",
                $"/repos/{_repositoryPath}/contents/{filePath}");
            AssertSuccess(result);
            return result.Output;
        }

        public ProcessResult ViewWorkflowRuns(string workflowFileName)
        {
            return ExecuteProcess("gh", "run", "list",
                "--workflow", workflowFileName,
                "--limit", "1",
                "--json", "status,conclusion",
                "--repo", _repositoryPath);
        }

        public bool PackageExists(string packageName)
        {
            var url = $"https://github.com/{_repositoryPath}/pkgs/container/{_repoName}/{packageName}";
            return UrlExists(url);
        }

        private bool UrlExists(string url)
        {
            using var client = new HttpClient();
            try
            {
                var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).Result;
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}