using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Domain.Utilities
{
    public static class CommonConstants
    {
        public static readonly string TemplatePath = "optivem/atdd-accelerator-template";

        public static readonly string AuthCommand = "gh auth login --web --git-protocol https --scopes \"repo,workflow,read:org\"";
    }
}
