using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Executors
{
    internal class LocalReadmeBadgeUpdater : BaseExecutor
    {
        private static readonly string ReadmeFilePath = "README.md";

        public LocalReadmeBadgeUpdater(Context context) : base(context)
        {
        }

        public override void Execute()
        {
            if (!File.Exists(ReadmeFilePath))
            {
                throw new FileNotFoundException("README.md not found in the current directory.");
            }
            var originalContent = File.ReadAllText(ReadmeFilePath);
            var updatedContent = GetUpdatedContent(originalContent);
            File.WriteAllText(ReadmeFilePath, updatedContent);

            AddToStaging();
        }

        private string GetUpdatedContent(string originalContent)
        {
            var readmeContent = originalContent;

            var badgesToRemove = new List<string>();

            foreach (var language in LanguageExtensions.GetAll())
            {
                if (language != _context.SystemLanguage)
                {
                    var languageString = language.Stringify();
                    badgesToRemove.Add($"commit-stage-monolith-{languageString}");
                }
            }
            foreach (var badge in badgesToRemove)
            {
                readmeContent = Regex.Replace(readmeContent, $@".*\[!\[{badge}\].*(?:\r?\n)?", "", RegexOptions.Multiline);
            }
            return readmeContent.Replace("optivem/atdd-accelerator-template-mono-repo", $"{_context.RepositoryPath}");
        }

        private void AddToStaging()
        {
            ProcessExecutor.RunProcess("git", "add README.md");
        }
    }
}
