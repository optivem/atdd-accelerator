using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Executors
{
    internal class GitHubCommitWorkflowWaiter : BaseExecutor
    {
        private static int MaxAttempts = 50;
        private static int DelayMilliseconds = 30000;

        public GitHubCommitWorkflowWaiter(Context context) : base(context)
        {
        }

        public override void Execute()
        {
            VerifyCommitStagePasses();
            VerifyDockerImagesExist();
        }

        private void VerifyCommitStagePasses()
        {
            string workflowName = $"commit-stage-monolith-{_context.SystemLanguage.Stringify()}";
            
            for (int attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                var result = ProcessExecutor.RunProcess("gh", $"run list --repo \"{_context.RepositoryPath}\" --workflow \"{workflowName}\" --limit 1 --json status,conclusion,createdAt");
                
                if(result.IsError)
                {
                    throw CreateException(result, $"Failed to get workflow runs for '{workflowName}'");
                }

                // TODO: VJ: Parse JSON and check status/conclusion (use Newtonsoft.Json or System.Text.Json)
                // For brevity, assume success if output contains "completed" and "success"
                if (result.Output.Contains("completed"))
                {
                    if(!result.Output.Contains("success"))
                    {
                        throw CreateException(result, $"Workflow '{workflowName}' completed but failed");
                    }

                    return;
                }

                Task.Delay(DelayMilliseconds).Wait();
            }

            throw CreateException($"Max attempts ({MaxAttempts}) reached waiting for workflow '{workflowName}' to complete");
        }

        private void VerifyDockerImagesExist()
        {
            string imageName = $"ghcr.io/{_context.RepositoryPath}/monolith-{_context.SystemLanguage.Stringify()}:latest";
            var result = ProcessExecutor.RunProcess("docker", $"manifest inspect {imageName}");
            
            if (result.IsError || string.IsNullOrWhiteSpace(result.Output))
            {
                throw CreateException(result, $"Docker image '{imageName}' does not exist or could not be found");
            }
        }
    }
}
