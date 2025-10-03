using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions;
using Optivem.AtddAccelerator.TemplateGenerator.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Executors
{
    internal class GitHubRepositoryTemplateGenerator : BaseExecutor
    {
        public GitHubRepositoryTemplateGenerator(Context context) : base(context)
        {
        }

        public override void Execute()
        {
            var parentDir = Directory.GetParent(_context.OutputPath).FullName;
            Directory.CreateDirectory(parentDir);

            if (Directory.Exists(_context.OutputPath))
            {
                Directory.Delete(_context.OutputPath, true);
            }

            var originalLocation = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(parentDir);

            try
            {
                var result = ProcessExecutor.RunProcess("gh", $"repo create {_context.RepositoryName} --template {TemplateConstants.TemplatePath} --public --clone");

                if (result.IsError)
                {
                    if (result.Errors.Contains("already exists") || result.Output.Contains("already exists"))
                    {
                        throw new Exception($"Repository name '{_context.RepositoryName}' already exists on this GitHub account. Please choose a different name.");
                    }
                    else
                    {
                        throw CreateException(result, $"Failed to create repository {_context.RepositoryName}");
                    }
                }

                var clonedDir = Path.Combine(parentDir, _context.RepositoryName);
                if (clonedDir != _context.OutputPath)
                    Directory.Move(clonedDir, _context.OutputPath);
                Directory.SetCurrentDirectory(_context.OutputPath);
                return;

            }
            catch(Exception ex)
            {
                Directory.SetCurrentDirectory(originalLocation);
                throw;
            }
        }
    }
}
