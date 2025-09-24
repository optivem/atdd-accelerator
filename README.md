# ATDD Accelerator Quickstart

This is a quickstart guide for the [ATDD Accelerator](https://atdd-accelerator.optivem.com/). This is designed to help you complete your Sandbox Project Setup.

# Choose Repository Strategy

What is the Repository Strategy that you use in your Real Life Project:
- Mono Repo
- Multi Repo

# Mono Repo Quickstart

## Create Repository

1. Log into GitHub
2. Go to https://github.com/optivem/atdd-accelerator-template-mono-repo
3. Click on "Use this template" > "Create a new repository"
4. Fill in these details:
   a. Repository name: this should be your System Name, e.g. nova-shop
   b. Visibility: Public
5. Click "Create Repository"
6. Wait for several minutes
7. Click on "Code" -> "Local" -> "Clone" -> "Open with GitHub Desktop"
8. When GitHub Desktop opens up, click on "Clone"
9. Click "Open in Visual Studio Code"

## Choose System Language

1. Choose your System Language, from the following options: Java | .NET | TypeScript (for example, I'll choose Java).
2. Based on your chosen language, keep the System folder only for that language, and delete all the rest, e.g. since I've chosen Java, then:
   - `monolith-dotnet` --> DELETE
   - `monolith-java` --> KEEP
   - `monolith-typescript` --> DELETE
3. Based on your chosen language, keep the commit stage only for that language, and delete all the rest, e.g. since I've chosen Java, then:
   - `.github\workflows\commit-stage-monolith-dotnet.yml` --> DELETE
   - `.github\workflows\commit-stage-monolith-java.yml` --> KEEP
   - `.github\workflows\commit-stage-monolith-typescript.yml` --> DELETE


_Note for Step 2: Within this template, the System is a Monolith. That's because, for purposes of the ATDD Accelerator Program, it really doesn't matter what System Architecture you use, whether it's Monolith, or Frontend & Monolithic Backend, or Frontend & Microservice Backend, or whatever else. Please keep the Monolith for now, later, at the end of the setup, you can change it to anything else._

_Note for Step 3: We have only one commit Stage because we're using a Monolith, so that's why we have commit-stage-monolith-java.yml. However, later if you decide to switch to Frontend & Monolithic Backend, you might have commit-stage-frontend-react.yml, commit-stage-backend-java.yml; or if you switch to Frontend & Microservice Backend, then you might have commit-stage-frontend-react.yml, commit-stage-microservice1-java.yml, commit-stage-microservice2-dotnet.yml, commit-stage-microservice3-java.yml, etc._

## Choose System Test Language

1. Choose your System Test Language, this is the language you'll be using to write System Tests (Smoke Tests, E2E Tests, Acceptance Tests). The choice of this language is an independent decision compared to what you've chosen for the System Language, so you can choose same or different. Please choose from one of the following options: Java | .NET | TypeScript. For example, I'll choose TypeScript, since my QA Automation Engineers are familar with TypeScript.
2. Keep the System Test folder only for that language, and delete all rest, e.g. since I've chosen TypeScript, then:
    - `system-test-dotnet` --> DELETE
    - `system-test-java` --> DELETE
    - `system-test-typescript` --> KEEP
2. Keep the Local Acceptance Stage only for that language, and delete all the rest, e.g. since I've chosen TypeScript, then:
   - `.github\workflows\local-acceptance-stage-dotnet.yml` --> DELETE
   - `.github\workflows\local-acceptance-stage-java.yml` --> DELETE
   - `.github\workflows\local-acceptance-stage-typescript.yml` --> KEEP
2. Keep the Local Acceptance Stage only for that language, and delete all the rest, e.g. since I've chosen TypeScript, then:
   - `.github\workflows\local-acceptance-stage-dotnet.yml` --> DELETE
   - `.github\workflows\local-acceptance-stage-java.yml` --> DELETE
   - `.github\workflows\local-acceptance-stage-typescript.yml` --> KEEP
3. Keep the Release Stage only for that language, and delete all the rest, e.g. since I've chosen TypeScript, then:
   - `.github\workflows\release-stage-dotnet.yml` --> DELETE
   - `.github\workflows\release-stage-java.yml` --> DELETE
   - `.github\workflows\release-stage-typescript.yml` --> KEEP


# Multi Repo Quickstart
