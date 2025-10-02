using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Presentation.Commands
{
    internal class OptionsValidator
    {
        // TODO: VJ: Should come from enum
        private static readonly HashSet<string> ValidLanguages = new HashSet<string> { "java", "dotnet", "typescript" };

        internal static int Validate(Options options)
        {
            var isValidRepositoryName = ValidateRepositoryName(options);
            var isValidSystemLanguage = ValidateSystemLanguage(options);
            var isValidSystemTestLanguage = ValidateSystemTestLanguage(options);
            var isValidRepositoryOwner = ValidateRepositoryOwner(options);

            var success = isValidRepositoryName && isValidSystemLanguage && isValidSystemTestLanguage && isValidRepositoryOwner;

            return success ? 0 : 1;
        }

        private static bool ValidateRepositoryName(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.RepositoryName))
            {
                Console.Error.WriteLine("Error: --repository-name is empty.");
                return false;
            }
            return true;
        }

        private static bool ValidateSystemLanguage(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.SystemLanguage))
            {
                Console.Error.WriteLine("Error: --system-language is empty.");
                return false;
            }

            if(!ValidLanguages.Contains(options.SystemLanguage))
            {
                Console.Error.WriteLine($"Error: --system-language '{options.SystemLanguage}' is invalid. Valid options: {string.Join(", ", ValidLanguages)}");
                return false;
            }

            return true;
        }


        private static bool ValidateSystemTestLanguage(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.SystemTestLanguage))
            {
                Console.Error.WriteLine("Error: --system-test-language is empty.");
                return false;
            }

            if(!ValidLanguages.Contains(options.SystemTestLanguage))
            {
                Console.Error.WriteLine($"Error: --system-test-language: '{options.SystemTestLanguage}' is invalid. Valid options: {string.Join(", ", ValidLanguages)}");
                return false;
            }

            return true;
        }

        private static bool ValidateRepositoryOwner(Options options)
        {
            var username = options.RepositoryOwner;
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.Error.WriteLine("Error: --repository-owner is required.");
                return false;
            }

            // Basic GitHub username format check
            if (username.Length < 1 || username.Length > 39 ||
                username.StartsWith("-") || username.EndsWith("-") ||
                username.Contains("--") ||
                !username.All(c => char.IsLetterOrDigit(c) || c == '-'))
            {
                Console.Error.WriteLine("Error: --repository-owner is invalid. It must be 1-39 characters, alphanumeric or hyphens, cannot start/end with hyphen, and no consecutive hyphens.");
                return false;
            }

            return true;
        }
    }
}
