using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;
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

        public void GenerateNewRepository(string repoName, Language systemLanguage, Language systemTestLanguage)
        {
            var result = _client.GenerateRepository(repoName, systemLanguage, systemTestLanguage);
            AssertSuccess(result, "Repository generation should succeed.");

            WaitTime(Millis);
            _created.Add(repoName);
        }

        public void GenerateNewRepositoryExpectError(string repoName, Language systemLanguage, Language systemTestLanguage)
        {
            var result = _client.GenerateRepository(repoName, systemLanguage, systemTestLanguage);
            AssertFailure(result, "Repository generation should fail.");
        }
    }
}