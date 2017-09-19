dotnet restore
dotnet build
dotnet vstest (Get-ChildItem test | % { Join-Path $_.FullName -ChildPath ("bin/Debug/netcoreapp2.0/$($_.Name).dll") })
move testresults\*.trx testresults\result.trx