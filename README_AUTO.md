# ATDD Accelerator Tests

This directory contains acceptance tests for the ATDD Accelerator setup scripts.

## Running Tests

### Run All Tests
```powershell
.\Run-Tests.ps1
```

### Run Specific Test
```powershell
.\Run-Tests.ps1 -TestName "Create-Repository"
```

### Run with Detailed Output
```powershell
.\Run-Tests.ps1 -Detailed
```

## Test Structure

- `Create-Repository.Tests.ps1` - Tests repository creation from template
- More tests will be added as we develop the automation scripts

## Prerequisites

- PowerShell 7+
- GitHub CLI authenticated (`gh auth login`)
- Pester testing framework (automatically installed if missing)

## Test Philosophy

These tests follow ATDD principles:
- Tests are written **before** implementation
- Tests describe **user behavior** and **expected outcomes**
- Tests use **real services** (GitHub) to ensure end-to-end functionality