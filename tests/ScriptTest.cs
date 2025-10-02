using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests
{
    public class ScriptTest : IDisposable
    {
        private const string RepoOwner = "valentinajemuovic";

        private GeneratorDsl _generator;
        private GitHubDsl _gitHub;
        private string _repoName;

        public ScriptTest()
        {
            var generatorClient = new GeneratorClient();
            _generator = new GeneratorDsl(generatorClient);

            _repoName = NewName();
            var githubClient = new GithubClient(RepoOwner, _repoName);
            _gitHub = new GitHubDsl(githubClient);
        }

        public void Dispose()
        {
            var created = _generator.IsCreated(_repoName);

            if (created && _gitHub != null)
            {
                // _gitHub.DeleteRepository();
            }
        }

        public static IEnumerable<object[]> LanguageProvider()
        {
            return new List<object[]>
            {
                new object[] { Language.DotNet, Language.DotNet },
                new object[] { Language.DotNet, Language.Java },
                new object[] { Language.DotNet, Language.TypeScript },

                new object[] { Language.Java, Language.DotNet },
                new object[] { Language.Java, Language.Java },
                new object[] { Language.Java, Language.TypeScript },

                new object[] { Language.TypeScript, Language.DotNet },
                new object[] { Language.TypeScript, Language.Java },
                new object[] { Language.TypeScript, Language.TypeScript }
            };
        }

        [Theory]
        [MemberData(nameof(LanguageProvider))]
        public async Task ShouldCreateRepositoryWithLanguages(string systemLanguage, string systemTestLanguage)
        {
            await _generator.GenerateNewRepository(_repoName, systemLanguage, systemTestLanguage);

            _gitHub.VerifyRepositoryExists();
            _gitHub.VerifyPathsExist(systemLanguage, systemTestLanguage);
            _gitHub.VerifyDockerComposeImage(systemLanguage, systemTestLanguage);
            _gitHub.VerifyReadmeHasBadges(systemLanguage, systemTestLanguage);
            _gitHub.VerifyPagesEnabled();

            _gitHub.VerifyWorkflowsPass(systemLanguage, systemTestLanguage);
            // _gitHub.VerifyPackagesExist(systemLanguage);

            // TODO: VJ: Verify that only one package exists, rest are deleted

            // TODO: VJ: Rewrite like this
            // _gitHub.VerifyCommitStageSuccessful(systemLanguage); // -this checks only this path, the docker come, the readme badge, and that package was created
        }

        // TODO: Cannot create repository if name already exists
        // TODO: Error if gh not installed
        // TODO: Error if not logged into gh auth
        // TODO: Error if some mandatory parameter missing, or invalid, or empty
        // TODO: Error if no internet connection


        [Fact]
        public async Task QuickTest()
        {
            string systemLanguage = Language.Java;
            string systemTestLanguage = Language.TypeScript;

            await _generator.GenerateNewRepository(_repoName, systemLanguage, systemTestLanguage);

            _gitHub.VerifyRepositoryExists();
            _gitHub.VerifyPathsExist(systemLanguage, systemTestLanguage);
            _gitHub.VerifyDockerComposeImage(systemLanguage, systemTestLanguage);
            _gitHub.VerifyReadmeHasBadges(systemLanguage, systemTestLanguage);
            _gitHub.VerifyPagesEnabled();
        }

        [Fact]
        public async Task ShouldReturnErrorForInvalidSystemLanguage()
        {
            await _generator.GenerateNewRepositoryExpectError(_repoName, Language.None, Language.TypeScript);
        }

        private static string NewName()
        {
            var repoName = "repo-" + Guid.NewGuid().ToString();
            return repoName;
        }
    }
}