using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions;
using Optivem.AtddAccelerator.TemplateGenerator.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Executors
{
    internal class LocalDockerComposeUpdater : BaseExecutor
    {
        public LocalDockerComposeUpdater(Context context) : base(context)
        {
        }

        public override void Execute()
        {
            var systemTestFolders = Directory.GetDirectories(Directory.GetCurrentDirectory(), "system-test-*");
            if (systemTestFolders.Length == 0)
            {
                throw CreateException("No system-test folder found - cannot update Docker Compose files");
            }
            if (systemTestFolders.Length > 1)
            {
                throw CreateException("Multiple system-test folders found: " + string.Join(", ", systemTestFolders));
            }
            var systemTestFolder = systemTestFolders[0];
            var systemTestLanguage = systemTestFolder.Split('-').Last().ToLower();

            var templatePath = Path.Combine("temp", _context.SystemLanguage.Stringify(), "docker-compose.yml");
            if (!File.Exists(templatePath))
            {
                var fullPath = Path.GetFullPath(templatePath);
                throw CreateException($"Template Docker Compose file not found: {templatePath} with full path {templatePath}");
            }
            var targetDockerCompose = Path.Combine(systemTestFolder, "docker-compose.yml");
            if (!File.Exists(targetDockerCompose))
            {
                throw CreateException($"Docker Compose file not found in system-test folder: {targetDockerCompose}");
            }
            var templateContent = File.ReadAllText(templatePath);
            var updatedContent = templateContent
                .Replace($"ghcr.io/{TemplateConstants.TemplatePath}", $"ghcr.io/{_context.RepositoryPath}")
                .Replace($"monolith-{systemTestLanguage}", $"monolith-{_context.SystemLanguage.Stringify()}");

            var currentContent = File.ReadAllText(targetDockerCompose);

            File.WriteAllText(targetDockerCompose, updatedContent);

            AddToStaging(targetDockerCompose);
        }

        private void AddToStaging(string targetDockerCompose)
        {
            ProcessExecutor.RunProcess("git", $"add {targetDockerCompose}");
        }
    }
}
