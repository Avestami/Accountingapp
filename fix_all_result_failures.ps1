# PowerShell script to fix all Result<T>.Failure calls to use Result.Failure<T>

$applicationPath = "src\Accounting.Application"

# Get all C# files in the application
$files = Get-ChildItem -Path $applicationPath -Recurse -Filter "*.cs"

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Skip if file doesn't contain Result<
    if ($content -notmatch "Result<") {
        continue
    }
    
    Write-Host "Processing $($file.Name)"
    
    # Replace all Result<Type>.Failure("message") with Result.Failure<Type>("message")
    $content = $content -replace 'Result<([^>]+)>\.Failure\(', 'Result.Failure<$1>('
    
    # Save the file if it was modified
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Updated: $($file.Name)"
    }
}

Write-Host "Completed fixing all Result<T>.Failure calls"