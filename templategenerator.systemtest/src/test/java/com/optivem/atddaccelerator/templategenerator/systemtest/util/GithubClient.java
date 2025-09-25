package com.optivem.atddaccelerator.templategenerator.systemtest.util;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.executeProcess;

public class GithubClient {
    public void viewRepository(String owner, String repoName) {
        var path = owner + "/" + repoName;
        executeProcess("gh", "repo", "view", path);
    }
}
