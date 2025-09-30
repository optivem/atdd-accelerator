package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessResult;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessExecutor.*;
import static org.assertj.core.api.Assertions.assertThat;

public class GeneratorClient {
    private static final String CLI_DLL_PATH = "../cli/bin/Release/net8.0/Optivem.AtddAccelerator.TemplateGenerator.dll";
    
    public void buildGenerator() {
        // Clean the project first - use the .csproj file path directly
        ProcessResult cleanResult = executeProcess("dotnet", "clean", "../cli/Optivem.AtddAccelerator.TemplateGenerator.csproj");

        assertThat(cleanResult.isSuccess())
                .withFailMessage(() -> "Failed to clean the project:\n" + cleanResult.getErrors())
                .isTrue();
        
        // Build the project - use the .csproj file path directly
        var buildResult = executeProcess("dotnet", "build", "../cli/Optivem.AtddAccelerator.TemplateGenerator.csproj", "--configuration", "Release");

        assertThat(buildResult.isSuccess())
                .withFailMessage(() -> "Failed to build the project:\n" + buildResult.getErrors())
                .isTrue();
    }
    
    public void generateRepository(String repoName, Language systemLanguage, Language systemTestLanguage) {
        var result = executeProcess("dotnet", CLI_DLL_PATH, "generate", "monorepo",
            "--repository-name", repoName, 
            "--system-language", systemLanguage.getValue(), 
            "--system-test-language", systemTestLanguage.getValue());

        assertThat(result.isSuccess())
            .withFailMessage(() -> "Failed to generate repository:\n" + result.getErrors())
            .isTrue();
    }
}