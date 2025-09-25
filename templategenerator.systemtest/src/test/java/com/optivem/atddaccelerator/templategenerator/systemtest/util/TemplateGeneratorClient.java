package com.optivem.atddaccelerator.templategenerator.systemtest.util;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.executeProcess;

public class TemplateGeneratorClient {
    private static final String SCRIPT_PATH = "../scripts/setup-mono-repo.ps1";

    public void generateNewRepository(String repoName) {
        executeProcess("pwsh", SCRIPT_PATH, "-RepositoryName", repoName);
    }

}
