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

# Ensure Pester 5+ is available
if (-not (Get-Module Pester -ListAvailable)) {
    Write-Host "Installing Pester 5+..." -ForegroundColor Yellow
    Install-Module Pester -Force -SkipPublisherCheck
} else {
    $pesterVersion = (Get-Module Pester -ListAvailable | Select-Object -First 1).Version.Major
    if ($pesterVersion -lt 5) {
        Write-Host "Updating to Pester 5+..." -ForegroundColor Yellow
        Install-Module Pester -Force -SkipPublisherCheck
    }
}

# Import Pester
Import-Module Pester -Force

# Configure Pester 5+ settings
$config = New-PesterConfiguration
$config.Run.Path = if ($TestName -eq "*") { ".\tests\" } else { ".\tests\$TestName.Tests.ps1" }
$config.Output.Verbosity = if ($Detailed) { 'Detailed' } else { 'Normal' }

Write-Host "Running ATDD Accelerator Tests..." -ForegroundColor Cyan
Write-Host "Test Path: $($config.Run.Path)" -ForegroundColor Gray
Write-Host ""

# Run tests with Pester 5+ syntax
Invoke-Pester -Configuration $config