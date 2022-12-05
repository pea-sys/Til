$targetPath = $Args[0]

$files = Get-ChildItem -Path $targetPath | ? { $_.Extension -like "*.doc" }

$wdFormatDocumentDefault = 16
 
foreach ($f in $files)
{
    Write-Host $f

    $word = New-Object -ComObject Word.Application

    $doc = $word.Documents.Open($f.FullName)

    $outputfile = $f.FullName.Replace(".doc", ".docx")
    Write-Host $outputfile

    $doc.SaveAs2([ref]$outputfile, [ref]$wdFormatDocumentDefault)

    $doc.Close()

    $word.Quit()
}