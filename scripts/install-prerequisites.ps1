# ATDD Accelerator - Prerequisites Installation Script
# This script checks and installs PowerShell 7+, GitHub CLI, and Git

[CmdletBinding()]
param(
    [Parameter(Mandatory=$false, HelpMessage="Skip interactive prompts")]
    [switch]$Force
)

# Color output functions
function Write-Success { param($Message) Write-Host "✅ $Message" -ForegroundColor Green }
function Write-Info { param($Message) Write-Host "ℹ️  $Message" -ForegroundColor Cyan }
function Write-Warning { param($Message) Write-Host "⚠️  $Message" -ForegroundColor Yellow }
function Write-Error { param($Message) Write-Host "❌ $Message" -ForegroundColor Red }
function Write-Step { param($Message) Write-Host "🔄 $Message" -ForegroundColor Blue }

function Confirm-Installation {
    param([string]$Tool)
    if ($Force) { return $true }
    $response = Read-Host "Do you want to install $Tool? (y/N)"
    return ($response -eq 'y' -or $response -eq 'Y')
}

# Detect OS
$IsWindows = $env:OS -eq "Windows_NT"
$IsLinux = $PSVersionTable.Platform -eq "Unix" -and (Get-Content /etc/os-release -ErrorAction SilentlyContinue | Select-String "ID=.*linux")
$IsMacOS = $PSVersionTable.Platform -eq "Unix" -and !(Get-Content /etc/os-release -ErrorAction SilentlyContinue | Select-String "ID=.*linux")

Write-Host @"
🚀 ATDD Accelerator Prerequisites Setup
======================================
OS Detected: $($IsWindows ? 'Windows' : $IsLinux ? 'Linux' : $IsMacOS ? 'macOS' : 'Unknown')
PowerShell Version: $($PSVersionTable.PSVersion)
"@ -ForegroundColor Magenta

# 1. CHECK AND INSTALL POWERSHELL 7+
Write-Step "Checking PowerShell 7+..."

$pwshInstalled = $false
$currentPwshVersion = $null

# Check if pwsh (PowerShell 7+) is available
try {
    if (Get-Command pwsh -ErrorAction SilentlyContinue) {
        $currentPwshVersion = & pwsh -Command '$PSVersionTable.PSVersion.ToString()'
        Write-Success "PowerShell 7+ is installed: v$currentPwshVersion"
        $pwshInstalled = $true
    }
}
catch {
    # Ignore error
}

# Check current PowerShell version if pwsh not found
if (-not $pwshInstalled) {
    $currentVersion = $PSVersionTable.PSVersion
    if ($currentVersion.Major -ge 7) {
        Write-Success "PowerShell 7+ is already running: v$($currentVersion.ToString())"
        $pwshInstalled = $true
    }
    else {
        Write-Warning "PowerShell 5.1 detected. PowerShell 7+ recommended for cross-platform compatibility."
        
        if (Confirm-Installation "PowerShell 7+") {
            Write-Step "Installing PowerShell 7+..."
            
            if ($IsWindows) {
                # Try winget first, then manual download
                try {
                    if (Get-Command winget -ErrorAction SilentlyContinue) {
                        winget install --id Microsoft.PowerShell --accept-source-agreements --accept-package-agreements
                        Write-Success "PowerShell 7+ installed via winget"
                        $pwshInstalled = $true
                    }
                    else {
                        Write-Info "Winget not available. Please download PowerShell 7+ from:"
                        Write-Info "https://github.com/PowerShell/PowerShell/releases"
                    }
                }
                catch {
                    Write-Warning "Winget installation failed. Please install manually from:"
                    Write-Warning "https://github.com/PowerShell/PowerShell/releases"
                }
            }
            elseif ($IsLinux) {
                # Try different package managers
                Write-Step "Attempting to install via package manager..."
                try {
                    if (Get-Command apt-get -ErrorAction SilentlyContinue) {
                        # Ubuntu/Debian
                        sudo apt-get update
                        sudo apt-get install -y wget apt-transport-https software-properties-common
                        wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
                        sudo dpkg -i packages-microsoft-prod.deb
                        sudo apt-get update
                        sudo apt-get install -y powershell
                        Write-Success "PowerShell 7+ installed via apt"
                        $pwshInstalled = $true
                    }
                    elseif (Get-Command yum -ErrorAction SilentlyContinue) {
                        # RHEL/CentOS
                        sudo yum install -y https://packages.microsoft.com/config/rhel/8/packages-microsoft-prod.rpm
                        sudo yum install -y powershell
                        Write-Success "PowerShell 7+ installed via yum"
                        $pwshInstalled = $true
                    }
                    elseif (Get-Command snap -ErrorAction SilentlyContinue) {
                        # Snap
                        sudo snap install powershell --classic
                        Write-Success "PowerShell 7+ installed via snap"
                        $pwshInstalled = $true
                    }
                }
                catch {
                    Write-Warning "Package manager installation failed. Please install manually:"
                    Write-Warning "https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-linux"
                }
            }
            elseif ($IsMacOS) {
                try {
                    if (Get-Command brew -ErrorAction SilentlyContinue) {
                        brew install powershell
                        Write-Success "PowerShell 7+ installed via Homebrew"
                        $pwshInstalled = $true
                    }
                    else {
                        Write-Warning "Homebrew not found. Please install PowerShell manually:"
                        Write-Warning "https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-macos"
                    }
                }
                catch {
                    Write-Warning "Homebrew installation failed. Please install manually."
                }
            }
        }
    }
}

# 2. CHECK AND INSTALL GIT
Write-Step "Checking Git..."

$gitInstalled = $false
try {
    $gitVersion = git --version 2>$null
    if ($gitVersion) {
        Write-Success "Git is installed: $gitVersion"
        $gitInstalled = $true
    }
}
catch {
    # Git not found
}

if (-not $gitInstalled) {
    Write-Warning "Git is not installed"
    
    if (Confirm-Installation "Git") {
        Write-Step "Installing Git..."
        
        if ($IsWindows) {
            try {
                if (Get-Command winget -ErrorAction SilentlyContinue) {
                    winget install --id Git.Git --accept-source-agreements --accept-package-agreements
                    Write-Success "Git installed via winget"
                    $gitInstalled = $true
                }
                else {
                    Write-Info "Please download Git from: https://git-scm.com/download/windows"
                }
            }
            catch {
                Write-Warning "Winget installation failed. Download from: https://git-scm.com/download/windows"
            }
        }
        elseif ($IsLinux) {
            try {
                if (Get-Command apt-get -ErrorAction SilentlyContinue) {
                    sudo apt-get update && sudo apt-get install -y git
                    Write-Success "Git installed via apt"
                    $gitInstalled = $true
                }
                elseif (Get-Command yum -ErrorAction SilentlyContinue) {
                    sudo yum install -y git
                    Write-Success "Git installed via yum"
                    $gitInstalled = $true
                }
                elseif (Get-Command dnf -ErrorAction SilentlyContinue) {
                    sudo dnf install -y git
                    Write-Success "Git installed via dnf"
                    $gitInstalled = $true
                }
            }
            catch {
                Write-Warning "Package manager installation failed."
            }
        }
        elseif ($IsMacOS) {
            try {
                if (Get-Command brew -ErrorAction SilentlyContinue) {
                    brew install git
                    Write-Success "Git installed via Homebrew"
                    $gitInstalled = $true
                }
                else {
                    Write-Info "Install Homebrew first: https://brew.sh/"
                    Write-Info "Or download Git from: https://git-scm.com/download/mac"
                }
            }
            catch {
                Write-Warning "Homebrew installation failed."
            }
        }
    }
}

# 3. CHECK AND INSTALL GITHUB CLI
Write-Step "Checking GitHub CLI..."

$ghInstalled = $false
try {
    $ghVersion = gh --version 2>$null
    if ($ghVersion) {
        Write-Success "GitHub CLI is installed:"
        Write-Info $ghVersion
        $ghInstalled = $true
    }
}
catch {
    # GitHub CLI not found
}

if (-not $ghInstalled) {
    Write-Warning "GitHub CLI is not installed"
    
    if (Confirm-Installation "GitHub CLI") {
        Write-Step "Installing GitHub CLI..."
        
        if ($IsWindows) {
            try {
                if (Get-Command winget -ErrorAction SilentlyContinue) {
                    winget install --id GitHub.cli --accept-source-agreements --accept-package-agreements
                    Write-Success "GitHub CLI installed via winget"
                    $ghInstalled = $true
                }
                else {
                    Write-Info "Please download GitHub CLI from: https://cli.github.com/"
                }
            }
            catch {
                Write-Warning "Winget installation failed. Download from: https://cli.github.com/"
            }
        }
        elseif ($IsLinux) {
            try {
                if (Get-Command apt-get -ErrorAction SilentlyContinue) {
                    # Ubuntu/Debian
                    curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | sudo dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg
                    echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | sudo tee /etc/apt/sources.list.d/github-cli.list > /dev/null
                    sudo apt-get update && sudo apt-get install -y gh
                    Write-Success "GitHub CLI installed via apt"
                    $ghInstalled = $true
                }
                elseif (Get-Command yum -ErrorAction SilentlyContinue) {
                    # RHEL/CentOS
                    sudo yum install -y yum-utils
                    sudo yum-config-manager --add-repo https://cli.github.com/packages/rpm/gh-cli.repo
                    sudo yum install -y gh
                    Write-Success "GitHub CLI installed via yum"
                    $ghInstalled = $true
                }
                elseif (Get-Command snap -ErrorAction SilentlyContinue) {
                    sudo snap install gh
                    Write-Success "GitHub CLI installed via snap"
                    $ghInstalled = $true
                }
            }
            catch {
                Write-Warning "Package manager installation failed. See: https://cli.github.com/manual/installation"
            }
        }
        elseif ($IsMacOS) {
            try {
                if (Get-Command brew -ErrorAction SilentlyContinue) {
                    brew install gh
                    Write-Success "GitHub CLI installed via Homebrew"
                    $ghInstalled = $true
                }
                else {
                    Write-Info "Install Homebrew first: https://brew.sh/"
                    Write-Info "Or download from: https://cli.github.com/"
                }
            }
            catch {
                Write-Warning "Homebrew installation failed."
            }
        }
    }
}

# 4. AUTHENTICATION SETUP
if ($ghInstalled) {
    Write-Step "Checking GitHub CLI authentication..."
    
    try {
        gh auth status 2>$null
        Write-Success "GitHub CLI is already authenticated"
    }
    catch {
        Write-Warning "GitHub CLI is not authenticated"
        Write-Info "Please run: gh auth login"
        Write-Info "This will open a browser for GitHub authentication"
        
        if (-not $Force) {
            $response = Read-Host "Do you want to authenticate now? (y/N)"
            if ($response -eq 'y' -or $response -eq 'Y') {
                try {
                    gh auth login
                    Write-Success "GitHub CLI authentication completed"
                }
                catch {
                    Write-Warning "Authentication setup incomplete. Please run 'gh auth login' manually"
                }
            }
        }
    }
}

# 5. FINAL SUMMARY
Write-Host @"

🎉 PREREQUISITES SETUP SUMMARY
===============================
"@ -ForegroundColor Green

Write-Host "PowerShell 7+: " -NoNewline
if ($pwshInstalled -or $PSVersionTable.PSVersion.Major -ge 7) {
    Write-Success "✅ Ready"
} else {
    Write-Error "❌ Not installed"
}

Write-Host "Git: " -NoNewline
if ($gitInstalled) {
    Write-Success "✅ Ready"
} else {
    Write-Error "❌ Not installed"
}

Write-Host "GitHub CLI: " -NoNewline
if ($ghInstalled) {
    Write-Success "✅ Ready"
} else {
    Write-Error "❌ Not installed"
}

Write-Host @"

📋 NEXT STEPS:
1. Restart your terminal/PowerShell session
2. Verify installations:
   - pwsh --version
   - git --version  
   - gh --version
3. If GitHub CLI is installed but not authenticated: gh auth login
4. Run the main setup script: .\setup-mono-repo.ps1

🔗 USEFUL LINKS:
- PowerShell: https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell
- Git: https://git-scm.com/downloads
- GitHub CLI: https://cli.github.com/
"@ -ForegroundColor Yellow

# Refresh environment (Windows)
if ($IsWindows) {
    Write-Info "You may need to restart your terminal or run 'refreshenv' if using Chocolatey"
}