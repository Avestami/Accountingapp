# PowerShell script to fix Result.Failure calls to use explicit generic types

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
    
    # Extract the return type from method signature
    if ($content -match "public async Task<Result<([^>]+)>>\s+Handle\(") {
        $returnType = $matches[1]
        Write-Host "Processing $($file.Name) with return type: $returnType"
        
        # Replace Result<Type>.Failure("message") patterns that might be using implicit conversion
        # Look for return statements that might be causing implicit conversion issues
        $content = $content -replace 'return Result<' + [regex]::Escape($returnType) + '>\.Failure\("([^"]+)"\);', 'return Result.Failure<' + $returnType + '>($1);'
        
        # Also handle the case where it's already correct but might have spacing issues
        $content = $content -replace 'return\s+Result<' + [regex]::Escape($returnType) + '>\.Failure\s*\(\s*"([^"]+)"\s*\)\s*;', 'return Result.Failure<' + $returnType + '>($1);'
    }
    
    # Save the file if it was modified
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Updated: $($file.Name)"
    }
}

Write-Host "Completed fixing Result.Failure calls"