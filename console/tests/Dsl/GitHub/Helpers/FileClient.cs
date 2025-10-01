using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;
using static Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process.ProcessResultAssertions;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl.GitHub.Helpers
{
    public class FileClient
    {
        private readonly GithubClient _client;

        public FileClient(GithubClient client)
        {
            _client = client;
        }

        public void VerifyPathsExist(Language systemLanguage, Language systemTestLanguage)
        {
            VerifyPathLanguageExists("monolith-{0}", systemLanguage);
            VerifyPathLanguageExists(GetWorkflowPath(Constants.CommitStageMonolithFormat), systemLanguage);

            VerifyPathLanguageExists("system-test-{0}/docker-compose.yml", systemTestLanguage);
            VerifyPathLanguageExists(GetWorkflowPath(Constants.LocalAcceptanceStageTestFormat), systemTestLanguage);
            VerifyPathLanguageExists(GetWorkflowPath(Constants.AcceptanceStageTestFormat), systemTestLanguage);
            VerifyPathLanguageExists(GetWorkflowPath(Constants.QaStageTestFormat), systemTestLanguage);
            VerifyPathLanguageExists(GetWorkflowPath(Constants.ProdStageTestFormat), systemTestLanguage);
        }

        private void VerifyPathExists(string path)
        {
            var result = _client.ViewPath(path);
            AssertSuccess(result, $"Path '{path}' should exist.");
        }

        private void VerifyPathDoesNotExist(string path)
        {
            var result = _client.ViewPath(path);
            AssertFailure(result, $"Path '{path}' should not exist.");
        }

        private void VerifyPathLanguageExists(string pathFormat, Language language)
        {
            foreach (var l in LanguageExtensions.GetAll())
            {
                var path = string.Format(pathFormat, l.GetValue());
                if (l.Equals(language))
                {
                    VerifyPathExists(path);
                }
                else
                {
                    VerifyPathDoesNotExist(path);
                }
            }
        }

        private string GetWorkflowPath(string stageFormat)
        {
            return $".github/workflows/{stageFormat}.yml";
        }
    }
}