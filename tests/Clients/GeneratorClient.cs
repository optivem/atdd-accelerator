using Optivem.AtddAccelerator.TemplateGenerator.Presentation;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients
{
    public class GeneratorClient
    {
        public Task<ProcessResult> GenerateRepositoryAsync(string repositoryOwner, string repositoryName, string systemLanguage, string systemTestLanguage)
        {
            var args = new[]
            {
                "generate",
                "monorepo",
                "--repository-owner", repositoryOwner,
                "--repository-name", repositoryName,
                "--system-language", systemLanguage,
                "--system-test-language", systemTestLanguage,

            };
            
            var commandArgs = new List<string>
            {
                "dotnet",
                "run",
                "--no-launch-profile",
                "--project", GetProjectPath(),
                "--"
            };
            commandArgs.AddRange(args);
            
            var result = ProcessExecutor.ExecuteProcess(commandArgs.ToArray());
            
            return Task.FromResult(result);
        }

        private string GetProjectPath()
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var solutionDir = Directory.GetParent(currentDir)?.Parent?.Parent?.Parent?.Parent?.FullName;
            if (solutionDir == null)
                throw new InvalidOperationException("Could not find solution directory");
            return Path.Combine(solutionDir, "src", "Optivem.AtddAccelerator.TemplateGenerator.csproj");
        }
    }
}