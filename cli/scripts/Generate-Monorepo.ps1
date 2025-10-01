param(
    [Parameter(Mandatory=$true)]
    [string]$RepositoryName,
    
    [Parameter(Mandatory=$false)]
    [string]$GitHubUsername,
    
    [Parameter(Mandatory=$true)]
    [string]$SystemLanguage,

    [Parameter(Mandatory=$true)]
    [string]$SystemTestLanguage,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath
)

# Get the script directory for importing modules
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Import helper modules with error handling
$moduleFiles = @(
    "remove-unused-language-folders.ps1",
    "update-readme-badges.ps1", 
    "setup-github-pages.ps1",
    "update-docker-compose.ps1",
    "invoke-system-test-release-workflows.ps1",
    "invoke-build-workflows.ps1"
)

foreach ($moduleFile in $moduleFiles) {
    $modulePath = "$scriptDir\$moduleFile"
    if (Test-Path $modulePath) {
        try {
            . $modulePath
            Write-Host "Loaded module: $moduleFile"
        } catch {
            Write-Warning "Failed to load module $moduleFile : $($_.Exception.Message)"
        }
    } else {
        Write-Warning "Module not found: $moduleFile (skipping)"
    }
}

function Get-OutputDirectory {
    param(
        [string]$RepositoryName,
        [string]$OutputPath
    )
    
    # If OutputPath is specified, use it
    if (-not [string]::IsNullOrWhiteSpace($OutputPath)) {
        $targetDir = Join-Path $OutputPath $RepositoryName
        Write-Host "Using specified output path: $targetDir"
        return $targetDir
    }
    
    # Otherwise, use temp directory
    $tempDir = [System.IO.Path]::GetTempPath()
    $atddTempDir = Join-Path $tempDir "ATDD-Accelerator"
    $targetDir = Join-Path $atddTempDir $RepositoryName
    
    # Ensure the ATDD temp directory exists
    if (-not (Test-Path $atddTempDir)) {
        New-Item -ItemType Directory -Path $atddTempDir -Force | Out-Null
    }
    
    Write-Host "Using temp directory: $targetDir"
    return $targetDir
}

function Test-SystemLanguage {
    param([string]$SystemLanguage)
    
    $validLanguages = @("java", "dotnet", "typescript")
    
    if ([string]::IsNullOrWhiteSpace($SystemLanguage)) {
        throw "SystemLanguage parameter is required. Valid options: $($validLanguages -join ', ')"
    }
    
    if ($SystemLanguage.ToLower() -notin $validLanguages) {
        throw "Invalid SystemLanguage: '$SystemLanguage'. Valid options: $($validLanguages -join ', ')"
    }
    
    Write-Host "SystemLanguage '$SystemLanguage' is valid"
}

function Get-GitHubUsername {
    param([string]$ProvidedUsername)
    
    if ($ProvidedUsername) {
        return $ProvidedUsername
    }
    
    try {
        $authStatus = gh auth status 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "GitHub CLI not authenticated. Using default username 'user'"
            return "user"
        }
        
        $username = ($authStatus | Select-String "Logged in to github\.com account (.+?) " | ForEach-Object { $_.Matches[0].Groups[1].Value })
        if (-not $username) {
            $username = ($authStatus | Select-String "Logged in to github\.com as (.+)" | ForEach-Object { $_.Matches[0].Groups[1].Value })
        }
        
        if ($username) {
            return $username.Trim()
        }
        return "user"
    } catch {
        Write-Warning "Could not get GitHub username: $($_.Exception.Message)"
        return "user"
    }
}

function New-RepositoryFromTemplate {
    param(
        [string]$RepositoryName,
        [string]$TargetDirectory,
        [string]$TemplateName = "optivem/atdd-accelerator-template-mono-repo"
    )
    
    Write-Host "Creating repository from template..."
    Write-Host "Target directory: $TargetDirectory"
    
    # Ensure parent directory exists
    $parentDir = Split-Path $TargetDirectory -Parent
    if (-not (Test-Path $parentDir)) {
        New-Item -ItemType Directory -Path $parentDir -Force | Out-Null
    }
    
    # Remove existing directory if it exists
    if (Test-Path $TargetDirectory) {
        Write-Host "Removing existing directory: $TargetDirectory"
        Remove-Item -Path $TargetDirectory -Recurse -Force
    }
    
    # Change to parent directory for cloning
    $originalLocation = Get-Location
    Set-Location $parentDir
    
    try {
        gh repo create $RepositoryName --template $TemplateName --public --clone
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Repository created successfully with GitHub CLI"
            
            # Rename the cloned directory if needed
            $clonedDir = Join-Path $parentDir $RepositoryName
            if ($clonedDir -ne $TargetDirectory) {
                Move-Item $clonedDir $TargetDirectory
            }
            
            Set-Location $TargetDirectory
            return
        } else {
            throw "GitHub CLI failed with exit code $LASTEXITCODE"
        }
    } catch {
        Write-Warning "GitHub CLI not available or failed: $($_.Exception.Message)"
        Write-Host "Creating local directory structure instead..."
        
        # Create directory structure
        New-Item -ItemType Directory -Path $TargetDirectory -Force | Out-Null
        Set-Location $TargetDirectory
        
        # Create basic files
        $readmeContent = @"
# $RepositoryName

Generated with ATDD Accelerator

- System Language: $SystemLanguage
- System Test Language: $SystemTestLanguage
"@
        
        Set-Content -Path "README.md" -Value $readmeContent -Encoding UTF8
        
        # Initialize git repo
        try {
            git init
            git add .
            git commit -m "Initial commit from ATDD Accelerator"
            Write-Host "Git repository initialized"
        } catch {
            Write-Warning "Git not available or failed to initialize repository"
        }
    } finally {
        # Always restore original location if something goes wrong
        if ((Get-Location).Path -eq $parentDir) {
            Set-Location $originalLocation
        }
    }
}

# Main execution
try {
    Write-Host "ATDD Accelerator Setup Script"
    Write-Host "Repository Name: $RepositoryName"
    Write-Host "System Language: $SystemLanguage"
    Write-Host "System Test Language: $SystemTestLanguage"
    
    # Validate languages
    Test-SystemLanguage -SystemLanguage $SystemLanguage
    Test-SystemLanguage -SystemLanguage $SystemTestLanguage

    $GitHubUsername = Get-GitHubUsername -ProvidedUsername $GitHubUsername
    Write-Host "GitHub Username: $GitHubUsername"
    
    # Get target directory (temp or specified)
    $targetDirectory = Get-OutputDirectory -RepositoryName $RepositoryName -OutputPath $OutputPath
    
    # Create repository from template
    New-RepositoryFromTemplate -RepositoryName $RepositoryName -TargetDirectory $targetDirectory
    
    # Call imported functions if they exist
    if (Get-Command "Remove-UnusedLanguageFolders" -ErrorAction SilentlyContinue) {
        Write-Host "Removing unused language folders..."
        Remove-UnusedLanguageFolders -SystemLanguage $SystemLanguage -SystemTestLanguage $SystemTestLanguage
    } else {
        Write-Warning "Remove-UnusedLanguageFolders function not available"
    }
    
    if (Get-Command "Update-ReadmeBadges" -ErrorAction SilentlyContinue) {
        Write-Host "Updating README badges..."
        Update-ReadmeBadges -SystemLanguage $SystemLanguage -RepositoryOwner $GitHubUsername -RepositoryName $RepositoryName -SystemTestLanguage $SystemTestLanguage
    } else {
        Write-Warning "Update-ReadmeBadges function not available"
    }
    
    if (Get-Command "Setup-GitHubPages" -ErrorAction SilentlyContinue) {
        Write-Host "Setting up GitHub Pages..."
        Setup-GitHubPages
    }
    
    if (Get-Command "Update-DockerCompose" -ErrorAction SilentlyContinue) {
        Write-Host "Updating Docker Compose..."
        Update-DockerCompose -SystemLanguage $SystemLanguage
    }
    
    Write-Host ""
    Write-Host "Repository setup completed successfully!"
    Write-Host "Repository: $GitHubUsername/$RepositoryName"
    Write-Host "Local path: $targetDirectory"
    Write-Host "System Language: $SystemLanguage"
    Write-Host "System Test Language: $SystemTestLanguage"
    
    exit 0
    
} catch {
    Write-Error "Setup failed: $($_.Exception.Message)"
    exit 1
}