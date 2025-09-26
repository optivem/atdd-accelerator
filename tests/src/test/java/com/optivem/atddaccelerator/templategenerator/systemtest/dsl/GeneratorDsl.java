package com.optivem.atddaccelerator.templategenerator.systemtest.dsl;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GeneratorClient;

import java.util.HashSet;
import java.util.Set;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.waitTime;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertFailure;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertSuccess;

public class GeneratorDsl {
    private static final int MILLIS = 3000;

    private Set<String> created;

    private final GeneratorClient client;

    public GeneratorDsl(GeneratorClient client) {
        this.client = client;
        this.created = new HashSet<>();
    }

    public boolean isCreated(String repoName) {
        return created.contains(repoName);
    }

    public void generateNewRepository(String repoName, String systemLanguage, String systemTestLanguage) {
        var result = client.generateRepository(repoName, systemLanguage, systemTestLanguage);
        assertSuccess(result);

        waitTime(MILLIS);
        created.add(repoName);
    }

    public void generateNewRepositoryExpectError(String repoName, String systemLanguage, String systemTestLanguage) {
        var result = client.generateRepository(repoName, systemLanguage, systemTestLanguage);
        assertFailure(result);
    }
}
