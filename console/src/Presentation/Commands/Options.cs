using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Presentation.Commands
{
    public class Options
    {
        public string RepositoryName { get; set; } = "";
        public string SystemLanguage { get; set; } = "";
        public string SystemTestLanguage { get; set; } = "";
        public string OutputPath { get; set; } = "";
        public string GitHubUsername { get; set; } = "";
    }
}
