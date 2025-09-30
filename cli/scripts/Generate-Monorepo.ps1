param(
    [Parameter(Mandatory=$true)]
    [string]$RepositoryName,
    
    [Parameter(Mandatory=$false)]
    [string]$GitHubUsername,
    
    [Parameter(Mandatory=$true)]
    [string]$SystemLanguage,

    [Parameter(Mandatory=$true)]
    [string]$SystemTestLanguage
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
            Write-Output "Loaded module: $moduleFile"
        } catch {
            Write-Warning "Failed to load module $moduleFile : $($_.Exception.Message)"
        }
    } else {
        Write-Warning "Module not found: $moduleFile (skipping)"
    }
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
    
    Write-Output "SystemLanguage '$SystemLanguage' is valid"
}

function Get-GitHubUsername {
    param([string]$ProvidedUsername)
    
    if ($ProvidedUsername) {
        return $ProvidedUsername
    }
    
    try {
        $authStatus = gh auth status 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "GitHub CLI not authenticated. Using provided username or defaulting to 'user'"
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
        [string]$TemplateName = "optivem/atdd-accelerator-template-mono-repo"
    )
    
    Write-Output "Creating repository from template..."
    
    try {
        gh repo create $RepositoryName --template $TemplateName --public --clone
        
        if ($LASTEXITCODE -eq 0) {
            Write-Output "Repository created successfully with GitHub CLI"
            Set-Location $RepositoryName
            return
        } else {
            throw "GitHub CLI failed with exit code $LASTEXITCODE"
        }
    } catch {
        Write-Warning "GitHub CLI not available or failed: $($_.Exception.Message)"
        Write-Output "Creating local directory structure instead..."
        
        # Create basic directory structure if GitHub CLI fails
        $repoPath = Join-Path (Get-Location) $RepositoryName
        New-Item -ItemType Directory -Path $repoPath -Force | Out-Null
        Set-Location $repoPath
        
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
            Write-Output "Git repository initialized"
        } catch {
            Write-Warning "Git not available or failed to initialize repository"
        }
    }
}

# Main execution
try {
    Write-Output "ATDD Accelerator Setup Script"
    Write-Output "Repository Name: $RepositoryName"
    Write-Output "System Language: $SystemLanguage"
    Write-Output "System Test Language: $SystemTestLanguage"
    
    # Validate languages
    Test-SystemLanguage -SystemLanguage $SystemLanguage
    Test-SystemLanguage -SystemLanguage $SystemTestLanguage

    $GitHubUsername = Get-GitHubUsername -ProvidedUsername $GitHubUsername
    Write-Output "GitHub Username: $GitHubUsername"
    
    # Create repository from template
    New-RepositoryFromTemplate -RepositoryName $RepositoryName
    
    # Call imported functions if they exist
    if (Get-Command "Remove-UnusedLanguageFolders" -ErrorAction SilentlyContinue) {
        Write-Output "Removing unused language folders..."
        Remove-UnusedLanguageFolders -SystemLanguage $SystemLanguage -SystemTestLanguage $SystemTestLanguage
    } else {
        Write-Warning "Remove-UnusedLanguageFolders function not available"
    }
    
    if (Get-Command "Update-ReadmeBadges" -ErrorAction SilentlyContinue) {
        Write-Output "Updating README badges..."
        Update-ReadmeBadges -SystemLanguage $SystemLanguage -RepositoryOwner $GitHubUsername -RepositoryName $RepositoryName -SystemTestLanguage $SystemTestLanguage
    } else {
        Write-Warning "Update-ReadmeBadges function not available"
    }
    
    if (Get-Command "Setup-GitHubPages" -ErrorAction SilentlyContinue) {
        Write-Output "Setting up GitHub Pages..."
        Setup-GitHubPages
    }
    
    if (Get-Command "Update-DockerCompose" -ErrorAction SilentlyContinue) {
        Write-Output "Updating Docker Compose..."
        Update-DockerCompose -SystemLanguage $SystemLanguage
    }
    
    Write-Output ""
    Write-Output "Repository setup completed successfully!"
    Write-Output "Repository: $GitHubUsername/$RepositoryName"
    Write-Output "System Language: $SystemLanguage"
    Write-Output "System Test Language: $SystemTestLanguage"
    
    exit 0
    
} catch {
    Write-Error "Setup failed: $($_.Exception.Message)"
    exit 1
}