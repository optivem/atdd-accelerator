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
            var isValidRepositoryName = ValidateRepositoryName(options);
            var isValidSystemLanguage = ValidateSystemLanguage(options);
            var isValidSystemTestLanguage = ValidateSystemTestLanguage(options);
            var isValidGitHubUsername = ValidateGitHubUsername(options);

            var success = isValidRepositoryName && isValidSystemLanguage && isValidSystemTestLanguage && isValidGitHubUsername;

            return success ? 0 : 1;
        }

        private static bool ValidateRepositoryName(MonorepoOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.RepositoryName))
            {
                Console.Error.WriteLine("Error: --repository-name is required.");
                return false;
            }
            return true;
        }

        private static bool ValidateSystemLanguage(MonorepoOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.SystemLanguage))
            {
                Console.Error.WriteLine("Error: --system-language is required.");
                return false;
            }

            if(!ValidLanguages.Contains(options.SystemLanguage))
            {
                Console.Error.WriteLine($"Invalid --system-language: '{options.SystemLanguage}'. Valid options: {string.Join(", ", ValidLanguages)}");
                return false;
            }

            return true;
        }


        private static bool ValidateSystemTestLanguage(MonorepoOptions options)
        {
            if (string.IsNullOrEmpty(options.SystemTestLanguage))
            {
                Console.Error.WriteLine("Error: --system-test-language is required.");
                return false;
            }

            return true;
        }

        private static bool ValidateGitHubUsername(MonorepoOptions options)
        {
            var username = options.GitHubUsername;
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.Error.WriteLine("Error: --github-username is required.");
                return false;
            }

            // Basic GitHub username format check
            if (username.Length < 1 || username.Length > 39 ||
                username.StartsWith("-") || username.EndsWith("-") ||
                username.Contains("--") ||
                !username.All(c => char.IsLetterOrDigit(c) || c == '-'))
            {
                Console.Error.WriteLine("Error: --github-username is invalid. It must be 1-39 characters, alphanumeric or hyphens, cannot start/end with hyphen, and no consecutive hyphens.");
                return false;
            }

            return true;
        }
    }
}
