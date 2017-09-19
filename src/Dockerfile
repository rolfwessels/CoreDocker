FROM microsoft/dotnet:2-runtime

WORKDIR /app/CoreDocker.Api

COPY ./CoreDocker.Api/bin/publish .

EXPOSE 5000/tcp

ENTRYPOINT ["dotnet", "CoreDocker.Api.dll"]
