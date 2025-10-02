using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests
{
    public class ScriptTest : IDisposable
    {
        private const string RepositoryOwner = "valentinajemuovic";

        private GeneratorDsl _generator;
        private GitHubDsl _gitHub;
        private string _repositoryName;

        public ScriptTest()
        {
            var generatorClient = new GeneratorClient();
            _generator = new GeneratorDsl(generatorClient);

            _repositoryName = NewRepositoryName();
            var githubClient = new GithubClient(RepositoryOwner, _repositoryName);
            _gitHub = new GitHubDsl(githubClient);
        }

        public void Dispose()
        {
            var created = _generator.IsCreated(_repositoryName);

            if (created && _gitHub != null)
            {
                _gitHub.DeleteRepository();
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
            await _generator.GenerateNewRepository(RepositoryOwner, _repositoryName, systemLanguage, systemTestLanguage);

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

            await _generator.GenerateNewRepository(RepositoryOwner, _repositoryName, systemLanguage, systemTestLanguage);

            _gitHub.VerifyRepositoryExists();
            _gitHub.VerifyPathsExist(systemLanguage, systemTestLanguage);
            _gitHub.VerifyDockerComposeImage(systemLanguage, systemTestLanguage);
            _gitHub.VerifyReadmeHasBadges(systemLanguage, systemTestLanguage);
            _gitHub.VerifyPagesEnabled();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldReturnErrorForEmptyRepositoryOwner(string repositoryOwner)
        {
            await _generator.GenerateNewRepositoryExpectError(repositoryOwner, _repositoryName, Language.Java, Language.TypeScript, "Error: --repository-owner is empty.");
        }


        [Theory]
        [InlineData("non-existent-repo-owner-12345")]
        [InlineData("non-existent-repo-owner-67890")]
        public async Task ShouldReturnErrorForNonExistentRepositoryOwner(string nonExistentRepositoryOwner)
        {
            await _generator.GenerateNewRepositoryExpectError(nonExistentRepositoryOwner, _repositoryName, Language.Java, Language.TypeScript, $"Error: --repository-owner '{nonExistentRepositoryOwner}' does not exist on GitHub.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldReturnErrorForEmptyRepositoryName(string repositoryName)
        {
            await _generator.GenerateNewRepositoryExpectError(RepositoryOwner, repositoryName, Language.Java, Language.TypeScript, "Error: --repository-name is empty.");
        }

        [Fact]
        public async Task ShouldReturnErrorForDuplicateRepositoryName()
        {
            await _generator.GenerateNewRepository(RepositoryOwner, _repositoryName, Language.Java, Language.TypeScript);
            await _generator.GenerateNewRepositoryExpectError(RepositoryOwner, _repositoryName, Language.Java, Language.TypeScript, $"Error: --repository-name '{_repositoryName}' already exists.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldReturnErrorForEmptySystemLanguage(string invalidSystemLanguage)
        {
            await _generator.GenerateNewRepositoryExpectError(RepositoryOwner, _repositoryName, invalidSystemLanguage, Language.TypeScript, "Error: --system-language is empty.");
        }

        [Theory]
        [InlineData("Java")]
        [InlineData("hello")]
        public async Task ShouldReturnErrorForInvalidSystemLanguage(string invalidSystemLanguage)
        {
            await _generator.GenerateNewRepositoryExpectError(RepositoryOwner, _repositoryName, invalidSystemLanguage, Language.TypeScript, $"Error: --system-language '{invalidSystemLanguage}' is invalid. Valid options: java, dotnet, typescript");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldReturnErrorForEmptySystemTestLanguage(string invalidSystemTestLanguage)
        {
            await _generator.GenerateNewRepositoryExpectError(RepositoryOwner, _repositoryName, Language.Java, invalidSystemTestLanguage, "Error: --system-test-language is empty.");
        }

        [Theory]
        [InlineData("Java")]
        [InlineData("hello")]
        public async Task ShouldReturnErrorForInvalidSystemTestLanguage(string invalidSystemTestLanguage)
        {
            await _generator.GenerateNewRepositoryExpectError(RepositoryOwner, _repositoryName, Language.Java, invalidSystemTestLanguage, $"Error: --system-test-language: '{invalidSystemTestLanguage}' is invalid. Valid options: java, dotnet, typescript");
        }




        private static string NewRepositoryName()
        {
            var repoName = "repo-" + Guid.NewGuid().ToString();
            return repoName;
        }
    }
}