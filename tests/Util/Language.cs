namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util
{

    public static class Language
    {
        public const string None = "none";
        public const string Java = "java";
        public const string DotNet = "dotnet";
        public const string TypeScript = "typescript";
    }

    public static class LanguageExtensions
    {
        public static string[] GetAll()
        {
            return new[] { Language.Java, Language.DotNet, Language.TypeScript };
        }
    }
}