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


        public GitHubRepositoryTemplateGenerator(Context context, ProcessExecutor processExecutor) : base(context, processExecutor)
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
                var result = _processExecutor.RunProcess("gh", $"repo create {_context.RepositoryName} --template {CommonConstants.TemplatePath} --public --clone");

                if (result.IsError)
                {
                    if (result.Errors.Contains("already exists") || result.Output.Contains("already exists"))
                    {
                        throw CreateException($"Repository name '{_context.RepositoryName}' already exists on this GitHub account. Please choose a different name.");
                    }
                    else if(result.Errors.Contains("failed to run git: exit status 128"))
                    {
                        throw CreateException($"Repository '{_context.RepositoryName}' was created at https://github.com/{_context.RepositoryOwner}/{_context.RepositoryName}, but cloning failed. This may be due to SSH authentication issue. Please re-authenticate with HTTPS using '{CommonConstants.AuthCommand}'");
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
