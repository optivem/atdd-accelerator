using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process;
using static Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process.ProcessExecutor;
using static Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process.ProcessResultAssertions;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients
{
    public class GeneratorClient
    {
        private static readonly string ConsoleDllPath = Path.GetFullPath(
            @"..\..\..\..\console\bin\Release\net8.0\Optivem.AtddAccelerator.TemplateGenerator.dll",
            AppContext.BaseDirectory
        );

        public ProcessResult GenerateRepository(string repoName, Language systemLanguage, Language systemTestLanguage)
        {
            var result = ExecuteProcess("dotnet", ConsoleDllPath, "generate", "monorepo",
                "--repository-name", repoName,
                "--system-language", systemLanguage.GetValue(),  // This is sending "none" somehow
                "--system-test-language", systemTestLanguage.GetValue());

            // Debug output for failed generation
            Console.WriteLine("Generation Result:");
            Console.WriteLine($"  Exit Code: {result.ExitCode}");
            Console.WriteLine($"  Output: {result.Output}");
            Console.WriteLine($"  Errors: {result.Errors}");

            if (result.ExitCode != 0)
            {
                Console.Error.WriteLine("❌ GENERATION FAILED!");
                Console.Error.WriteLine($"Exit code: {result.ExitCode}");
                Console.Error.WriteLine($"Output: {result.Output}");
                Console.Error.WriteLine($"Errors: {result.Errors}");
                Console.Error.WriteLine($"Command executed: dotnet {ConsoleDllPath} generate monorepo --repository-name {repoName} --system-language {systemLanguage.GetValue()} --system-test-language {systemTestLanguage.GetValue()}");
            }

            AssertSuccess(result, "Repository generation process should complete successfully.");

            return result;
        }
    }
}