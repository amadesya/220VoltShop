# Seed database script
# Usage: from workspace root run: PowerShell -ExecutionPolicy Bypass -File .\scripts\seed-db.ps1

Write-Output "Applying EF migrations..."
dotnet ef database update --project "courseApi" --no-build

Write-Output "Calling API seed endpoint..."
try {
    $res = Invoke-WebRequest -Uri http://localhost:5000/api/dev/seed -Method Post -UseBasicParsing -TimeoutSec 30 -ErrorAction Stop
    Write-Output "Seed response: $($res.StatusCode)"
    Write-Output $res.Content
} catch {
    Write-Error "Failed to call seed endpoint. Ensure courseApi is running on http://localhost:5000 and you are in Development environment. $_"
}

Write-Output "Done."
