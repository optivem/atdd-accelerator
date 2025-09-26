# ATDD Accelerator Quickstart

[![system-test](https://github.com/optivem/atdd-accelerator/actions/workflows/system-test.yml/badge.svg)](https://github.com/optivem/atdd-accelerator/actions/workflows/system-test.yml)

This is a quickstart guide for the [ATDD Accelerator](https://atdd-accelerator.optivem.com/). This is designed to help you complete your Sandbox Project Setup.

# Choose Repository Strategy

What is the Repository Strategy that you use in your Real Life Project:
- Mono Repo
- Multi Repo

# Mono Repo Quickstart

## Option A: Automated Setup (Recommended)

- **PowerShell 7+**: [Install Guide](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell)
- **GitHub CLI**: [Install Guide](https://cli.github.com/)
- **Git**: [Install Guide](https://git-scm.com/)

Check whether you have these installed:

```
pwsh --version      # PowerShell 7+ 
git --version       # Git
gh --version        # GitHub CLI
```

Check that you have authenticated into GitHub:
```
gh auth status
```

If you haven't authenticated, then you'll need to log into GitHub:
```
gh auth login
```


### Quick Setup

#### One-Line Setup (Recommended):
```powershell
# Download and run in one command
Invoke-WebRequest -Uri "https://raw.githubusercontent.com/optivem/atdd-accelerator/main/scripts/setup-mono-repo.ps1" -OutFile "setup-mono-repo.ps1"; .\setup-mono-repo.ps1 -RepositoryName "your-project-name" -SystemLanguage "Java" -SystemTestLanguage "TypeScript"
```

#### Alternative - Two-Step Setup:
1. Download the setup script: [setup-mono-repo.ps1](https://raw.githubusercontent.com/optivem/atdd-accelerator/main/scripts/setup-mono-repo.ps1)
2. Run PowerShell and execute:
```powershell
.\setup-mono-repo.ps1 -RepositoryName "your-project-name" -SystemLanguage "Java" -SystemTestLanguage "TypeScript"
```

**Setup Time:** 2-3 minutes
4. Follow the displayed next steps for GitHub Pages setup

### What the script automates:
- ✅ Creates repository from template
- ✅ Clones to your local machine  
- ✅ Removes unused language folders
- ✅ Cleans up workflow files
- ✅ Updates README badges with your repository
- ✅ Updates docker-compose configuration
- ✅ Commits and pushes all changes

### Script Parameters:
- `-RepositoryName` - Your repository name (e.g., "eshop")
- `-SystemLanguage` - Java | DotNet | TypeScript
- `-SystemTestLanguage` - Java | DotNet | TypeScript  
- `-GitHubUsername` - (optional, auto-detected)
- `-SkipClone` - Skip cloning if repo exists locally

## Option B: Manual Setup

### Repository Name

Choose your Repository Name

### Create Repository

1. Log into GitHub
2. Go to https://github.com/optivem/atdd-accelerator-template-mono-repo
3. Click on `Use this template` > `Create a new repository`
4. Fill in these details:
   - Repository name: this should be your Repository Name, e.g. eshop
   - Visibility: Public
5. Click `Create Repository`
6. Wait for several minutes
7. Click on `Code` -> `Local` -> `Clone` -> `Open with GitHub Desktop`
8. When GitHub Desktop opens up, click on `Clone`
9. Click `Open in Visual Studio Code`

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




4. Open up the README.md file, see the section `System`. Keep only the status badge for our chosen language (in my example, Java) and delete all the REST

   - `[![commit-stage-monolith-dotnet](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/commit-stage-monolith-dotnet.yml/badge.svg)](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/commit-stage-monolith-dotnet.yml)` --> DELETE
   - `[![commit-stage-monolith-java](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/commit-stage-monolith-java.yml/badge.svg)](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/commit-stage-monolith-java.yml)` --> KEEP
   - `[![commit-stage-monolith-typescript](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/commit-stage-monolith-typescript.yml/badge.svg)](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/commit-stage-monolith-typescript.yml)` --> DELETE

5. In the `System` section, update the status badge by providing your repository path, i.e. we'll be replacing `optivem/atdd-accelerator-template-mono-repo` by your concrete repository link, e.g. `valentinajemuovic/eshop`
   - BEFORE: `[![commit-stage-monolith-java](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/commit-stage-monolith-java.yml/badge.svg)](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/commit-stage-monolith-java.yml)`
   - AFTER: `[![commit-stage-monolith-java](https://github.com/valentinajemuovic/eshop/actions/workflows/commit-stage-monolith-java.yml/badge.svg)](https://github.com/valentinajemuovic/eshop/actions/workflows/commit-stage-monolith-java.yml)`

6. Commit & push changes.

6. Go to your repository on GitHub website, e.g. `https://github.com/valentinajemuovic/eshop`

4. Open up the README.md file, in the `System` section, verify that the status badge is `passing`.



6. See the packages generated, example, keep only the one for your System Language (to delete the rest, click on the Package, go to `Package Settings`, then `Delete this package`):
`eshop/monolith-dotnet` --> DELETE
`eshop/monolith-java` --> KEEP
`eshop/monolith-typescript` --> DELETE

_Note for Step 2: Within this template, the System is a Monolith. That's because, for purposes of the ATDD Accelerator Program, it really doesn't matter what System Architecture you use, whether it's Monolith, or Frontend & Monolithic Backend, or Frontend & Microservice Backend, or whatever else. Please keep the Monolith for now, later, at the end of the setup, you can change it to anything else._

_Note for Step 3: We have only one commit Stage because we're using a Monolith, so that's why we have commit-stage-monolith-java.yml. However, later if you decide to switch to Frontend & Monolithic Backend, you might have commit-stage-frontend-react.yml, commit-stage-backend-java.yml; or if you switch to Frontend & Microservice Backend, then you might have commit-stage-frontend-react.yml, commit-stage-microservice1-java.yml, commit-stage-microservice2-dotnet.yml, commit-stage-microservice3-java.yml, etc._


## Choose System Test Language

1. Choose your System Test Language, this is the language you'll be using to write System Tests (Smoke Tests, E2E Tests, Acceptance Tests). The choice of this language is an independent decision compared to what you've chosen for the System Language, so you can choose same or different. Please choose from one of the following options: Java | .NET | TypeScript. For example, I'll choose TypeScript, since my QA Automation Engineers are familar with TypeScript.

2. Keep the System Test folder only for that language, and delete all rest, e.g. since I've chosen TypeScript, then:
    - `system-test-dotnet` --> DELETE
    - `system-test-java` --> DELETE
    - `system-test-typescript` --> KEEP

3. Then open up the System Test folder (in my case `system-test-typescript`), and in the image section, uncomment out all the options, and keep only the one that corresponds to your System Language (in my case, Java), delete all the rest:

```
  monolith:
    image: ghcr.io/optivem/atdd-accelerator-template-mono-repo/monolith-dotnet:latest
    ports:
      - "8080:80"
```
--> DELETE

```
  monolith:
    image: ghcr.io/optivem/atdd-accelerator-template-mono-repo/monolith-java:latest
    ports:
      - "8080:8080"
```
--> KEEP

```
  monolith:
    image: ghcr.io/optivem/atdd-accelerator-template-mono-repo/monolith-typescript:latest
    ports:
      - "8080:3000"
```
--> DELETE


4. Now change the path so that it corresponds to this Repository, specifically replacing this part `optivem/atdd-accelerator-template-mono-repo` for example:
    - TEMPLATE: `image: ghcr.io/optivem/atdd-accelerator-template-mono-repo/monolith-java:latest`
    - UPDATED: `image: ghcr.io/valentinajemuovic/eshop/monolith-java:latest`
_Note: This step is critical that you get the path right! Otherwise, your System Tests & Release will fail!_




6. Keep the Local Acceptance Stage only for that language, and delete all the rest, e.g. since I've chosen TypeScript, then:
   - `.github\workflows\local-acceptance-stage-dotnet.yml` --> DELETE
   - `.github\workflows\local-acceptance-stage-java.yml` --> DELETE
   - `.github\workflows\local-acceptance-stage-typescript.yml` --> KEEP



7. Keep the Release Stage only for that language, and delete all the rest, e.g. since I've chosen TypeScript, then:
   - `.github\workflows\release-stage-dotnet.yml` --> DELETE
   - `.github\workflows\release-stage-java.yml` --> DELETE
   - `.github\workflows\release-stage-typescript.yml` --> KEEP



8. In the README.md file, in the `System Release` section, keep only the System Test Language, delete all the rest (in my example, I've chosen TypeScript)

`[![release-stage-dotnet](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/release-stage-dotnet.yml/badge.svg)](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/release-stage-dotnet.yml)` --> DELETE

`[![release-stage-java](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/release-stage-java.yml/badge.svg)](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/release-stage-java.yml)` --> DELETE

`[![release-stage-typescript](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/release-stage-typescript.yml/badge.svg)](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/release-stage-typescript.yml)` --> KEEP

9. In that `System Release` section, for the status badge that remains, replace `optivem/atdd-accelerator-template-mono-repo` by your repository path, for example:

- OLD: `[![release-stage-typescript](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/release-stage-typescript.yml/badge.svg)](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/release-stage-typescript.yml)`

- UPDATED: `[![release-stage-typescript](https://github.com/valentinajemuovic/eshop/actions/workflows/release-stage-typescript.yml/badge.svg)](https://github.com/valentinajemuovic/eshop/actions/workflows/release-stage-typescript.yml)`

10. Commit and push the changes.

11. Open your repository on GitHub online, for example I go on my repository https://github.com/valentinajemuovic/eshop

10. In the section `System Release`, check that the status badge has `no status`. Click on the status badge, then click on `Run workflow`. Reload the page. Wait for several minutes. Occassionally keep re-loading again, until it finishes, so that you can see when it finishes. It should finish successfully.

11. Go back to the README.md file, in System Release, verify that status is `passing`.

## Documentation

1. On your GitHub repository, go to settings, for example I go here `https://github.com/valentinajemuovic/eshop/settings`

2. Click on `Pages`.

3. In `Build and deployment`, select the following:
   - `Source`: `Deploy from a branch`
   - `Branch`: `main`
   - `Select folder`: `/docs`
   - Click on `Save`

4. Go back to main page, for example I go on `https://github.com/valentinajemuovic/eshop`

5. In the top right corner, you'll see the `About` section, click on the gear symbol, then in `Website`, tick `Use your GitHub Pages website`. Copy-paste that website link (in my case, it's `https://valentinajemuovic.github.io/eshop/`). Click on `Save changes`

6. In the README.md file, go to the section `Documentation`, adn replace the link:
- OLD: `[Documentation](https://optivem.github.io/atdd-accelerator-template-mono-repo/)`
- UPDATED: `[Documentation](https://valentinajemuovic.github.io/eshop/)`

7. Find the status badge for `[pages-build-deployment]`. Replace that link to point to your Website, e.g.
- OLD: `[![pages-build-deployment](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/pages/pages-build-deployment/badge.svg)](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/pages/pages-build-deployment)`
- NEW: `[![pages-build-deployment](https://github.com/valentinajemuovic/eshop/actions/workflows/pages/pages-build-deployment/badge.svg)](https://github.com/valentinajemuovic/eshop/actions/workflows/pages/pages-build-deployment)`

8. Commit & push the changes.

9. Check that the `pages-build-deployment` badge has status `passing`.

10. In the top-right `About` section, click on the Website link (in my case `https://valentinajemuovic.github.io/eshop/`) and check that it opens.

## Project Naming

Now you can open up the project in IDE, and wherever you see reference to optivem/template, you need to replace it by your project naming.

# Multi Repo Quickstart
