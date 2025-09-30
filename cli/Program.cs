using System;
using System.Reflection;

namespace Optivem.AtddAccelerator.TemplateGenerator;

class Program
{
    static void Main(string[] args)
    {
        // Check for version argument
        if (args.Length > 0 && (args[0] == "--version" || args[0] == "-v"))
        {
            ShowVersion();
            return;
        }

        // Check for help argument
        if (args.Length > 0 && (args[0] == "--help" || args[0] == "-h"))
        {
            ShowHelp();
            return;
        }

        // Default Hello World behavior
        Console.WriteLine("Hello, World!");
        Console.WriteLine("ATDD Accelerator Template Generator");
        Console.WriteLine("Use --help for usage information or --version for version info");
    }

    static void ShowVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "Unknown";
        var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? version;
        
        Console.WriteLine($"ATDD Accelerator Template Generator");
        Console.WriteLine($"Version: {informationalVersion}");
        Console.WriteLine($"Assembly Version: {version}");
        Console.WriteLine();
        Console.WriteLine("Copyright (c) Optivem");
        Console.WriteLine("Licensed under MIT License");
        Console.WriteLine("Repository: https://github.com/optivem/atdd-accelerator");
    }

    static void ShowHelp()
    {
        Console.WriteLine("ATDD Accelerator Template Generator");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  atdd [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --version, -v    Show version information");
        Console.WriteLine("  --help, -h       Show help information");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  atdd --version   Display version");
        Console.WriteLine("  atdd --help      Show this help");
        Console.WriteLine();
        Console.WriteLine("For more information, visit:");
        Console.WriteLine("https://github.com/optivem/atdd-accelerator");
    }
}