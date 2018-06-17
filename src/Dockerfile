FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/app/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY src/app/*.cs ./
RUN dotnet publish -c Release -o out
RUN ls out
# Build runtime image
FROM microsoft/dotnet:2.1-runtime
RUN apt-get update 
RUN apt-get install -y cron

WORKDIR /app
COPY --from=build-env /app/out .
COPY src/app/*.sh ./
RUN ls
ENTRYPOINT ["sh","buildruncron.sh"]

FROM microsoft/dotnet:2-runtime

WORKDIR /app/CoreDocker.Api

COPY ./CoreDocker.Api/bin/publish .

EXPOSE 5000/tcp

ENTRYPOINT ["dotnet", "CoreDocker.Api.dll"]
