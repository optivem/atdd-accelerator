package com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class PackageClient {
    private final GithubClient client;
    private final String repositoryPath;

    public PackageClient(GithubClient client) {
        this.client = client;
        this.repositoryPath = client.getRepositoryPath();
    }

    public void verifyPackagesExist(Language systemLanguage) {
        for(var l : Language.getAll()) {
            var path = String.format("monolith-%s", l.getValue());
            if(l.equals(systemLanguage)) {
                verifyPackageExists(path);
            } else {
                verifyPackageDoesNotExist(path);
            }
        }
    }

    private void verifyPackageExists(String packageName) {
        var exists = client.packageExists(packageName);
        assertTrue(exists, "Package '" + packageName + "' should exist.");
    }

    private void verifyPackageDoesNotExist(String packageName) {
        var exists = client.packageExists(packageName);
        assertFalse(exists, "Package '" + packageName + "' should not exist.");
    }
}
