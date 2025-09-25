package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.executeProcessExpectSuccess;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.waitTime;
import static org.assertj.core.api.Assertions.assertThat;

public class TemplateGeneratorClient {
    public static final int MILLIS = 3000;
    private static final String SCRIPT_PATH = "../scripts/setup-mono-repo.ps1";

    public void generateNewRepository(String repoName, String systemLanguage) {
        executeProcessExpectSuccess("pwsh", SCRIPT_PATH, "-RepositoryName", repoName, "-SystemLanguage", systemLanguage);
        waitTime(MILLIS);
    }
}
