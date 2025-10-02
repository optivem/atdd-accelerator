using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities
{
    public enum Language
    {
        None,
        DotNet,
        Java,
        TypeScript
    }

    public static class LanguageExtensions
    {
        public static string Stringify(this Language language)
        {
            return language switch
            {
                Language.DotNet => "dotnet",
                Language.Java => "java",
                Language.TypeScript => "typescript",
                Language.None => "none",
                _ => "none"
            };
        }

        public static IEnumerable<Language> GetAll()
        {
            return (Language[])Enum.GetValues(typeof(Language));
        }
    }
}
