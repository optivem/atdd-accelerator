package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.executeProcess;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.executeProcessExpectSuccess;
import static org.assertj.core.api.Assertions.assertThat;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class GithubClient {

    private String repositoryPath;

    public GithubClient(String owner, String repoName) {
        this.repositoryPath = owner + "/" + repoName;
    }

    public String getRepositoryPath() {
        return repositoryPath;
    }

    public void verifyRepositoryExists() {
        executeProcessExpectSuccess("gh", "repo", "view", repositoryPath);
    }

    public void deleteRepository() {
        executeProcessExpectSuccess("gh", "repo", "delete", repositoryPath, "--yes");
    }

    public void verifyFolderExists(String folderName) {
        var result = executeProcessExpectSuccess( "gh", "api", "/repos/" + repositoryPath + "/contents");

        assertThat(result.getOutput()).contains("\"name\":\"README.md\"");
        assertThat(result.getOutput()).contains("\"name\":\"monolith-java\"");
        assertThat(result.getOutput()).contains("\"name\":\"" + folderName + "\"");
    }

    public static GithubClient createRandom(String owner) {
        var repoName = "repo-" + System.currentTimeMillis();
        return new GithubClient(owner, repoName);
    }
}
