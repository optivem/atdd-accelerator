using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Presentation.Commands
{
    public class OptionsValidator
    {
        private readonly ILogger<OptionsValidator> _logger;

        // TODO: VJ: Should come from enum
        private static readonly HashSet<string> ValidLanguages = new HashSet<string> { "java", "dotnet", "typescript" };

        public OptionsValidator(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OptionsValidator>();
        }

        public int Validate(Options options)
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

        private bool ValidateRepositoryOwner(Options options)
        {
            var repositoryOwner = options.RepositoryOwner;
            if (string.IsNullOrWhiteSpace(repositoryOwner))
            {
                _logger.LogError("Error: --repository-owner is empty.");
                return false;
            }

            return true;
        }

        private bool ValidateRepositoryName(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.RepositoryName))
            {
                _logger.LogError("Error: --repository-name is empty.");
                return false;
            }

            var lowercaseRepositoryName = options.RepositoryName.ToLowerInvariant();

            // Ensure repository name is all lowercase
            if (options.RepositoryName != lowercaseRepositoryName)
            {
                _logger.LogError("Error: --repository-name '{RepositoryName}' is not lowercase. Please use only lowercase letters, numbers, and hyphens (due to compatibility with Docker repositories).", options.RepositoryName);
                return false;
            }

            return true;
        }

        private bool ValidateSystemLanguage(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.SystemLanguage))
            {
                _logger.LogError("Error: --system-language is empty.");
                return false;
            }

            if(!ValidLanguages.Contains(options.SystemLanguage))
            {
                _logger.LogError("Error: --system-language '{SystemLanguage}' is invalid. Valid options: {ValidOptions}", options.SystemLanguage, string.Join(", ", ValidLanguages));
                return false;
            }

            return true;
        }

        private bool ValidateSystemTestLanguage(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.SystemTestLanguage))
            {
                _logger.LogError("Error: --system-test-language is empty.");
                return false;
            }

            if(!ValidLanguages.Contains(options.SystemTestLanguage))
            {
                _logger.LogError("Error: --system-test-language: '{SystemTestLanguage}' is invalid. Valid options: {ValidOptions}", options.SystemTestLanguage, string.Join(", ", ValidLanguages));
                return false;
            }

            return true;
        }
    }
}
