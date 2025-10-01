package com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;

public class PackageClient {
    private final GithubClient client;
    private final String repositoryPath;

    public PackageClient(GithubClient client) {
        this.client = client;
        this.repositoryPath = client.getRepositoryPath();
    }

    public void verifyPackagesExist(Language systemLanguage) {
        for(var l : Language.getAll()) {
            var path = String.format("%s/monolith-%s", repositoryPath, l.getValue());
            if(l.equals(systemLanguage)) {
                verifyPackageExists(path);
            } else {
                verifyPackageDoesNotExist(path);
            }
        }
    }

    private void verifyPackageDoesNotExist(String path) {
    }

    private void verifyPackageExists(String path) {
        
    }
}
