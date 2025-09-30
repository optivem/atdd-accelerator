package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessResult;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessExecutor.*;

public class GeneratorClient {
    private static final String CLI_DLL_PATH = "../cli/bin/Release/net8.0/Optivem.AtddAccelerator.TemplateGenerator.dll";
    
    public ProcessResult buildGenerator() {
        // Clean the project first - use the .csproj file path directly
        ProcessResult cleanResult = executeProcess("dotnet", "clean", "../cli/Optivem.AtddAccelerator.TemplateGenerator.csproj");
        
        if (cleanResult.getExitCode() != 0) {
            return cleanResult; // Return clean failure immediately
        }
        
        // Build the project - use the .csproj file path directly
        return executeProcess("dotnet", "build", "../cli/Optivem.AtddAccelerator.TemplateGenerator.csproj", "--configuration", "Release");
    }
    
    public ProcessResult generateRepository(String repoName, Language systemLanguage, Language systemTestLanguage) {
        return executeProcess("dotnet", CLI_DLL_PATH, "generate", "monorepo",
            "--repository-name", repoName, 
            "--system-language", systemLanguage.getValue(), 
            "--system-test-language", systemTestLanguage.getValue());
    }
}