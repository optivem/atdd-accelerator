using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;

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

            Assert.Contains(badgeName, readmeContent);
            Assert.Contains(badgeWorkflow, readmeContent);
            Assert.Contains(badgeSvg, readmeContent);
        }

        private void VerifyReadmeDoesNotContainBadge(string badge)
        {
            var readmeContent = GetReadmeContent();
            Assert.DoesNotContain(badge, readmeContent);
        }

        private string GetReadmeContent()
        {
            return _client.GetFileContent(ReadmePath);
        }
    }
}