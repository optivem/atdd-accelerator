package com.optivem.atddaccelerator.templategenerator.systemtest.util;

public class Constants {
    public static final String PAGES_BUILD_DEPLOYMENT = "pages-build-deployment";

    public static String COMMIT_STAGE_MONOLITH_FORMAT = "commit-stage-monolith-%s";
    public static String LOCAL_ACCEPTANCE_STAGE_TEST_FORMAT = "local-acceptance-stage-test-%s";
    public static String ACCEPTANCE_STAGE_TEST_FORMAT = "acceptance-stage-test-%s";
    public static String QA_STAGE_TEST_FORMAT = "qa-stage-test-%s";
    public static String PROD_STAGE_TEST_FORMAT = "prod-stage-test-%s";

    public static final String SYSTEM_TEST_TYPESCRIPT_DOCKER_COMPOSE = "system-test-typescript/docker-compose.yml";

    public static String MONOLITH_DOCKER_IMAGE_NAME_FORMAT = "ghcr.io/%s/monolith-%s:latest";
}
