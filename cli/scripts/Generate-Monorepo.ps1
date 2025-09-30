param(
    [Parameter(Mandatory=$true)]
    [string]$RepositoryName,
    
    [Parameter(Mandatory=$false)]
    [string]$GitHubUsername,
    
    [Parameter(Mandatory=$false)]
    [string]$SystemLanguage,

    [Parameter(Mandatory=$false)]
    [string]$SystemTestLanguage
)

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
        
        return $username
    } catch {
        Write-Warning "Could not get GitHub username: $_"
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
        
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to create repository"
        }
    } catch {
        Write-Warning "GitHub CLI not available or failed. Creating local directory structure instead."
        
        # Create basic directory structure if GitHub CLI fails
        New-Item -ItemType Directory -Path $RepositoryName -Force
        Set-Location $RepositoryName
        
        # Create basic files
        @"
# $RepositoryName

Generated with ATDD Accelerator

- System Language: $SystemLanguage
- System Test Language: $SystemTestLanguage
"@ | Out-File -FilePath "README.md" -Encoding UTF8
        
        # Initialize git repo
        git init
        git add .
        git commit -m "Initial commit from ATDD Accelerator"
    }
}

# Main execution
try {
    Write-Output "ATDD Accelerator Setup Script"
    Write-Output "Repository Name: $RepositoryName"
    Write-Output "System Language: $SystemLanguage"
    Write-Output "System Test Language: $SystemTestLanguage"
    
    Test-SystemLanguage -SystemLanguage $SystemLanguage
    Test-SystemLanguage -SystemLanguage $SystemTestLanguage

    $GitHubUsername = Get-GitHubUsername -ProvidedUsername $GitHubUsername
    Write-Output "GitHub Username: $GitHubUsername"
    
    New-RepositoryFromTemplate -RepositoryName $RepositoryName
    
    Write-Output ""
    Write-Output "✅ Repository setup completed successfully!"
    Write-Output "Repository: $GitHubUsername/$RepositoryName"
    Write-Output "System Language: $SystemLanguage"
    Write-Output "System Test Language: $SystemTestLanguage"
    
    exit 0
    
} catch {
    Write-Error "Setup failed: $($_.Exception.Message)"
    exit 1
}