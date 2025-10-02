using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process;
using FluentAssertions;
using System.Text.Json;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl.GitHub.Helpers
{
    public class PagesClient
    {
        private readonly GithubClient _client;

        public PagesClient(GithubClient client)
        {
            _client = client;
        }

        public void VerifyPagesEnabled()
        {
            var result = _client.ViewPages();
            ProcessResultAssertions.ShouldSucceed(result, "GitHub Pages should be enabled.");
            VerifyPagesSourceIsMainDocs();
        }

        private void VerifyPagesSourceIsMainDocs()
        {
            var result = _client.ViewPages();
            ProcessResultAssertions.ShouldSucceed(result, "Failed to get GitHub Pages info.");

            var json = result.Output;
            using var doc = JsonDocument.Parse(json);
            var source = doc.RootElement.GetProperty("source");
            var branch = source.GetProperty("branch").GetString();
            var path = source.GetProperty("path").GetString();

            branch.Should().Be("main", "GitHub Pages source branch should be 'main'");
            path.Should().Be("/docs", "GitHub Pages source path should be '/docs'");
        }
    }
}