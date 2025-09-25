package com.optivem.atddaccelerator.templategenerator.systemtest.util;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.executeProcess;

public class GithubClient {

    private String repositoryPath;

    public GithubClient(String owner, String repoName) {
        this.repositoryPath = owner + "/" + repoName;
    }

    public String getRepositoryPath() {
        return repositoryPath;
    }

    public void viewRepository() {
        executeProcess("gh", "repo", "view", repositoryPath);
    }

    public void deleteRepository() {
        // executeProcess("gh", "auth", "refresh", "-h", "github.com", "-s", "delete_repo");
        executeProcess("gh", "repo", "delete", repositoryPath, "--confirm");
    }

    public static GithubClient createRandom(String owner) {
        var repoName = "repo-" + System.currentTimeMillis();
        return new GithubClient(owner, repoName);
    }
}
