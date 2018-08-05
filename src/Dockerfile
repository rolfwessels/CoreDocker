FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers

COPY CoreDocker.Api/*.csproj ./CoreDocker.Api/
COPY CoreDocker.Dal/*.csproj ./CoreDocker.Dal/
COPY CoreDocker.Console/*.csproj ./CoreDocker.Console/
COPY CoreDocker.Sdk/*.csproj ./CoreDocker.Sdk/
COPY CoreDocker.Core/*.csproj ./CoreDocker.Core/
COPY CoreDocker.Shared/*.csproj ./CoreDocker.Shared/
COPY CoreDocker.Dal/*.csproj ./CoreDocker.Dal/
COPY CoreDocker.Utilities/*.csproj ./CoreDocker.Utilities/
COPY CoreDocker.Dal.MongoDb/*.csproj ./CoreDocker.Dal.MongoDb/

WORKDIR /app/CoreDocker.Api
RUN dir
RUN dotnet restore

# Copy everything else and build
WORKDIR /app
COPY CoreDocker.Api/ ./CoreDocker.Api/
COPY CoreDocker.Dal/ ./CoreDocker.Dal/
COPY CoreDocker.Console/ ./CoreDocker.Console/
COPY CoreDocker.Sdk/ ./CoreDocker.Sdk/
COPY CoreDocker.Core/ ./CoreDocker.Core/
COPY CoreDocker.Shared/ ./CoreDocker.Shared/
COPY CoreDocker.Dal/ ./CoreDocker.Dal/
COPY CoreDocker.Utilities/ ./CoreDocker.Utilities/
COPY CoreDocker.Dal.MongoDb/ ./CoreDocker.Dal.MongoDb/

WORKDIR /app/CoreDocker.Api
RUN dotnet publish -c Release -o out
RUN ls out


# Build runtime image
FROM microsoft/dotnet:2.0-runtime

WORKDIR /app
COPY --from=build-env /app/CoreDocker.Api/out .
COPY logSettings.xml .
RUN cat logSettings.xml
#  <file value="C:\temp\Logs\CoreDocker.Api.log" />
#     <lockingModel type="log4net.Appender.FileAppender+MinimalLock" />
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "CoreDocker.Api.dll"]
