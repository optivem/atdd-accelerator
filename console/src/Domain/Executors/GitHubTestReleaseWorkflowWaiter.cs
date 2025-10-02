using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Executors
{
    internal class GitHubTestReleaseWorkflowWaiter : BaseExecutor
    {
        private static int DelayMillis = 500;

        public GitHubTestReleaseWorkflowWaiter(Context context) : base(context)
        {
        }

        public override void Execute()
        {
            var workflows = GetWorkflows();

            foreach (var workflow in workflows)
            {
                ExecuteWorkflow(workflow);
                Task.Delay(DelayMillis).Wait();
            }
        }

        private string[] GetWorkflows()
        {
            var systemLanguageString = _context.SystemLanguage.Stringify();

            return [
                $"local-acceptance-stage-test-{systemLanguageString}",
                $"acceptance-stage-test-{systemLanguageString}",
                $"qa-stage-test-{systemLanguageString}",
                $"prod-stage-test-{systemLanguageString}"
            ];
        }

        private void ExecuteWorkflow(string workflow)
        {
            var result = ProcessExecutor.RunProcess("gh", $"workflow run {workflow} --repo \"{_context.RepositoryPath}\"");

            if (result.IsError || string.IsNullOrWhiteSpace(result.Output))
            {
                throw CreateException(result, $"Failed to trigger: {workflow}");
            }
        }
    }
}
