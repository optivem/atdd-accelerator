package com.optivem.atddaccelerator.templategenerator.systemtest;

import com.optivem.atddaccelerator.templategenerator.systemtest.util.GithubClient;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.io.IOException;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.executeProcess;
import static org.assertj.core.api.Assertions.assertThat;

class SetupScriptTest {

    private static final String SCRIPT_PATH = "../scripts/setup-mono-repo.ps1";
    private static final String REPO_OWNER = "valentinajemuovic";

    private GithubClient githubClient;

    @BeforeEach
    void setup() {
        githubClient = new GithubClient();
    }

    @Test
    void setupScript_shouldReturnExitCode0() throws IOException, InterruptedException {
        executeProcess("pwsh", SCRIPT_PATH);
    }

    @Test
    void githubRepository_shouldExist_viaGhCli() throws IOException, InterruptedException {
        githubClient.viewRepository(REPO_OWNER, "flowers2");
    }
}