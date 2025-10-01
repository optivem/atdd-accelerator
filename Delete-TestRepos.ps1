# Delete test repositories created after a specific cut-off time
$cutoffTime = [DateTime]"2025-09-29T08:50:00"  # Local time

Write-Host "Cut-off time: $cutoffTime" -ForegroundColor Yellow

# First, let's see what repositories match our criteria
$reposToDelete = gh repo list --json name,createdAt --limit 10 | ConvertFrom-Json | Where-Object { [DateTime]$_.createdAt -gt $cutoffTime }

Write-Host "Found $($reposToDelete.Count) repositories to delete:" -ForegroundColor Yellow
$reposToDelete | ForEach-Object {
    Write-Host "  - $($_.name) (created: $($_.createdAt))" -ForegroundColor Cyan
}

if ($reposToDelete.Count -gt 0) {
    Write-Host "Starting deletion..." -ForegroundColor Red
    $reposToDelete | ForEach-Object { 
        Write-Host "Deleting $($_.name)" -ForegroundColor Red
        gh repo delete $_.name --yes 
    }
    Write-Host "Deletion completed!" -ForegroundColor Green
} else {
    Write-Host "No repositories found matching the criteria." -ForegroundColor Green
}