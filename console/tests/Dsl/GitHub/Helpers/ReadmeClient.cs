using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;
using FluentAssertions;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl.GitHub.Helpers
{
    public class ReadmeClient
    {
        private const string ReadmePath = "README.md";

        private readonly GithubClient _client;
        private readonly string _repositoryPath;

        public ReadmeClient(GithubClient client)
        {
            _client = client;
            _repositoryPath = client.GetRepositoryPath();
        }

        public void VerifyReadmeHasBadges(Language systemLanguage, Language systemTestLanguage)
        {
            VerifyReadmePagesBadge();
            VerifyReadmeStageLanguageBadge(Constants.CommitStageMonolithFormat, systemLanguage);
            VerifyReadmeStageLanguageBadge(Constants.LocalAcceptanceStageTestFormat, systemTestLanguage);
            VerifyReadmeStageLanguageBadge(Constants.AcceptanceStageTestFormat, systemTestLanguage);
            VerifyReadmeStageLanguageBadge(Constants.QaStageTestFormat, systemTestLanguage);
            VerifyReadmeStageLanguageBadge(Constants.ProdStageTestFormat, systemTestLanguage);
        }

        private void VerifyReadmePagesBadge()
        {
            var badgeWorkflow = string.Format(Constants.PagesBuilderDeploymentWorkflowFormat, _repositoryPath);
            var badgeSvg = string.Format(Constants.PagesBuilderDeploymentWorkflowImageFormat, _repositoryPath);
            VerifyReadmeContainsBadge(Constants.PagesBuilderDeployment, badgeWorkflow, badgeSvg);
        }

        private void VerifyReadmeStageLanguageBadge(string workflowNameFormat, Language language)
        {
            foreach (var l in LanguageExtensions.GetAll())
            {
                var workflowName = string.Format(workflowNameFormat, l.GetValue());
                if (l.Equals(language))
                {
                    VerifyReadmeContainsBadge(workflowName);
                }
                else
                {
                    VerifyReadmeDoesNotContainBadge(workflowName);
                }
            }
        }

        private void VerifyReadmeContainsBadge(string workflowName)
        {
            var badgeWorkflow = string.Format(Constants.StageWorkflowFormat, _repositoryPath, workflowName);
            var badgeSvg = string.Format(Constants.StageWorkflowImageFormat, _repositoryPath, workflowName);

            VerifyReadmeContainsBadge(workflowName, badgeWorkflow, badgeSvg);
        }

        private void VerifyReadmeContainsBadge(string badgeName, string badgeWorkflow, string badgeSvg)
        {
            var readmeContent = GetReadmeContent();

            readmeContent.Should().Contain(badgeName, $"Expected README to contain badge name '{badgeName}', but it was not found.");
            readmeContent.Should().Contain(badgeWorkflow, $"Expected README to contain badge workflow '{badgeWorkflow}', but it was not found.");
            readmeContent.Should().Contain(badgeSvg, $"Expected README to contain badge SVG '{badgeSvg}', but it was not found.");
        }

        private void VerifyReadmeDoesNotContainBadge(string badge)
        {
            var readmeContent = GetReadmeContent();
            readmeContent.Should().NotContain(badge, $"Expected README to NOT contain badge '{badge}', but it was found.");
        }

        private string GetReadmeContent()
        {
            return _client.GetFileContent(ReadmePath);
        }
    }
}