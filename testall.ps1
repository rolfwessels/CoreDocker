dotnet restore
dotnet build
dotnet vstest (Get-ChildItem test | % { Join-Path $_.FullName -ChildPath ("bin/Debug/netcoreapp1.1/$($_.Name).dll") })
move testresults\*.trx testresults\result.trx