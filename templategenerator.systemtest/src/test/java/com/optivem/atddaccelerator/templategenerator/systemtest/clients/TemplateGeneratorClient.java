package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.executeProcessExpectSuccess;
import static org.assertj.core.api.Assertions.assertThat;

public class TemplateGeneratorClient {
    private static final String SCRIPT_PATH = "../scripts/setup-mono-repo.ps1";

    public void generateNewRepository(String repoName, String java) {
        var result = executeProcessExpectSuccess("pwsh", SCRIPT_PATH, "-RepositoryName", repoName);
    }

}
