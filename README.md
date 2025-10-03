# ATDD Accelerator Quickstart

[![system-tests](https://github.com/optivem/atdd-accelerator/actions/workflows/system-tests.yml/badge.svg)](https://github.com/optivem/atdd-accelerator/actions/workflows/system-tests.yml)

This is a quickstart guide for the [ATDD Accelerator](https://atdd-accelerator.optivem.com/). This is designed to help you complete your Sandbox Project Setup.

# Choose Repository Strategy

What is the Repository Strategy that you use in your Real Life Project:
- Mono Repo
- Multi Repo

# Mono Repo Quickstart

## Option A: Automated Setup (Recommended)

- You have an account on GitHub
- **.NET 8**: [Install Guide](https://dotnet.microsoft.com/en-us/download)
- **Git**: [Install Guide](https://git-scm.com/downloads)
- **GitHub CLI**: [Install Guide](https://cli.github.com/)
- **Docker Desktop**: [Install Guide](https://docs.docker.com/engine/install)

Check whether you have these installed:

```
dotnet --version    # .NET
git --version       # Git
gh --version        # GitHub CLI
docker --version    # Docker
```

Check that you have authenticated into GitHub:
```
gh auth status
```

If you haven't authenticated, then you'll need to log into GitHub:
```
gh auth login
```


### Tool Installation

```bash
# Simple installation - no authentication required
dotnet tool install --global Optivem.AtddAccelerator.TemplateGenerator

# Use the tool
atdd --version
atdd --help
```

To update the version

```bash
# Update the tool to the latest version
dotnet tool update --global Optivem.AtddAccelerator.TemplateGenerator

# Verify the updated version
atdd --version
```

### Tool Usage

