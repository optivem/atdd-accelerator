package com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertSuccess;

public class RepositoryClient {
    private final GithubClient client;
    private final String repositoryPath;

    public RepositoryClient(GithubClient client) {
        this.client = client;
        this.repositoryPath = client.getRepositoryPath();
    }

    public void verifyRepositoryExists() {
        var result = client.viewRepository();
        assertSuccess(result, "Repository '" + client.getRepositoryPath() + "' should exist.");
    }

    public void deleteRepository() {
        var result = client.deleteRepository();
        assertSuccess(result, "Failed to delete repository '" + client.getRepositoryPath() + "'.");
    }
}
