package com.optivem.atddaccelerator.templategenerator.systemtest.util;

public class Constants {
    // Stages
    public static final String PAGES_BUILD_DEPLOYMENT = "pages-build-deployment";
    public static final String COMMIT_STAGE_MONOLITH_FORMAT = "commit-stage-monolith-%s";
    public static final String LOCAL_ACCEPTANCE_STAGE_TEST_FORMAT = "local-acceptance-stage-test-%s";
    public static final String ACCEPTANCE_STAGE_TEST_FORMAT = "acceptance-stage-test-%s";
    public static final String QA_STAGE_TEST_FORMAT = "qa-stage-test-%s";
    public static final String PROD_STAGE_TEST_FORMAT = "prod-stage-test-%s";

    // Workflows
    public static final String PAGES_BUILD_DEPLOYMENT_WORKFLOW_FORMAT = "https://github.com/%s/actions/workflows/pages/pages-build-deployment";
    public static final String PAGES_BUILD_DEPLOYMENT_WORKFLOW_IMAGE_FORMAT = "https://github.com/%s/actions/workflows/pages/pages-build-deployment/badge.svg";
    public static final String STAGE_WORKFLOW_FORMAT = "https://github.com/%s/actions/workflows/%s.yml";
    public static final String STAGE_WORKFLOW_IMAGE_FORMAT = "https://github.com/%s/actions/workflows/%s.yml/badge.svg";

    // Docker Images
    public static final String MONOLITH_DOCKER_IMAGE_NAME_FORMAT = "ghcr.io/%s/monolith-%s:latest";
}
