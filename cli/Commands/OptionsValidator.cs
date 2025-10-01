using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Commands
{
    internal class OptionsValidator
    {
        private static readonly HashSet<string> ValidLanguages = new HashSet<string> { "java", "dotnet", "typescript" };

        internal static int Validate(MonorepoOptions options)
        {
            var success = ValidateRepositoryName(options) == 1
                && ValidateSystemLanguage(options) == 1
                && ValidateSystemTestLanguage(options) == 1;

            return success ? 0 : 1;
        }

        private static int ValidateRepositoryName(MonorepoOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.RepositoryName))
            {
                Console.WriteLine("Error: --repository-name is required.");
                return 1;
            }
            return 0;
        }

        private static int ValidateSystemLanguage(MonorepoOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.SystemLanguage))
            {
                Console.WriteLine("Error: --system-language is required.");
                return 1;
            }

            if(!ValidLanguages.Contains(options.SystemLanguage))
            {
                Console.WriteLine($"Invalid --system-language: '{options.SystemLanguage}'. Valid options: {string.Join(", ", ValidLanguages)}");
                return 1;
            }

            return 0;
        }


        private static int ValidateSystemTestLanguage(MonorepoOptions options)
        {
            if (string.IsNullOrEmpty(options.SystemTestLanguage))
            {
                Console.WriteLine("Error: --system-test-language is required.");
                return 1;
            }

            return 0;
        }
    }
}
