using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Executors
{
    internal class GitHubTestReleaseWorkflowWaiter : BaseExecutor
    {
        public GitHubTestReleaseWorkflowWaiter(Context context) : base(context)
        {
        }

        public override void Execute()
        {
            // TODO: VJ
            //InvokeSystemTestWorkflows(_context.SystemTestLanguage.Stringify(), _context.RepositoryOwner, _context.RepositoryName);
        }

        private static bool InvokeSystemTestWorkflows(string systemTestLanguage, string repositoryOwner, string repositoryName)
        {
            Console.WriteLine($"Triggering system test workflows for language: {systemTestLanguage}");
            string[] workflows = {
            $"local-acceptance-stage-test-{systemTestLanguage.ToLower()}",
            $"acceptance-stage-test-{systemTestLanguage.ToLower()}",
            $"qa-stage-test-{systemTestLanguage.ToLower()}",
            $"prod-stage-test-{systemTestLanguage.ToLower()}"
        };

            int triggered = 0, failed = 0;
            foreach (var workflow in workflows)
            {
                Console.WriteLine($"Triggering workflow: {workflow}");
                var result = ProcessExecutor.RunProcess("gh", $"workflow run {workflow} --repo \"{repositoryOwner}/{repositoryName}\"");
                if (!string.IsNullOrWhiteSpace(result.Output))
                {
                    Console.WriteLine($"   Successfully triggered: {workflow}");
                    triggered++;
                }
                else
                {
                    Console.WriteLine($"   Failed to trigger: {workflow}");
                    failed++;
                }
                Task.Delay(500).Wait();
            }
            Console.WriteLine("\nWorkflow trigger summary:");
            Console.WriteLine($"   Successfully triggered ({triggered})");
            Console.WriteLine($"   Failed to trigger ({failed})");
            return triggered > 0;
        }
    }
}
