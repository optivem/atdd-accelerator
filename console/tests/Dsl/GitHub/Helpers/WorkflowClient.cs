using System.Text.Json;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;
using static Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process.ProcessResultAssertions;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Dsl.GitHub.Helpers
{
    public class WorkflowClient
    {
        private readonly GithubClient _client;
        private readonly string _repositoryPath;

        public WorkflowClient(GithubClient client)
        {
            _client = client;
            _repositoryPath = client.GetRepositoryPath();
        }

        public void VerifyWorkflowsPass(Language systemLanguage, Language systemTestLanguage)
        {
            VerifyWorkflowPasses(Constants.PagesBuilderDeployment);
            VerifyWorkflowPasses(Constants.CommitStageMonolithFormat, systemLanguage);
            VerifyWorkflowPasses(Constants.LocalAcceptanceStageTestFormat, systemTestLanguage);
            VerifyWorkflowPasses(Constants.AcceptanceStageTestFormat, systemTestLanguage);
            VerifyWorkflowPasses(Constants.QaStageTestFormat, systemTestLanguage);
            VerifyWorkflowPasses(Constants.ProdStageTestFormat, systemTestLanguage);
        }

        private void VerifyWorkflowPasses(string workflowFileName)
        {
            var workflowRun = WaitUntilCompleted(workflowFileName);

            Assert.Equal("success", workflowRun.Conclusion);
        }

        private void VerifyWorkflowPasses(string workflowFileNameFormat, Language language)
        {
            var workflowFileName = string.Format(workflowFileNameFormat, language.GetValue());
            VerifyWorkflowPasses(workflowFileName);
        }

        private WorkflowRunResult WaitUntilCompleted(string workflowFileName)
        {
            const int maxRetries = 10;
            const int baseDelayMs = 1000; // Start with 1 second
            const int maxDelayMs = 300000; // Max 5 minutes
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                var workflowRun = GetWorkflowRunResult(workflowFileName);
                
                if (workflowRun.Status == "completed")
                {
                    return workflowRun;
                }

                if (attempt < maxRetries)
                {
                    var delay = Math.Min(baseDelayMs * (int)Math.Pow(2, attempt - 1), maxDelayMs);
                    Console.WriteLine($"Workflow: {workflowFileName} Attempt {attempt}: Workflow status is '{workflowRun.Status}', retrying in {delay}ms...");
                    Thread.Sleep(delay);
                }
                else
                {
                    Console.WriteLine($"Status: {workflowRun.Status}, max retries reached.");
                }
            }

            throw new TimeoutException($"Workflow '{workflowFileName}' did not complete within the expected time.");
        }

        private WorkflowRunResult GetWorkflowRunResult(string workflowFileName)
        {
            var result = _client.ViewWorkflowRuns(workflowFileName);
            AssertSuccess(result, $"Failed to get workflow runs for '{workflowFileName}'.");

            var jsonOutput = result.Output;
            var workflowRuns = ParseWorkflowRuns(jsonOutput);
            return workflowRuns.First();
        }

        private List<WorkflowRunResult> ParseWorkflowRuns(string jsonOutput)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                
                return JsonSerializer.Deserialize<List<WorkflowRunResult>>(jsonOutput, options) 
                       ?? new List<WorkflowRunResult>();
            }
            catch (JsonException e)
            {
                throw new InvalidOperationException("Failed to parse workflow runs JSON", e);
            }
        }
    }
}