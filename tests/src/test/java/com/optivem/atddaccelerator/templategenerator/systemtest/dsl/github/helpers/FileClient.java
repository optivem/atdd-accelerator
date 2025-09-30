package com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Constants;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertFailure;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertSuccess;

public class FileClient {

    private final GithubClient client;

    public FileClient (GithubClient client) {
        this.client = client;
    }

    public void verifyPathsExist(String systemLanguage, String systemTestLanguage) {
        verifyPathLanguageExists("monolith-%s", systemLanguage);
        verifyPathLanguageExists( getWorkflowPath(Constants.COMMIT_STAGE_MONOLITH_FORMAT), systemLanguage);

        verifyPathLanguageExists("system-test-%s/docker-compose.yml", systemTestLanguage);
        verifyPathLanguageExists( getWorkflowPath(Constants.LOCAL_ACCEPTANCE_STAGE_TEST_FORMAT), systemTestLanguage);
        verifyPathLanguageExists( getWorkflowPath(Constants.ACCEPTANCE_STAGE_TEST_FORMAT), systemTestLanguage);
        verifyPathLanguageExists( getWorkflowPath(Constants.QA_STAGE_TEST_FORMAT), systemTestLanguage);
        verifyPathLanguageExists( getWorkflowPath(Constants.PROD_STAGE_TEST_FORMAT), systemTestLanguage);
    }

    private void verifyPathExists(String path) {
        var result = client.viewPath(path);
        assertSuccess(result, "Path '" + path + "' should exist.");
    }

    private void verifyPathDoesNotExist(String path) {
        var result = client.viewPath(path);
        assertFailure(result, "Path '" + path + "' should not exist.");
    }

    private void verifyPathLanguageExists(String pathFormat, String language) {
        for(String l : Language.ALL) {
            var path = String.format(pathFormat, l);
            if(l.equals(language)) {
                verifyPathExists(path);
            } else {
                verifyPathDoesNotExist(path);
            }
        }
    }

    private String getWorkflowPath(String stageFormat) {
        return ".github/workflows/" + stageFormat + ".yml";
    }

}