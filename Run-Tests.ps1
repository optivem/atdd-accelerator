<#
.SYNOPSIS
Test runner for ATDD Accelerator tests

.DESCRIPTION
Runs all acceptance tests for the ATDD Accelerator setup scripts

.EXAMPLE
.\Run-Tests.ps1
Runs all tests

.EXAMPLE
.\Run-Tests.ps1 -TestName "Create-Repository"
Runs specific test file
#>

param(
    [string]$TestName = "*",
    [switch]$Detailed
)

# Ensure Pester is available
if (-not (Get-Module Pester -ListAvailable)) {
    Write-Host "Installing Pester..." -ForegroundColor Yellow
    Install-Module Pester -Force -SkipPublisherCheck
}

# Import Pester
Import-Module Pester -Force

# Run tests
$testPath = if ($TestName -eq "*") { ".\tests\" } else { ".\tests\$TestName.Tests.ps1" }
$outputLevel = if ($Detailed) { "Detailed" } else { "Normal" }

Write-Host "Running ATDD Accelerator Tests..." -ForegroundColor Cyan
Write-Host "Test Path: $testPath" -ForegroundColor Gray
Write-Host ""

Invoke-Pester $testPath -Output $outputLevel