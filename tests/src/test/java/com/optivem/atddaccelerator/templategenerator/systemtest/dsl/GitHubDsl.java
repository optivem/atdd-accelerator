package com.optivem.atddaccelerator.templategenerator.systemtest.dsl;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers.*;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;

public class GitHubDsl {
    private final ReadmeClient readmeClient;
    private final FileClient fileClient;
    private final WorkflowClient workflowClient;
    private final PagesClient pagesClient;
    private final DockerComposeClient dockerComposeClient;
    private final RepositoryClient repositoryClient;
    private final PackageClient packageClient;

    public GitHubDsl(GithubClient client) {
        this.readmeClient = new ReadmeClient(client);
        this.fileClient = new FileClient(client);
        this.workflowClient = new WorkflowClient(client);
        this.pagesClient = new PagesClient(client);
        this.dockerComposeClient = new DockerComposeClient(client);
        this.repositoryClient = new RepositoryClient(client);
        this.packageClient = new PackageClient(client);
    }

    public void verifyReadmeHasBadges(Language systemLanguage, Language systemTestLanguage) {
        readmeClient.verifyReadmeHasBadges(systemLanguage, systemTestLanguage);
    }

    public void verifyPathsExist(Language systemLanguage, Language systemTestLanguage) {
        fileClient.verifyPathsExist(systemLanguage, systemTestLanguage);
    }

    public void verifyWorkflowsPass(Language systemLanguage, Language systemTestLanguage) {
        workflowClient.verifyWorkflowsPass(systemLanguage, systemTestLanguage);
    }

    public void verifyPagesEnabled() {
        pagesClient.verifyPagesEnabled();
    }

    public void verifyDockerComposeImage(Language systemLanguage, Language systemTestLanguage) {
        dockerComposeClient.verifyDockerComposeImage(systemLanguage, systemTestLanguage);
    }

    public void deleteRepository() {
        repositoryClient.deleteRepository();
    }

    public void verifyRepositoryExists() {
        repositoryClient.verifyRepositoryExists();
    }

    public void verifyPackagesExist(Language systemLanguage) {
        packageClient.verifyPackagesExist(systemLanguage);
    }
}
