package com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Constants;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;

import static org.assertj.core.api.Assertions.assertThat;

public class ReadmeClient {

    private static final String README_PATH = "README.md";

    private final GithubClient client;
    private final String repositoryPath;

    public ReadmeClient (GithubClient client) {
        this.client = client;
        this.repositoryPath = client.getRepositoryPath();
    }

    public void verifyReadmeHasBadges(Language systemLanguage, Language systemTestLanguage) {
        verifyReadmePagesBadge();
        verifyReadmeStageLanguageBadge(Constants.COMMIT_STAGE_MONOLITH_FORMAT, systemLanguage);
        verifyReadmeStageLanguageBadge(Constants.LOCAL_ACCEPTANCE_STAGE_TEST_FORMAT, systemTestLanguage);
        verifyReadmeStageLanguageBadge(Constants.ACCEPTANCE_STAGE_TEST_FORMAT, systemTestLanguage);
        verifyReadmeStageLanguageBadge(Constants.QA_STAGE_TEST_FORMAT, systemTestLanguage);
        verifyReadmeStageLanguageBadge(Constants.PROD_STAGE_TEST_FORMAT, systemTestLanguage);
    }

    private void verifyReadmePagesBadge() {
        var badgeWorkflow = String.format(Constants.PAGES_BUILD_DEPLOYMENT_WORKFLOW_FORMAT, repositoryPath);
        var badgeSvg = String.format(Constants.PAGES_BUILD_DEPLOYMENT_WORKFLOW_IMAGE_FORMAT, repositoryPath);
        verifyReadmeContainsBadge(Constants.PAGES_BUILD_DEPLOYMENT, badgeWorkflow, badgeSvg);
    }

    private void verifyReadmeStageLanguageBadge(String workflowNameFormat, Language language){
        for(var l : Language.getAll()) {
            var workflowName = String.format(workflowNameFormat, l.getValue());
            if(l.equals(language)) {
                verifyReadmeContainsBadge(workflowName);
            } else {
                verifyReadmeDoesNotContainBadge(workflowName);
            }
        }
    }

    private void verifyReadmeContainsBadge(String workflowName) {
        var badgeWorkflow = String.format(Constants.STAGE_WORKFLOW_FORMAT, repositoryPath, workflowName);
        var badgeSvg = String.format(Constants.STAGE_WORKFLOW_IMAGE_FORMAT, repositoryPath, workflowName);

        verifyReadmeContainsBadge(workflowName, badgeWorkflow, badgeSvg);
    }

    private void verifyReadmeContainsBadge(String badgeName, String badgeWorkflow, String badgeSvg) {
        var readmeContent = getReadmeContent();

        assertThat(readmeContent)
                .as("README should contain badge: " + badgeName)
                .contains(badgeName);

        assertThat(readmeContent)
                .as("README should contain badge workflow link: " + badgeWorkflow)
                .contains(badgeWorkflow);

        assertThat(readmeContent)
                .as("README should contain badge SVG: " + badgeSvg)
                .contains(badgeSvg);
    }

    private void verifyReadmeDoesNotContainBadge(String badge) {
        var readmeContent = getReadmeContent();
        assertThat(readmeContent)
                .as("README should not contain badge: " + badge)
                .doesNotContain(badge);
    }

    private String getReadmeContent() {
        return client.getFileContent(README_PATH);
    }

}
