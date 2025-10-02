using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;
using Shouldly;
using static Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process.ProcessExecutor;
using static Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process.ProcessResultAssertions;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl
{
    public class GeneratorDsl
    {
        private const int Millis = 3000;

        private readonly HashSet<string> _created;
        private readonly GeneratorClient _client;

        public GeneratorDsl(GeneratorClient client)
        {
            _client = client;
            _created = new HashSet<string>();
        }

        public bool IsCreated(string repoName)
        {
            return _created.Contains(repoName);
        }

        public async Task GenerateNewRepository(string repositoryOwner, string repositoryName, string systemLanguage, string systemTestLanguage)
        {
            var result = await _client.GenerateRepositoryAsync(repositoryOwner, repositoryName, systemLanguage, systemTestLanguage);
            result.ShouldSucceed("Repository generation should succeed.");

            WaitTime(Millis);
            _created.Add(repositoryName);
        }

        public async Task GenerateNewRepositoryExpectError(string repositoryOwner, string repositoryName, string systemLanguage, string systemTestLanguage, string expectedErrorMessage)
        {
            var result = await _client.GenerateRepositoryAsync(repositoryOwner, repositoryName, systemLanguage, systemTestLanguage);
            result.ShouldFail("Repository generation should fail.");
            result.Errors.ShouldContain(expectedErrorMessage, Case.Insensitive);
        }
    }
}