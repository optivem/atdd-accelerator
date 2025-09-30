## Installation

### Prerequisites
- .NET 8.0 or later

### Install from NuGet.org (Recommended)

```bash
# Simple installation - no authentication required
dotnet tool install --global Optivem.AtddAccelerator.TemplateGenerator

# Use the tool
atdd --version
atdd --help
```

Update:

```bash
# Update the tool to the latest version
dotnet tool update --global Optivem.AtddAccelerator.TemplateGenerator

# Verify the updated version
atdd --version
```

### Install from GitHub Packages (For Development/CI)

```bash
# Configure authentication
export NUGET_AUTH_TOKEN=your_github_token
dotnet nuget add source https://nuget.pkg.github.com/optivem/index.json --name github

# Install the tool
dotnet tool install --global --add-source github Optivem.AtddAccelerator.TemplateGenerator
```

### Install from Release Assets

1. Download the `.nupkg` file from [Releases](https://github.com/optivem/atdd-accelerator/releases)
2. Install locally:
```bash
dotnet tool install --global --add-source ./path/to/download Optivem.AtddAccelerator.TemplateGenerator
```