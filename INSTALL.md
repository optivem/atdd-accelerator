## Installation

### Prerequisites
- .NET 8.0 or later
- GitHub account with access to this repository

### Install from GitHub Packages

```bash
# Configure GitHub Packages source (one-time setup)
dotnet nuget add source https://nuget.pkg.github.com/optivem/index.json \
  --name github \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_TOKEN \
  --store-password-in-clear-text

# Install the tool
dotnet tool install --global \
  --add-source https://nuget.pkg.github.com/optivem/index.json \
  Optivem.AtddAccelerator.TemplateGenerator

# Use the tool
atdd --help
```