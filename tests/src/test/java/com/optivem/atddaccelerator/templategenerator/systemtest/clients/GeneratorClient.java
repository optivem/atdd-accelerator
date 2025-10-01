package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessResult;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessExecutor.*;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessResultAssertions.assertSuccess;

public class GeneratorClient {
    private static final String CLI_DLL_PATH = "../cli/bin/Release/net8.0/Optivem.AtddAccelerator.TemplateGenerator.dll";
    
    public ProcessResult generateRepository(String repoName, Language systemLanguage, Language systemTestLanguage) {
        ProcessResult result = executeProcess("dotnet", CLI_DLL_PATH, "generate", "monorepo",
            "--repository-name", repoName, 
            "--system-language", systemLanguage.getValue(),  // This is sending "none" somehow
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

        assertSuccess(result, "Repository generation process should complete successfully.");

        return result;
    }
}