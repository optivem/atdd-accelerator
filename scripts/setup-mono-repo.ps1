# ATDD Accelerator - Mono Repo Setup Script
# This script automates the Mono Repo Quickstart process

[CmdletBinding()]
param(
      # Clean up System Language folders
    Write-Step "Cleanin    # Clean up System Test Language folders
    Write-Step "Cleaning up System Test Language folders (keeping: $SystemT    # Commit and push changes
    Write-Step "Committing and pushing changes..."
    
    try {
        git add -A
        $commitMessage = "chore: automated setup for $SystemLanguage system with $SystemTestLanguage tests"
        git commit -m $commitMessage
        git push
        Write-Success "Changes committed and pushed"
    }
    catch {
        Write-Warning "Git operations failed: $($_.Exception.Message)"
        Write-Info "You may need to commit and push manually"
    }
    
    # Final summary and next steps
    
    $testFolders = @("system-test-dotnet", "system-test-java", "system-test-typescript")
    $keepTestFolder = "system-test-$($SystemTestLanguage.ToLower())"
    
    foreach ($folder in $testFolders) {
        if ($folder -eq $keepTestFolder) {
            Write-Success "Keeping $folder"
        }
        else {
            if (Test-Path $folder) {
                Remove-Item -Path $folder -Recurse -Force
                Write-Success "Removed $folder"
            }
        }
    }
    
    # Clean up System Test Language workflow filese folders (keeping: $SystemLanguage)..."
    
    $systemFolders = @("monolith-dotnet", "monolith-java", "monolith-typescript")
    $keepSystemFolder = "monolith-$($SystemLanguage.ToLower())"
    
    foreach ($folder in $systemFolders) {
        if ($folder -eq $keepSystemFolder) {
            Write-Success "Keeping $folder"
        }
        else {
            if (Test-Path $folder) {
                Remove-Item -Path $folder -Recurse -Force
                Write-Success "Removed $folder"
            }
        }
    }
    
    # Clean up System Language workflow filesory=$true, HelpMessage="Repository name (e.g., eshop)")]
    [ValidateNotNullOrEmpty()]
    [string]$RepositoryName,
    
    [Parameter(Mandatory=$true, HelpMessage="Choose System Language")]
    [ValidateSet("Java", "DotNet", "TypeScript")]
    [string]$SystemLanguage,
    
    [Parameter(Mandatory=$true, HelpMessage="Choose System Test Language")]
    [ValidateSet("Java", "DotNet", "TypeScript")]
    [string]$SystemTestLanguage,
    
    [Parameter(Mandatory=$false, HelpMessage="GitHub username (auto-detected if not provided)")]
    [string]$GitHubUsername,
    
    [Parameter(Mandatory=$false, HelpMessage="Skip cloning if repository already exists locally")]
    [switch]$SkipClone
)

# Color output functions
function Write-Success { param($Message) Write-Host "[SUCCESS] $Message" -ForegroundColor Green }
function Write-Info { param($Message) Write-Host "[INFO] $Message" -ForegroundColor Cyan }
function Write-Warning { param($Message) Write-Host "[WARNING] $Message" -ForegroundColor Yellow }
function Write-Error { param($Message) Write-Host "[ERROR] $Message" -ForegroundColor Red }
function Write-Step { param($Message) Write-Host "[STEP] $Message" -ForegroundColor Blue }

# Main execution
try {
    Write-Host @"
ATDD Accelerator Mono Repo Setup
====================================
Repository Name: $RepositoryName
System Language: $SystemLanguage
Test Language: $SystemTestLanguage
"@ -ForegroundColor Magenta

    # Validate prerequisites
    Write-Step "Validating prerequisites..."
    
    # Check GitHub CLI
    if (!(Get-Command gh -ErrorAction SilentlyContinue)) {
        Write-Error "GitHub CLI (gh) is not installed."
        Write-Info "Please install it from: https://cli.github.com/"
        Write-Info "Then run: gh auth login"
        exit 1
    }
    
    # Check authentication
    try {
        $null = gh auth status 2>$null
        Write-Success "GitHub CLI authenticated"
    }
    catch {
        Write-Error "Please authenticate with GitHub CLI first: gh auth login"
        exit 1
    }
    
    # Get GitHub username
    if (-not $GitHubUsername) {
        try {
            $GitHubUsername = (gh api user --jq '.login').Trim()
            Write-Info "Auto-detected GitHub username: $GitHubUsername"
        }
        catch {
            Write-Error "Could not detect GitHub username. Please provide it manually with -GitHubUsername parameter."
            exit 1
        }
    }
    
    # Create repository from template
    Write-Step "Creating repository '$RepositoryName' from template..."
    
    try {
        $createOutput = gh repo create $RepositoryName --template optivem/atdd-accelerator-template-mono-repo --public --confirm 2>&1
        Write-Success "Repository created successfully"
        Write-Info "Repository URL: https://github.com/$GitHubUsername/$RepositoryName"
    }
    catch {
        Write-Warning "Repository creation failed or repository already exists"
        Write-Info "Continuing with existing repository..."
    }
    
    # Configure GitHub Actions permissions (immediately after creation)
    Write-Step "Configuring GitHub Actions permissions..."
    
    try {
        # Set workflow permissions to read and write
        $permissionResult = gh api repos/$GitHubUsername/$RepositoryName --method PATCH --field actions_permissions_enabled=true --field actions_default_workflow_permissions=write --field actions_can_approve_pull_request_reviews=true 2>&1
        Write-Success "GitHub Actions permissions configured (read and write)"
    }
    catch {
        Write-Warning "Failed to set GitHub Actions permissions automatically: $($_.Exception.Message)"
        Write-Info "You may need to set permissions manually at: https://github.com/$GitHubUsername/$RepositoryName/settings/actions"
    }
    
    # Clone repository (if not skipped)
    # Clone repository (if not skipped)
    if (-not $SkipClone) {
        Write-Step "Cloning repository..."
        
        if (Test-Path $RepositoryName) {
            Write-Warning "Directory '$RepositoryName' already exists"
            $response = Read-Host "Do you want to continue and overwrite? (y/N)"
            if ($response -ne 'y' -and $response -ne 'Y') {
                Write-Info "Operation cancelled by user"
                exit 0
            }
            Remove-Item -Path $RepositoryName -Recurse -Force
        }
        
        try {
            git clone "https://github.com/$GitHubUsername/$RepositoryName" 2>&1 | Out-Null
            Write-Success "Repository cloned successfully"
        }
        catch {
            Write-Error "Failed to clone repository: $($_.Exception.Message)"
            exit 1
        }
    }
    
    # Change to repository directory
    if (-not (Test-Path $RepositoryName)) {
        Write-Error "Repository directory '$RepositoryName' not found"
        exit 1
    }
    
    Push-Location $RepositoryName
    Write-Info "Working in directory: $(Get-Location)"
    
    # Clean up System Language folders
    Write-Step "Cleaning up System Language folders (keeping: $SystemLanguage)..."
    
    $systemFolders = @("monolith-dotnet", "monolith-java", "monolith-typescript")
    $keepSystemFolder = "monolith-$($SystemLanguage.ToLower())"
    
    foreach ($folder in $systemFolders) {
        if ($folder -eq $keepSystemFolder) {
            Write-Success "Keeping $folder"
        }
        else {
            if (Test-Path $folder) {
                Remove-Item -Path $folder -Recurse -Force
                Write-Success "Removed $folder"
            }
        }
    }
    
    # Clean up System Language workflow files
    Write-Step "Cleaning up System Language workflow files..."
    
    $workflowPath = ".github/workflows"
    if (Test-Path $workflowPath) {
        $systemWorkflows = Get-ChildItem "$workflowPath/commit-stage-monolith-*.yml" -ErrorAction SilentlyContinue
        $keepSystemWorkflow = "commit-stage-monolith-$($SystemLanguage.ToLower()).yml"
        
        Write-Info "Looking for workflow files in: $workflowPath"
        Write-Info "Keep workflow: $keepSystemWorkflow"
        
        foreach ($workflow in $systemWorkflows) {
            Write-Info "Found workflow: $($workflow.Name)"
            if ($workflow.Name -eq $keepSystemWorkflow) {
                Write-Success "Keeping $($workflow.Name)"
            }
            else {
                Remove-Item -Path $workflow.FullName -Force
                Write-Success "Removed $($workflow.Name)"
            }
        }
        
        # Verify what's left
        $remainingWorkflows = Get-ChildItem "$workflowPath/commit-stage-monolith-*.yml" -ErrorAction SilentlyContinue
        Write-Info "Remaining commit-stage workflows: $($remainingWorkflows.Name -join ', ')"
    }
    
    # Clean up System Test Language folders
    Write-Step "Cleaning up System Test Language folders (keeping: $SystemTestLanguage)..."
    
    $testFolders = @("system-test-dotnet", "system-test-java", "system-test-typescript")
    $keepTestFolder = "system-test-$($SystemTestLanguage.ToLower())"
    
    foreach ($folder in $testFolders) {
        if ($folder -eq $keepTestFolder) {
            Write-Success "Keeping $folder"
        }
        else {
            if (Test-Path $folder) {
                Remove-Item -Path $folder -Recurse -Force
                Write-Success "Removed $folder"
            }
        }
    }
    
    # Clean up System Test Language workflow files
    Write-Step "Cleaning up System Test Language workflow files..."
    
    if (Test-Path $workflowPath) {
        $testWorkflows = @(
            Get-ChildItem "$workflowPath/local-acceptance-stage-*.yml" -ErrorAction SilentlyContinue
            Get-ChildItem "$workflowPath/release-stage-*.yml" -ErrorAction SilentlyContinue
        )
        
        $keepTestWorkflows = @(
            "local-acceptance-stage-$($SystemTestLanguage.ToLower()).yml",
            "release-stage-$($SystemTestLanguage.ToLower()).yml"
        )
        
        Write-Info "Keep test workflows: $($keepTestWorkflows -join ', ')"
        
        foreach ($workflow in $testWorkflows) {
            Write-Info "Found test workflow: $($workflow.Name)"
            if ($workflow.Name -in $keepTestWorkflows) {
                Write-Success "Keeping $($workflow.Name)"
            }
            else {
                Remove-Item -Path $workflow.FullName -Force
                Write-Success "Removed $($workflow.Name)"
            }
        }
        
        # Verify what's left
        $remainingTestWorkflows = @(
            Get-ChildItem "$workflowPath/local-acceptance-stage-*.yml" -ErrorAction SilentlyContinue
            Get-ChildItem "$workflowPath/release-stage-*.yml" -ErrorAction SilentlyContinue
        )
        Write-Info "Remaining test workflows: $($remainingTestWorkflows.Name -join ', ')"
    }
    
    # Update README.md badges and paths
    Write-Step "Updating README.md with repository-specific paths..."
    
    if (Test-Path "README.md") {
        $readmeContent = Get-Content "README.md" -Raw
        
        # Replace repository paths
        $readmeContent = $readmeContent -replace "optivem/atdd-accelerator-template-mono-repo", "$GitHubUsername/$RepositoryName"
        
        # Remove unused system badges
        $systemLanguages = @("dotnet", "java", "typescript")
        foreach ($lang in $systemLanguages) {
            if ($lang -ne $SystemLanguage.ToLower()) {
                $badgePattern = "\\[!\\[commit-stage-monolith-$lang\\].*?\\)\\)"
                $readmeContent = $readmeContent -replace $badgePattern, ""
            }
        }
        
        # Remove unused release badges
        foreach ($lang in $systemLanguages) {
            if ($lang -ne $SystemTestLanguage.ToLower()) {
                $badgePattern = "\\[!\\[release-stage-$lang\\].*?\\)\\)"
                $readmeContent = $readmeContent -replace $badgePattern, ""
            }
        }
        
        # Clean up extra newlines
        $readmeContent = $readmeContent -replace "(`r?`n){3,}", "`r`n`r`n"
        
        Set-Content "README.md" -Value $readmeContent -NoNewline
        Write-Success "Updated README.md"
    }
    
    # Update docker-compose.yml in system test folder
    Write-Step "Updating docker-compose configuration..."
    
    $dockerComposePath = "$keepTestFolder/docker-compose.yml"
    if (Test-Path $dockerComposePath) {
        $composeContent = Get-Content $dockerComposePath -Raw
        
        # Update image path
        $oldImagePattern = "ghcr\.io/optivem/atdd-accelerator-template-mono-repo/monolith-\w+:latest"
        $newImage = "ghcr.io/$GitHubUsername/$RepositoryName/monolith-$($SystemLanguage.ToLower()):latest"
        $composeContent = $composeContent -replace $oldImagePattern, $newImage
        
        Set-Content $dockerComposePath -Value $composeContent -NoNewline
        Write-Success "Updated docker-compose.yml"
    }
    
    # Commit and push changes
    Write-Step "Committing and pushing changes..."
    
    try {
        git add -A
        $commitMessage = "chore: automated setup for $SystemLanguage system with $SystemTestLanguage tests"
        git commit -m $commitMessage
        git push
        Write-Success "Changes committed and pushed"
    }
    catch {
        Write-Warning "Git operations failed: $($_.Exception.Message)"
        Write-Info "You may need to commit and push manually"
    }
    
    # Final summary and next steps
    Write-Host @"

SETUP COMPLETE!
==================
Repository: https://github.com/$GitHubUsername/$RepositoryName
System Language: $SystemLanguage
Test Language: $SystemTestLanguage

NEXT STEPS:
1. ✅ GitHub Actions permissions: Automatically configured for Docker image publishing
2. Wait for GitHub Actions to complete building (check: https://github.com/$GitHubUsername/$RepositoryName/actions)
3. Verify the system status badge shows 'passing'
4. Set up GitHub Pages for documentation:
   - Go to Settings > Pages
   - Source: Deploy from a branch
   - Branch: main, Folder: /docs
5. Run the release workflow to verify system tests
6. Update project naming throughout the codebase

Quick Links:
- Repository: https://github.com/$GitHubUsername/$RepositoryName
- Actions: https://github.com/$GitHubUsername/$RepositoryName/actions
- Settings: https://github.com/$GitHubUsername/$RepositoryName/settings
"@ -ForegroundColor Green

}
catch {
    Write-Error "Setup failed: $($_.Exception.Message)"
    Write-Info "You can run the script again or complete the setup manually"
    exit 1
}
finally {
    if (Get-Location | Where-Object { $_.Path.EndsWith($RepositoryName) }) {
        Pop-Location
    }
}

# Usage examples
Write-Host @"

USAGE EXAMPLES:
# Basic usage
.\setup-mono-repo.ps1 -RepositoryName "eshop" -SystemLanguage "Java" -SystemTestLanguage "TypeScript"

# With specific GitHub username
.\setup-mono-repo.ps1 -RepositoryName "my-project" -SystemLanguage "DotNet" -SystemTestLanguage "Java" -GitHubUsername "myuser"

# Skip cloning (if repo already exists locally)
.\setup-mono-repo.ps1 -RepositoryName "existing-repo" -SystemLanguage "TypeScript" -SystemTestLanguage "TypeScript" -SkipClone
"@ -ForegroundColor Yellow