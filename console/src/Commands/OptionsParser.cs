using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Commands
{
    internal class OptionsParser
    {
        internal static MonorepoOptions ParseMonorepoOptions(string[] args)
        {
            var options = new MonorepoOptions
            {
                OutputPath = Directory.GetCurrentDirectory()
            };

            for (int i = 1; i < args.Length; i += 2)
            {
                if (i + 1 >= args.Length) break;

                switch (args[i])
                {
                    case "--repository-name":
                        options.RepositoryName = args[i + 1];
                        break;
                    case "--system-language":
                        options.SystemLanguage = args[i + 1];
                        break;
                    case "--system-test-language":
                        options.SystemTestLanguage = args[i + 1];
                        break;
                    case "--output-path":
                        options.OutputPath = Path.GetFullPath(args[i + 1]);
                        break;
                }
            }

            return options;
        }
    }
}
