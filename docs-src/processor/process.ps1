
$inputPath = $(Resolve-Path "$PSScriptRoot\..").Path

$outputPath = "$PSScriptRoot\..\..\docs"
New-Item -ItemType directory -Path $outputPath -Force | Out-Null
$outputPath = $(Resolve-Path $outputPath).Path

$re = [regex]'`\[[^\]]+\]\((?<path>[^)#]+)(#(?<snippet>[^)]+))?\)'

Get-ChildItem -Path $inputPath -Recurse -Filter "*.md" |
ForEach-Object {
    $input = $_.FullName
    $namePart = $input.Substring($inputPath.Length)
    Write-Output "$($namePart) processing..."
    $output = "$($outputPath)$($namePart)"
    New-Item -ItemType directory -Path $([System.IO.Path]::GetDirectoryName($output)) -Force | Out-Null

    $relativePath = $([System.IO.Path]::GetDirectoryName($input))
    $getSnip = {
        param([String] $path, [String] $snip)
        if ($snip -eq '') {
            Get-Content $([System.IO.Path]::Combine($relativePath, $path)) -Encoding UTF8 -Raw
        } else {
            $tempContent = Get-Content $([System.IO.Path]::Combine($relativePath, $path)) -Encoding UTF8 -Raw
            $start = $tempContent.IndexOf("###$($snip)") + 3 + $snip.Length
            $end = $tempContent.IndexOf("$($snip)###")
            $lines = $tempContent.Substring($start, $end - $start) -Replace "`r","" -Replace "`t","    " -Split "`n"
            $lines = $lines[1..($lines.Length - 2)]
            $prefixSpacing = $($lines | Where-Object Length -ne 0 | ForEach-Object { $($_.Length - $_.TrimStart().Length) } | Measure-Object -Minimum).Minimum
            $lines = $lines | ForEach-Object { if ($_.Length -eq 0) { $_ } else { $_.Substring($prefixSpacing) } }
            $([String]::Join("`n", $lines))
        }
    }

    $content = Get-Content $input -Encoding UTF8 -Raw
    $content = $re.Replace($content, { param([System.Text.RegularExpressions.Match] $match) & $getSnip $match.Groups['path'].Value $match.Groups['snippet'].Value })
    $content | Set-Content $output -Encoding UTF8


    Write-Output "$($namePart) written."
}

