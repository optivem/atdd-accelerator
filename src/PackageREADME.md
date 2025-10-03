# ATDD Accelerator CLI

A command-line tool for generating ATDD (Acceptance Test-Driven Development) accelerator templates.

## Installation

Install as a global .NET tool:

```shell
dotnet tool install -g atdd-accelerator-cli
```

## Requirements

## Requirements

- [.NET 8.0](https://dotnet.microsoft.com/en-us/download)
- [GitHub](https://git-scm.com/downloads)
- [GitHub CLI](https://cli.github.com)
- [Docker Desktop](https://www.docker.com/get-started)


You can check that you have installed them:
```
dotnet --version    # .NET
git --version       # Git
gh --version        # GitHub CLI
docker --version    # Docker
```

Also check that you have authenticated into GitHub:
```
gh auth status
```

If you haven't authenticated, then you'll need to log into GitHub:
```
gh auth login
```

## Usage

Generate a new monorepo with ATDD structure:

```shell
atdd generate monorepo --repository-owner jsmith --repository-name eshop --system-language java --system-test-language typescript
```

### Options

- `--repository-owner`: Your GitHub username or organization name
- `--repository-name`: Unique name for the new repository
- `--system-language`: Language for source code (java, dotnet, typescript)
- `--system-test-language`: Language for system tests (java, dotnet, typescript)

## Examples

Generate a monorepo for an eshop project using Java and TypeScript:

```shell
atdd generate monorepo --repository-owner jsmith --repository-name eshop --system-language java --system-test-language typescript
```

Generate a monorepo for an API project using .NET for both system and tests:

```shell
atdd generate monorepo --repository-owner jsmith --repository-name eshop --system-language dotnet --system-test-language dotnet
```

As you can see, the choice of language for your System versus System Tests is flexible, you can choose the same language or different languages.

Regarding the repository owner, if you're creating the repository under your personal account, then you would use your GitHub username. If you're creating the repository under an organization, then you would use the organization name.

## Support

- Repository: https://github.com/optivem/atdd-accelerator
- Issues: https://github.com/optivem/atdd-accelerator/issues
- License: MIT