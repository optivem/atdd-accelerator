using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities
{
    public class Context
    {
        public Context(Repository repository, Language systemLanguage, Language systemTestLanguage, string outputPath)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            SystemLanguage = systemLanguage;
            SystemTestLanguage = systemTestLanguage;
            OutputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));
        }

        public Repository Repository { get; }
        public string RepositoryName => Repository.Name;
        public string RepositoryOwner => Repository.Owner;
        public Language SystemLanguage { get; }
        public Language SystemTestLanguage { get; }
        public string OutputPath { get; }
        public string TargetDirectory => OutputPath; // For backward compatibility

        public object RepositoryPath => Repository.Path;

        public override string ToString()
        {   var sb = new StringBuilder();
            sb.AppendLine($"Repository Name: {RepositoryName}");
            sb.AppendLine($"Repository Owner: {RepositoryOwner}");
            sb.AppendLine($"System Language: {SystemLanguage}");
            sb.AppendLine($"System Test Language: {SystemTestLanguage}");
            sb.AppendLine($"Output Path: {OutputPath}");
            return sb.ToString();
        }
    }
}
