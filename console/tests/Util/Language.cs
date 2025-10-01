namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util
{
    public enum Language
    {
        None,
        Java,
        DotNet,
        TypeScript
    }

    public static class LanguageExtensions
    {
        public static string GetValue(this Language language)
        {
            return language switch
            {
                Language.None => "none",
                Language.Java => "java",
                Language.DotNet => "dotnet",
                Language.TypeScript => "typescript",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            };
        }

        public static Language[] GetAll()
        {
            return new[] { Language.Java, Language.DotNet, Language.TypeScript };
        }
    }
}