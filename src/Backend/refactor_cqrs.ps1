$ErrorActionPreference = 'Stop'

$basePath = "C:\Users\vanil\source\repos\IOrderNew\src\Backend\IOrder.Application\UseCases"
$folders = @("Product", "Category", "Store", "StoreCategory")

foreach ($folder in $folders) {
    $folderPath = Join-Path $basePath $folder
    if (-not (Test-Path $folderPath)) { continue }

    # Create Commands and Queries folders
    $commandsPath = Join-Path $folderPath "Commands"
    $queriesPath = Join-Path $folderPath "Queries"
    
    if (-not (Test-Path $commandsPath)) { New-Item -ItemType Directory -Path $commandsPath | Out-Null }
    if (-not (Test-Path $queriesPath)) { New-Item -ItemType Directory -Path $queriesPath | Out-Null }

    # Get all .cs files in the root of the entity folder
    $files = Get-ChildItem -Path $folderPath -File -Filter "*.cs"

    foreach ($file in $files) {
        $name = $file.Name
        
        # Determine if it's a Query or Command
        # Queries usually start with Get
        if ($name -match "^I?Get") {
            $destPath = Join-Path $queriesPath $name
            $newNamespace = "IOrder.Application.UseCases.${folder}.Queries"
        } else {
            $destPath = Join-Path $commandsPath $name
            $newNamespace = "IOrder.Application.UseCases.${folder}.Commands"
        }

        # Move file
        Move-Item -Path $file.FullName -Destination $destPath

        # Update namespace in the file
        $content = Get-Content $destPath -Raw
        $content = $content -replace "namespace IOrder\.Application\.UseCases\.${folder};", "namespace $newNamespace;"
        Set-Content -Path $destPath -Value $content
    }
}

# Now, we need to fix the `using` statements in the Controllers
$apiControllersPath = "C:\Users\vanil\source\repos\IOrderNew\src\Backend\IOrder.Api\Controllers"
$apiFiles = Get-ChildItem -Path $apiControllersPath -Recurse -Filter "*.cs"

foreach ($file in $apiFiles) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false

    foreach ($folder in $folders) {
        if ($content -match "using IOrder\.Application\.UseCases\.${folder};") {
            $replacement = "using IOrder.Application.UseCases.${folder}.Commands;`r`nusing IOrder.Application.UseCases.${folder}.Queries;"
            $content = $content -replace "using IOrder\.Application\.UseCases\.${folder};", $replacement
            $modified = $true
        }
    }

    if ($modified) {
        Set-Content -Path $file.FullName -Value $content
    }
}

# Fix DependencyInjectionExtension.cs
$diPath = "C:\Users\vanil\source\repos\IOrderNew\src\Backend\IOrder.Application\DependencyInjectionExtension.cs"
if (Test-Path $diPath) {
    $content = Get-Content $diPath -Raw
    $modified = $false

    foreach ($folder in $folders) {
        if ($content -match "using IOrder\.Application\.UseCases\.${folder};") {
            $replacement = "using IOrder.Application.UseCases.${folder}.Commands;`r`nusing IOrder.Application.UseCases.${folder}.Queries;"
            $content = $content -replace "using IOrder\.Application\.UseCases\.${folder};", $replacement
            $modified = $true
        }
    }

    if ($modified) {
        Set-Content -Path $diPath -Value $content
    }
}

Write-Host "CQRS Refactoring Completed!"
