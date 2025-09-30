package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessResult;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessExecutor.*;

public class GeneratorClient {
    private static final String SCRIPT_PATH = "../scripts/setup-mono-repo.ps1";

    public ProcessResult generateRepository(String repoName, Language systemLanguage, Language systemTestLanguage) {
        return executeProcess("pwsh", SCRIPT_PATH, "-RepositoryName", repoName, "-SystemLanguage", systemLanguage.getValue(), "-SystemTestLanguage", systemTestLanguage.getValue());
    }

}
