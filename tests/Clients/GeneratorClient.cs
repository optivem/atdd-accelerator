using Optivem.AtddAccelerator.TemplateGenerator.Presentation;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util;
using Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Clients
{
    public class GeneratorClient
    {
        public async Task<ProcessResult> GenerateRepositoryAsync(string repositoryOwner, string repositoryName, string systemLanguage, string systemTestLanguage)
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

            var originalOut = Console.Out;
            var originalErr = Console.Error;
            var outputWriter = new StringWriter();
            var errorWriter = new StringWriter();

            Console.SetOut(outputWriter);
            Console.SetError(errorWriter);

            int exitCode = 0;
            try
            {
                exitCode = await GeneratorProgram.Main(args);
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
}