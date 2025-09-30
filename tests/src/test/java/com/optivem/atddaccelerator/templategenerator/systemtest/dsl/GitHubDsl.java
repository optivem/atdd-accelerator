package com.optivem.atddaccelerator.templategenerator.systemtest.dsl;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers.*;

public class GitHubDsl {
    private final ReadmeClient readmeClient;
    private final FileClient fileClient;
    private final WorkflowClient workflowClient;
    private final PagesClient pagesClient;
    private final DockerComposeClient dockerComposeClient;
    private final RepositoryClient repositoryClient;

    public GitHubDsl(GithubClient client) {
        this.readmeClient = new ReadmeClient(client);
        this.fileClient = new FileClient(client);
        this.workflowClient = new WorkflowClient(client);
        this.pagesClient = new PagesClient(client);
        this.dockerComposeClient = new DockerComposeClient(client);
        this.repositoryClient = new RepositoryClient(client);
    }

    public void verifyReadmeHasBadges(String systemLanguage, String systemTestLanguage) {
        readmeClient.verifyReadmeHasBadges(systemLanguage, systemTestLanguage);
    }

    public void verifyPathsExist(String systemLanguage, String systemTestLanguage) {
        fileClient.verifyPathsExist(systemLanguage, systemTestLanguage);
    }

    public void verifyWorkflowsPass(String systemLanguage, String systemTestLanguage) {
        workflowClient.verifyWorkflowsPass(systemLanguage, systemTestLanguage);
    }

    public void verifyPagesEnabled() {
        pagesClient.verifyPagesEnabled();
    }

    public void verifyDockerComposeImage(String systemLanguage, String systemTestLanguage) {
        dockerComposeClient.verifyDockerComposeImage(systemLanguage, systemTestLanguage);
    }

    public void deleteRepository() {
        repositoryClient.deleteRepository();
    }

    public void verifyRepositoryExists() {
        repositoryClient.verifyRepositoryExists();
    }
}
