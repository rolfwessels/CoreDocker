docker pull microsoft/dotnet
dotnet restore
dotnet publish -c Release -o bin/publish
docker build rolfwessels/coredocker:latest -t rolfwessels/coredocker:v0.0.1 .
