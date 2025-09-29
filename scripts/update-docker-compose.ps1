function Update-DockerComposeFiles {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName
    )
    
    Write-Output "Updating Docker Compose files..."
    
    # Find all docker-compose files in system-test folders
    $dockerComposeFiles = Get-ChildItem -Path "system-test-*" -Include "docker-compose.yml" -Recurse -ErrorAction SilentlyContinue
    
    $filesUpdated = $false
    
    foreach ($file in $dockerComposeFiles) {
        Write-Output "Updating Docker Compose file: $($file.FullName)"
        
        $content = Get-Content $file.FullName -Raw
        $originalContent = $content
        
        # First, update repository references
        $content = $content -replace "ghcr\.io/optivem/atdd-accelerator-template-mono-repo/", "ghcr.io/$RepositoryOwner/$RepositoryName/"
        
        # Define the target language service
        $targetService = "monolith-$($SystemLanguage.ToLower())"
        
        # Split content into lines for processing
        $lines = $content -split "`n"
        $newLines = @()
        
        $i = 0
        while ($i -lt $lines.Length) {
            $line = $lines[$i]
            
            # Check if this line starts a monolith service block
            if ($line -match "^\s*#?\s*monolith:\s*$") {
                # Look ahead to find the image line to determine the language
                $serviceLanguage = ""
                $serviceLines = @()
                $serviceLines += $line
                
                $j = $i + 1
                while ($j -lt $lines.Length -and $lines[$j] -match "^\s+(#\s*)?[^a-zA-Z]") {
                    $serviceLines += $lines[$j]
                    if ($lines[$j] -match "image:.*/(monolith-(java|dotnet|typescript)):latest") {
                        $serviceLanguage = $matches[2]
                    }
                    $j++
                }
                
                # If this is the target language, keep and uncomment the service
                if ($serviceLanguage -eq $SystemLanguage.ToLower()) {
                    foreach ($serviceLine in $serviceLines) {
                        # Uncomment and add to new lines
                        $cleanLine = $serviceLine -replace "^\s*#\s*", ""
                        if ($cleanLine -match "^monolith:\s*$") {
                            $newLines += "monolith:"
                        } elseif ($cleanLine -match "^\s*image:") {
                            $newLines += "  image: " + ($cleanLine -replace "^\s*image:\s*", "")
                        } elseif ($cleanLine -match "^\s*ports:") {
                            $newLines += "  ports:"
                        } elseif ($cleanLine -match "^\s*-\s*") {
                            $newLines += "    " + ($cleanLine -replace "^\s*-\s*", "- ")
                        } else {
                            $newLines += $cleanLine
                        }
                    }
                }
                # If not target language, skip this entire service block
                
                # Move index past this service block
                $i = $j - 1
            } else {
                # Not a monolith service line, keep as-is
                $newLines += $line
            }
            $i++
        }
        
        # Join lines back together
        $newContent = $newLines -join "`n"
        
        if ($newContent -ne $originalContent) {
            Set-Content -Path $file.FullName -Value $newContent -NoNewline
            git add $file.FullName
            $filesUpdated = $true
            Write-Output "Updated: $($file.FullName)"
        }
    }
    
    return $filesUpdated
}