using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Commands
{
    internal class OptionsConverter
    {
        public static Context Convert(MonorepoOptions options)
        {
            var repository = new Repository(options.RepositoryName, options.GitHubUsername);
            var systemLanguage = ParseLanguage(options.SystemLanguage);
            var systemTestLanguage = ParseLanguage(options.SystemTestLanguage);
            var outputPath = GetOutputDirectory(options.RepositoryName); // TODO: VJ: Allow user input

            return new Context(repository, systemLanguage, systemTestLanguage, outputPath);
        }

        private static Language ParseLanguage(string language)
        {
            return (Language)Enum.Parse(typeof(Language), language);
        }

        private static string GetOutputDirectory(string repositoryName)
        {
            var tempDir = Path.GetTempPath();
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var atddTempDir = Path.Combine(tempDir, $"ATDD-Accelerator-{uniqueId}");
            var targetDirectory = Path.Combine(atddTempDir, repositoryName);
            Directory.CreateDirectory(atddTempDir);
            Console.WriteLine($"Using temp directory: {targetDirectory}");
            return targetDirectory;
        }
    }
}
