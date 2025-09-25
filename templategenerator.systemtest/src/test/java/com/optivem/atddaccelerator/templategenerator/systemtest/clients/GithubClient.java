package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.*;
import static org.assertj.core.api.Assertions.assertThat;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class GithubClient {

    private String repositoryPath;

    public GithubClient(String owner, String repoName) {
        this.repositoryPath = owner + "/" + repoName;
    }

    public static GithubClient createRandom(String owner) {
        var repoName = "repo-" + System.currentTimeMillis();
        return new GithubClient(owner, repoName);
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

    public void verifyPathExists(String path) {
        executeProcessExpectSuccess("gh", "api", "/repos/" + repositoryPath + "/contents/" + path);
    }

    public void verifyPathDoesNotExist(String path) {
        executeProcessExpectError("gh", "api", "/repos/" + repositoryPath + "/contents/" + path);
    }
}
