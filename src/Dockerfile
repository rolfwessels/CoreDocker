FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env
WORKDIR /app
COPY ./ ./
WORKDIR /app/CoreDocker.Api
RUN dotnet publish -c Release -o out


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

WORKDIR /app
COPY --from=build-env /app/CoreDocker.Api/out .
RUN apk add curl
HEALTHCHECK CMD curl --fail http://localhost:5000/api/ping/hc || exit 1
#  <file value="C:\temp\Logs\CoreDocker.Api.log" />
#     <lockingModel type="log4net.Appender.FileAppender+MinimalLock" />
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "CoreDocker.Api.dll"]
