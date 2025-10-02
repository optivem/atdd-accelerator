using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;
using Shouldly;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl.GitHub.Helpers
{
    public class DockerComposeClient
    {
        private readonly GithubClient _client;
        private readonly string _repositoryPath;

        public DockerComposeClient(GithubClient client)
        {
            _client = client;
            _repositoryPath = client.GetRepositoryPath();
        }

        public void VerifyDockerComposeImage(Language systemLanguage, Language systemTestLanguage)
        {
            var dockerComposePath = $"system-test-{systemTestLanguage.GetValue()}/docker-compose.yml";

            foreach (var l in LanguageExtensions.GetAll())
            {
                var monolithDockerImageName = string.Format(Constants.MonolithDockerImageNameFormat, _repositoryPath, l.GetValue());
                if (l.Equals(systemLanguage))
                {
                    VerifyDockerComposeContainsImage(dockerComposePath, monolithDockerImageName);
                }
                else
                {
                    VerifyDockerComposeDoesNotContainImage(dockerComposePath, monolithDockerImageName);
                }
            }
        }

        private void VerifyDockerComposeContainsImage(string dockerComposePath, string image)
        {
            var dockerComposeContent = _client.GetFileContent(dockerComposePath);
            dockerComposeContent.ShouldContain(image, Case.Insensitive, $"Docker Compose should contain image: {image}");
        }

        private void VerifyDockerComposeDoesNotContainImage(string dockerComposePath, string image)
        {
            var dockerComposeContent = _client.GetFileContent(dockerComposePath);
            dockerComposeContent.ShouldNotContain(image, Case.Insensitive, $"Docker Compose should not contain image: {image}");
        }
    }
}