package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.executeProcess;
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
        var result = executeProcess("gh", "repo", "view", repositoryPath);
        assertTrue(result.isSuccess());
    }

    public void deleteRepository() {
        var result = executeProcess("gh", "repo", "delete", repositoryPath, "--yes");
        assertTrue(result.isSuccess());
    }

    public void verifyFolderExists(String folderPath) {
        var result = executeProcess( "gh", "api", "/repos/" + repositoryPath + "/contents");
        assertTrue(result.isSuccess());
    }

    public static GithubClient createRandom(String owner) {
        var repoName = "repo-" + System.currentTimeMillis();
        return new GithubClient(owner, repoName);
    }
}
