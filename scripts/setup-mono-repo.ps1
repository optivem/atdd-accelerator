param(
    [Parameter(Mandatory=$true)]
    [string]$RepositoryName,
    
    [Parameter(Mandatory=$false)]
    [string]$GitHubUsername
)

try {
    Write-Output "ATDD Accelerator Setup Script"
    Write-Output "Repository Name: $RepositoryName"
    
    # Get GitHub username if not provided
    if (-not $GitHubUsername) {
        $authStatus = gh auth status 2>&1
        if ($LASTEXITCODE -ne 0) {
            throw "GitHub CLI not authenticated. Please run: gh auth login"
        }
        
        $GitHubUsername = ($authStatus | Select-String "Logged in to github\.com account (.+?) " | ForEach-Object { $_.Matches[0].Groups[1].Value })
        if (-not $GitHubUsername) {
            $GitHubUsername = ($authStatus | Select-String "Logged in to github\.com as (.+)" | ForEach-Object { $_.Matches[0].Groups[1].Value })
        }
    }
    
    Write-Output "GitHub Username: $GitHubUsername"
    
    # Create repository from template
    Write-Output "Creating repository from template..."
    gh repo create $RepositoryName --template "optivem/atdd-accelerator-template-mono-repo" --public
    
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create repository"
    }
    
    Write-Output "Repository created successfully: $GitHubUsername/$RepositoryName"
    Write-Output "Setup completed successfully"
    
    exit 0
    
} catch {
    Write-Error "Setup failed: $_"
    exit 1
}