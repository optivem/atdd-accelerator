using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Executors
{
    internal class LocalLanguageFoldersCleaner : BaseExecutor
    {
        // TODO: VJ: Add to enum
        private static readonly string[] Languages = { "java", "dotnet", "typescript" };

        public LocalLanguageFoldersCleaner(Context context, ProcessExecutor processExecutor) : base(context, processExecutor)
        {
        }

        public override void Execute()
        {
            var workingDirectory = Directory.GetCurrentDirectory();

            // System
            RemoveItemsByTemplate("monolith-{language}", _context.SystemLanguage, true);
            RemoveItemsByTemplate(".github/workflows/commit-stage-monolith-{language}.yml", _context.SystemLanguage, false);

            // System Language
            RemoveItemsByTemplate("system-test-{language}", _context.SystemTestLanguage, true);
            RemoveItemsByTemplate(".github/workflows/local-acceptance-stage-test-{language}.yml", _context.SystemTestLanguage, false);
            RemoveItemsByTemplate(".github/workflows/acceptance-stage-test-{language}.yml", _context.SystemTestLanguage, false);
            RemoveItemsByTemplate(".github/workflows/qa-stage-test-{language}.yml", _context.SystemTestLanguage, false);
            RemoveItemsByTemplate(".github/workflows/prod-stage-test-{language}.yml", _context.SystemTestLanguage, false);
        }

        private class LanguageItems
        {
            public List<string> AllItems { get; set; } = new();
            public Dictionary<string, string> LanguageToItemMapping { get; set; } = new();
        }

        private static LanguageItems GetLanguageItems(string pathTemplate)
        {
            var allItems = new List<string>();
            var languageToItemMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var language in Languages)
            {
                var item = pathTemplate.Replace("{language}", language);
                allItems.Add(item);
                languageToItemMapping[language] = item;
            }

            return new LanguageItems
            {
                AllItems = allItems,
                LanguageToItemMapping = languageToItemMapping
            };
        }

        private void RemoveLanguageSpecificItems(
            Language language,
            List<string> allItems,
            Dictionary<string, string> languageToItemMapping,
            bool isFolder)
        {
            var keepItem = languageToItemMapping[language.Stringify()];

            foreach (var item in allItems)
            {
                if (!string.Equals(item, keepItem))
                {

                    if (isFolder && Directory.Exists(item))
                    {
                        Directory.Delete(item, true);
                        _processExecutor.RunProcess("git", $"rm -r {item}");
                    }
                    else if (File.Exists(item))
                    {
                        File.Delete(item);
                        _processExecutor.RunProcess("git", $"rm {item}");
                    }
                }
            }
        }

        private void RemoveItemsByTemplate(string pathTemplate, Language language, bool isFolder)
        {
            var items = GetLanguageItems(pathTemplate);

            RemoveLanguageSpecificItems(language, items.AllItems, items.LanguageToItemMapping, isFolder);
        }


    }
}
