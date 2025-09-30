package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessResult;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessExecutor.*;

public class GeneratorClient {
    private static final String CLI_DLL_PATH = "../cli/bin/Release/net8.0/Optivem.AtddAccelerator.TemplateGenerator.dll";
    
    public ProcessResult buildGenerator() {
        System.out.println("=== STARTING BUILD PROCESS ===");
        
        // Clean the project first - use the .csproj file path directly
        System.out.println("Step 1: Cleaning project...");
        ProcessResult cleanResult = executeProcess("dotnet", "clean", "../cli/Optivem.AtddAccelerator.TemplateGenerator.csproj");
        
        System.out.println("Clean Result:");
        System.out.println("  Exit Code: " + cleanResult.getExitCode());
        System.out.println("  Output: " + cleanResult.getOutput());
        System.out.println("  Errors: " + cleanResult.getErrors());
        
        if (cleanResult.getExitCode() != 0) {
            System.err.println("❌ CLEAN FAILED!");
            System.err.println("Exit code: " + cleanResult.getExitCode());
            System.err.println("Output: " + cleanResult.getOutput());
            System.err.println("Errors: " + cleanResult.getErrors());
            throw new RuntimeException("Failed to clean the project: " + cleanResult.getErrors());
        }
        
        System.out.println("✅ Clean completed successfully");
        
        // Build the project - use the .csproj file path directly
        System.out.println("Step 2: Building project...");
        ProcessResult buildResult = executeProcess("dotnet", "build", "../cli/Optivem.AtddAccelerator.TemplateGenerator.csproj", "--configuration", "Release");
        
        System.out.println("Build Result:");
        System.out.println("  Exit Code: " + buildResult.getExitCode());
        System.out.println("  Output: " + buildResult.getOutput());
        System.out.println("  Errors: " + buildResult.getErrors());
        
        if (buildResult.getExitCode() != 0) {
            System.err.println("❌ BUILD FAILED!");
            System.err.println("Exit code: " + buildResult.getExitCode());
            System.err.println("Output: " + buildResult.getOutput());
            System.err.println("Errors: " + buildResult.getErrors());
            throw new RuntimeException("Failed to build the project: " + buildResult.getErrors());
        }
        
        System.out.println("✅ Build completed successfully");
        
        // Verify the DLL was created
        System.out.println("Step 3: Verifying build output...");
        try {
            java.io.File dllFile = new java.io.File(CLI_DLL_PATH);
            System.out.println("DLL Path: " + dllFile.getAbsolutePath());
            System.out.println("DLL Exists: " + dllFile.exists());
            
            if (dllFile.exists()) {
                System.out.println("DLL Size: " + dllFile.length() + " bytes");
                System.out.println("DLL Last Modified: " + new java.util.Date(dllFile.lastModified()));
            }
            
            // Check scripts directory
            java.io.File scriptsDir = new java.io.File("../cli/bin/Release/net8.0/scripts");
            System.out.println("Scripts Directory: " + scriptsDir.getAbsolutePath());
            System.out.println("Scripts Directory Exists: " + scriptsDir.exists());
            
            if (scriptsDir.exists()) {
                java.io.File[] scriptFiles = scriptsDir.listFiles();
                System.out.println("Script Files Count: " + (scriptFiles != null ? scriptFiles.length : 0));
                if (scriptFiles != null) {
                    for (java.io.File scriptFile : scriptFiles) {
                        System.out.println("  - " + scriptFile.getName());
                    }
                }
            }
            
        } catch (Exception e) {
            System.err.println("Error verifying build output: " + e.getMessage());
        }
        
        System.out.println("=== BUILD PROCESS COMPLETED ===");
        return buildResult;
    }
    
    public ProcessResult generateRepository(String repoName, Language systemLanguage, Language systemTestLanguage) {
        System.out.println("=== STARTING GENERATION PROCESS ===");
        System.out.println("Repository Name: " + repoName);
        System.out.println("System Language: " + systemLanguage.getValue());
        System.out.println("System Test Language: " + systemTestLanguage.getValue());
        System.out.println("DLL Path: " + CLI_DLL_PATH);
        
        ProcessResult result = executeProcess("dotnet", CLI_DLL_PATH, "generate", "monorepo",
            "--repository-name", repoName, 
            "--system-language", systemLanguage.getValue(), 
            "--system-test-language", systemTestLanguage.getValue());
        
        // Debug output for failed generation
        System.out.println("Generation Result:");
        System.out.println("  Exit Code: " + result.getExitCode());
        System.out.println("  Output: " + result.getOutput());
        System.out.println("  Errors: " + result.getErrors());
        
        if (result.getExitCode() != 0) {
            System.err.println("❌ GENERATION FAILED!");
            System.err.println("Exit code: " + result.getExitCode());
            System.err.println("Output: " + result.getOutput());
            System.err.println("Errors: " + result.getErrors());
            System.err.println("Command executed: dotnet " + CLI_DLL_PATH + " generate monorepo --repository-name " + repoName + " --system-language " + systemLanguage.getValue() + " --system-test-language " + systemTestLanguage.getValue());
        }
        
        System.out.println("=== GENERATION PROCESS COMPLETED ===");
        return result;
    }
}