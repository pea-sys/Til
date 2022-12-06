$targetPath = $Args[0]

$files = Get-ChildItem -Path $targetPath | ? { $_.Extension -like "*.doc" }

$wdFormatDocumentDefault = 16
 
$word = New-Object -ComObject Word.Application
     
foreach ($f in $files)
{
    Write-Host $f

    $doc = $word.Documents.Open($f.FullName)

    $outputfile = $f.FullName.Replace(".doc", ".docx")
    Write-Host $outputfile

    $doc.SaveAs2([ref]$outputfile, [ref]$wdFormatDocumentDefault)

    $doc.Close()
}

$word.Quit()
