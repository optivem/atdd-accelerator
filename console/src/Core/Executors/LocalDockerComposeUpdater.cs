using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
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
                throw new ExecutionException(_context, "No system-test folder found - cannot update Docker Compose files");
            }
            if (systemTestFolders.Length > 1)
            {
                throw new ExecutionException(_context, "Multiple system-test folders found: " + string.Join(", ", systemTestFolders));
            }
            var systemTestFolder = systemTestFolders[0];
            var systemTestLanguage = systemTestFolder.Split('-').Last().ToLower();

            var templatePath = Path.Combine("temp", _context.SystemLanguage.ToString(), "docker-compose.yml");
            if (!File.Exists(templatePath))
            {
                throw new ExecutionException(_context, $"Template Docker Compose file not found: {templatePath}");
            }
            var targetDockerCompose = Path.Combine(systemTestFolder, "docker-compose.yml");
            if (!File.Exists(targetDockerCompose))
            {
                throw new ExecutionException(_context, $"Docker Compose file not found in system-test folder: {targetDockerCompose}");
            }
            var templateContent = File.ReadAllText(templatePath);
            var updatedContent = templateContent
                .Replace("ghcr.io/optivem/atdd-accelerator-template-mono-repo/", $"ghcr.io/{_context.RepositoryPath}/")
                .Replace($"monolith-{systemTestLanguage}", $"monolith-{_context.SystemLanguage.ToString()}");

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
