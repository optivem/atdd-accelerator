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
        public LocalReadmeBadgeUpdater(Context context) : base(context)
        {
        }

        public override void Execute()
        {
            if (!File.Exists("README.md"))
            {
                throw new FileNotFoundException("README.md not found in the current directory.");
            }
            var readmeContent = File.ReadAllText("README.md");
            var originalContent = readmeContent;

            var badgesToRemove = new System.Collections.Generic.List<string>();
            switch (_context.SystemLanguage)
            {
                case Language.Java:
                    badgesToRemove.Add("commit-stage-monolith-dotnet");
                    badgesToRemove.Add("commit-stage-monolith-typescript");
                    break;
                case Language.DotNet:
                    badgesToRemove.Add("commit-stage-monolith-java");
                    badgesToRemove.Add("commit-stage-monolith-typescript");
                    break;
                case Language.TypeScript:
                    badgesToRemove.Add("commit-stage-monolith-java");
                    badgesToRemove.Add("commit-stage-monolith-dotnet");
                    break;
            }

            foreach (var lang in new[] { "java", "dotnet", "typescript" })
            {
                if (lang != _context.SystemTestLanguage.ToString())
                {
                    badgesToRemove.Add($"local-acceptance-stage-test-{lang}");
                    badgesToRemove.Add($"acceptance-stage-test-{lang}");
                    badgesToRemove.Add($"qa-stage-test-{lang}");
                    badgesToRemove.Add($"prod-stage-test-{lang}");
                }
            }
            foreach (var badge in badgesToRemove)
            {
                readmeContent = Regex.Replace(readmeContent, $@".*\[!\[{badge}\].*(?:\r?\n)?", "", RegexOptions.Multiline);
            }
            readmeContent = readmeContent.Replace("optivem/atdd-accelerator-template-mono-repo", $"{_context.RepositoryPath}");

            AddToStaging();
        }

        private void AddToStaging()
        {
            ProcessExecutor.RunProcess("git", "add README.md");
        }
    }
}
