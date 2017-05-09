dotnet restore
cd .\test\CoreDocker.Core.Tests 
dotnet test 
cd ..\..\
cd .\test\CoreDocker.Utilities.Tests
dotnet test 
cd ..\..\
cd .\test\CoreDocker.Sdk.Tests
dotnet test 
cd ..\..\
cd .\test\CoreDocker.Api.Tests
dotnet test 
cd ..\..\
cd .\test\CoreDocker.Dal.Tests
dotnet test 
cd ..\..\
pause