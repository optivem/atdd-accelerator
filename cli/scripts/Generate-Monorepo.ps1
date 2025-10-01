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
    
    # Otherwise, use temp directory with unique identifier for parallel test safety
    $tempDir = [System.IO.Path]::GetTempPath()
    $uniqueId = [System.Guid]::NewGuid().ToString("N").Substring(0, 8)
    $atddTempDir = Join-Path $tempDir "ATDD-Accelerator-$uniqueId"
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
        # Capture both stdout and stderr
        $processInfo = Start-Process -FilePath "gh" -ArgumentList @("repo", "create", $RepositoryName, "--template", $TemplateName, "--public", "--clone") -Wait -PassThru -RedirectStandardError "gh_error.txt" -RedirectStandardOutput "gh_output.txt" -NoNewWindow
        
        # Read the error output
        $errorOutput = ""
        if (Test-Path "gh_error.txt") {
            $errorOutput = Get-Content "gh_error.txt" -Raw
            Remove-Item "gh_error.txt" -Force
        }
        
        # Read the standard output
        $standardOutput = ""
        if (Test-Path "gh_output.txt") {
            $standardOutput = Get-Content "gh_output.txt" -Raw
            Remove-Item "gh_output.txt" -Force
        }
        
        if ($processInfo.ExitCode -eq 0) {
            Write-Host "Repository created successfully with GitHub CLI"
            
            # Rename the cloned directory if needed
            $clonedDir = Join-Path $parentDir $RepositoryName
            if ($clonedDir -ne $TargetDirectory) {
                Move-Item $clonedDir $TargetDirectory
            }
            
            Set-Location $TargetDirectory
            return
        } else {
            # Check if the error is specifically about repository name already existing
            if ($errorOutput -match "(Name already exists|already exists on this account|repository name .* already exists)" -or 
                $standardOutput -match "(Name already exists|already exists on this account|repository name .* already exists)") {
                throw "Repository name '$RepositoryName' already exists on this GitHub account. Please choose a different name."
            } else {
                throw "GitHub CLI failed with exit code $($processInfo.ExitCode). Error: $errorOutput"
            }
        }
    } catch {
        # Check if this is a "repository already exists" error
        if ($_.Exception.Message -match "already exists") {
            Write-Error $_.Exception.Message
            Set-Location $originalLocation
            exit 1
        }
        
        Write-Warning "GitHub CLI not available or failed: $($_.Exception.Message)"
        Write-Host "This will create a local directory structure only (not a GitHub repository)."
        
        # Ask user if they want to continue with local-only setup
        $continue = Read-Host "Do you want to continue with local setup only? (y/N)"
        if ($continue -notmatch "^[yY]") {
            Write-Host "Operation cancelled by user."
            Set-Location $originalLocation
            exit 1
        }
        
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
    
    # Initialize target directory variable for cleanup purposes
    $targetDirectory = $null
    
    # Validate languages FIRST - before any directory operations
    Test-SystemLanguage -SystemLanguage $SystemLanguage
    Test-SystemLanguage -SystemLanguage $SystemTestLanguage

    $GitHubUsername = Get-GitHubUsername -ProvidedUsername $GitHubUsername
    Write-Host "GitHub Username: $GitHubUsername"
    
    # Get target directory (temp or specified)
    $targetDirectory = Get-OutputDirectory -RepositoryName $RepositoryName -OutputPath $OutputPath
    
    # Create repository from template
    New-RepositoryFromTemplate -RepositoryName $RepositoryName -TargetDirectory $targetDirectory
    
    # Call imported functions
    Write-Host "Removing unused language folders..."
    Remove-UnusedLanguageFolders -SystemLanguage $SystemLanguage -SystemTestLanguage $SystemTestLanguage
    
    Write-Host "Updating README badges..."
    Update-ReadmeBadges -SystemLanguage $SystemLanguage -RepositoryOwner $GitHubUsername -RepositoryName $RepositoryName -SystemTestLanguage $SystemTestLanguage
    
    Write-Host "Setting up GitHub Pages..."
    Enable-GitHubPages -RepositoryOwner $GitHubUsername -RepositoryName $RepositoryName
    
    Write-Host "Updating Docker Compose..."
    Update-DockerComposeFiles -SystemLanguage $SystemLanguage -RepositoryOwner $GitHubUsername -RepositoryName $RepositoryName
    
    # Push all changes to remote repository (if it's a GitHub repository)
    Write-Host ""
    Write-Host "Pushing changes to remote repository..."
    try {
        # Check if we have a remote origin (indicating this is a cloned GitHub repo)
        $remoteOrigin = git remote get-url origin 2>$null
        if ($remoteOrigin) {
            # Add all changes and commit if there are any uncommitted changes
            $status = git status --porcelain
            if ($status) {
                Write-Host "Committing final changes..."
                git add .
                git commit -m "Final setup and configuration changes"
            }
            
            # Push to remote
            Write-Host "Pushing to remote repository..."
            git push origin main 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✅ Changes pushed successfully to GitHub"
            } else {
                Write-Warning "⚠️ Failed to push changes to GitHub"
            }
        } else {
            Write-Host "No remote origin found - skipping push (local repository only)"
        }
    } catch {
        Write-Warning "Failed to push changes: $($_.Exception.Message)"
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
    
    # Clean up any partially created directories when the script fails
    if ($targetDirectory -and (Test-Path $targetDirectory)) {
        try {
            Write-Host "Cleaning up failed generation directory: $targetDirectory"
            # Change to a different directory before cleanup to avoid "in use" errors
            Set-Location ([System.IO.Path]::GetTempPath())
            Remove-Item -Path $targetDirectory -Recurse -Force
        } catch {
            Write-Warning "Could not clean up directory: $targetDirectory - $($_.Exception.Message)"
        }
    }
    
    exit 1
}