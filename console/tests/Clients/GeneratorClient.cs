using Microsoft.VisualStudio.TestPlatform.TestHost;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process;
using static Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process.ProcessExecutor;
using static Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process.ProcessResultAssertions;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients
{
    public class GeneratorClient
    {

        public async Task<ProcessResult> GenerateRepositoryAsync(string repoName, Language systemLanguage, Language systemTestLanguage)
        {
            var args = new[]
            {
                "generate",
                "monorepo",
                "--repository-name", repoName,
                "--system-language", systemLanguage.GetValue(),
                "--system-test-language", systemTestLanguage.GetValue()
            };

            var originalOut = Console.Out;
            var originalErr = Console.Error;
            var outputWriter = new StringWriter();
            var errorWriter = new StringWriter();

            Console.SetOut(outputWriter);
            Console.SetError(errorWriter);

            int exitCode = 0;
            try
            {
                await Generator.Main(args); // If Main returns int, use exitCode = Program.Main(args);
            }
            catch (Exception ex)
            {
                exitCode = 1;
                errorWriter.WriteLine(ex.ToString());
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetError(originalErr);
            }

            var output = outputWriter.ToString();
            var errors = errorWriter.ToString();

            return new ProcessResult(exitCode, output, errors);
        }
    }


    //    private static readonly string CliDllPath = @"c:\GitHub\optivem\atdd-accelerator\console\src\bin\Release\net8.0\Optivem.AtddAccelerator.TemplateGenerator.dll";

    //    public ProcessResult GenerateRepository(string repoName, Language systemLanguage, Language systemTestLanguage)
    //    {
    //        var result = ExecuteProcess("dotnet", CliDllPath, "generate", "monorepo",
    //            "--repository-name", repoName,
    //            "--system-language", systemLanguage.GetValue(),  // This is sending "none" somehow
    //            "--system-test-language", systemTestLanguage.GetValue());

    //        // Debug output for failed generation
    //        Console.WriteLine("Generation Result:");
    //        Console.WriteLine($"  Exit Code: {result.ExitCode}");
    //        Console.WriteLine($"  Output: {result.Output}");
    //        Console.WriteLine($"  Errors: {result.Errors}");

    //        if (result.ExitCode != 0)
    //        {
    //            Console.Error.WriteLine("❌ GENERATION FAILED!");
    //            Console.Error.WriteLine($"Exit code: {result.ExitCode}");
    //            Console.Error.WriteLine($"Output: {result.Output}");
    //            Console.Error.WriteLine($"Errors: {result.Errors}");
    //            Console.Error.WriteLine($"Command executed: dotnet {CliDllPath} generate monorepo --repository-name {repoName} --system-language {systemLanguage.GetValue()} --system-test-language {systemTestLanguage.GetValue()}");
    //        }

    //        AssertSuccess(result, "Repository generation process should complete successfully.");

    //        return result;
    //    }
    //}
}