function Invoke-SystemTestWorkflows {
    param(
        [string]$SystemTestLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName
    )
    
    Write-Output "Triggering system test workflows for language: $SystemTestLanguage"
    
    # Define workflows to trigger based on your Badges.java
    $workflows = @(
        "local-acceptance-stage-test-$($SystemTestLanguage.ToLower())",
        "acceptance-stage-test-$($SystemTestLanguage.ToLower())",
        "qa-stage-test-$($SystemTestLanguage.ToLower())",
        "prod-stage-test-$($SystemTestLanguage.ToLower())"
    )
    
    $triggeredWorkflows = @()
    $failedWorkflows = @()
    
    foreach ($workflow in $workflows) {
        Write-Output "Triggering workflow: $workflow"
        
        try {
            # Trigger the workflow using GitHub CLI
            gh workflow run $workflow --repo "$RepositoryOwner/$RepositoryName"
            
            if ($LASTEXITCODE -eq 0) {
                Write-Output "   Successfully triggered: $workflow"
                $triggeredWorkflows += $workflow
            } else {
                Write-Warning "   Failed to trigger: $workflow (exit code: $LASTEXITCODE)"
                $failedWorkflows += $workflow
            }
        } catch {
            Write-Warning "   Error triggering $workflow : $_"
            $failedWorkflows += $workflow
        }
        
        # Small delay between triggers to avoid rate limiting
        Start-Sleep -Milliseconds 500
    }
    
    # Summary
    Write-Output ""
    Write-Output "Workflow trigger summary:"
    if ($triggeredWorkflows.Count -gt 0) {
        Write-Output "   Successfully triggered ($($triggeredWorkflows.Count)):"
        $triggeredWorkflows | ForEach-Object { Write-Output "    - $_" }
    }
    
    if ($failedWorkflows.Count -gt 0) {
        Write-Warning "   Failed to trigger ($($failedWorkflows.Count)):"
        $failedWorkflows | ForEach-Object { Write-Warning "    - $_" }
        Write-Warning "You may need to trigger these workflows manually in GitHub Actions"
    }
    
    return $triggeredWorkflows.Count -gt 0
}
