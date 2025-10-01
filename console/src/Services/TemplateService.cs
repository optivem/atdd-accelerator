using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator;

public class TemplateService
{
    public async Task GenerateAsync(Context context)
    {
        var generator = new GenerateMonorepo(context);
        await generator.GenerateAsync();
    }
}