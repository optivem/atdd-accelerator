using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Executors
{
    internal class LocalRepositoryDeleter : BaseExecutor
    {
        public LocalRepositoryDeleter(Context context) : base(context)
        {
        }

        public override void Execute()
        {
            // TODO: VJ: Fix later, fails locally but passes on GitHub flows

            //if (Directory.Exists(_context.TargetDirectory))
            //{
            //    Directory.SetCurrentDirectory(Path.GetTempPath());
            //    Directory.Delete(_context.TargetDirectory, true);
            //}
        }
    }
}
