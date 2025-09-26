package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import java.util.HashSet;
import java.util.Set;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.*;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertFailure;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertSuccess;
import static org.assertj.core.api.Assertions.assertThat;

public class TemplateGeneratorClient {
    private static final int MILLIS = 3000;
    private static final String SCRIPT_PATH = "../scripts/setup-mono-repo.ps1";

    private Set<String> created;

    public TemplateGeneratorClient() {
        this.created = new HashSet<>();
    }

    public boolean isCreated(String repoName) {
        return created.contains(repoName);
    }

    public void generateNewRepository(String repoName, String systemLanguage) {
        var result = executeProcess("pwsh", SCRIPT_PATH, "-RepositoryName", repoName, "-SystemLanguage", systemLanguage);
        assertSuccess(result);

        waitTime(MILLIS);
        created.add(repoName);
    }

    public void generateNewRepositoryExpectError(String repoName, String systemLanguage) {
        var result = executeProcess("pwsh", SCRIPT_PATH, "-RepositoryName", repoName, "-SystemLanguage", systemLanguage);
        assertFailure(result);
    }

}
