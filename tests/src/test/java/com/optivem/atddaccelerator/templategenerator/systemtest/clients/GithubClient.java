package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessResult;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessExecutor.*;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessResultAssertions.assertFailure;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessResultAssertions.assertSuccess;
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

    public ProcessResult deleteRepository() {
        return executeProcess("gh", "repo", "delete", repositoryPath, "--yes");
    }

    public ProcessResult viewRepository() {
        return executeProcess("gh", "repo", "view", repositoryPath);
    }

    public ProcessResult viewPath(String path) {
        return executeProcess("gh", "api", "/repos/" + repositoryPath + "/contents/" + path);
    }

    public ProcessResult viewPages() {
        return executeProcess("gh", "api", "/repos/" + repositoryPath + "/pages");
    }

    public String getFileContent(String filePath) {
        var result = executeProcess("gh", "api",
                "-H", "Accept: application/vnd.github.raw",
                "/repos/" + repositoryPath + "/contents/" + filePath);
        assertSuccess(result);
        return result.getOutput();
    }

    public ProcessResult viewWorkflowRuns(String workflowFileName) {
        return executeProcess("gh", "run", "list",
                "--workflow", workflowFileName,
                "--limit", "1",
                "--json", "status,conclusion",
                "--repo", repositoryPath);
    }


}
