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
            var isValidRepositoryOwner = ValidateRepositoryOwner(options);
            var isValidRepositoryName = ValidateRepositoryName(options);
            var isValidSystemLanguage = ValidateSystemLanguage(options);
            var isValidSystemTestLanguage = ValidateSystemTestLanguage(options);


            var success = isValidRepositoryOwner 
                && isValidRepositoryName 
                && isValidSystemLanguage 
                && isValidSystemTestLanguage;

            return success ? 0 : 1;
        }


        private static bool ValidateRepositoryOwner(Options options)
        {
            var repositoryOwner = options.RepositoryOwner;
            if (string.IsNullOrWhiteSpace(repositoryOwner))
            {
                Console.Error.WriteLine("Error: --repository-owner is empty.");
                return false;
            }

            return true;
        }

        private static bool ValidateRepositoryName(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.RepositoryName))
            {
                Console.Error.WriteLine("Error: --repository-name is empty.");
                return false;
            }

            var lowercaseRepositoryName = options.RepositoryName.ToLowerInvariant();

            // Ensure repository name is all lowercase
            if (options.RepositoryName != lowercaseRepositoryName)
            {
                Console.Error.WriteLine($"Error: --repository-name '{options.RepositoryName}' is not lowercase. Please use only lowercase letters, numbers, and hyphens (due to compatibility with Docker repositories).");
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

    }
}
