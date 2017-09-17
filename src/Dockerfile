FROM microsoft/dotnet:1.1.2-runtime-jessie

WORKDIR /app/CoreDocker.Api

COPY ./CoreDocker.Api/bin/publish .

EXPOSE 5000/tcp

ENTRYPOINT ["dotnet", "CoreDocker.Api.dll"]
