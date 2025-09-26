package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResult;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.*;

public class GeneratorClient {
    private static final String SCRIPT_PATH = "../scripts/setup-mono-repo.ps1";

    public ProcessResult generateRepository(String repoName, String systemLanguage) {
        return executeProcess("pwsh", SCRIPT_PATH, "-RepositoryName", repoName, "-SystemLanguage", systemLanguage);
    }

}
