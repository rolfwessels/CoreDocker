FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine

# Base Development Packages
RUN apk update \
  && apk upgrade \
  && apk add ca-certificates wget && update-ca-certificates \
  && apk add --no-cache --update \
  git \
  curl \
  wget \
  bash \
  make \
  rsync \
  nano

WORKDIR /CoreDocker

COPY src/CoreDocker.Api/*.csproj ./src/CoreDocker.Api/
COPY src/CoreDocker.Dal/*.csproj ./src/CoreDocker.Dal/
COPY src/CoreDocker.Sdk/*.csproj ./src/CoreDocker.Sdk/
COPY src/CoreDocker.Core/*.csproj ./src/CoreDocker.Core/
COPY src/CoreDocker.Shared/*.csproj ./src/CoreDocker.Shared/
COPY src/CoreDocker.Dal/*.csproj ./src/CoreDocker.Dal/
COPY src/CoreDocker.Utilities/*.csproj ./src/CoreDocker.Utilities/
COPY src/CoreDocker.Dal.MongoDb/*.csproj ./src/CoreDocker.Dal.MongoDb/

WORKDIR /CoreDocker/src/CoreDocker.Api
RUN dotnet restore
RUN dotnet tool install nukeeper --global
# Working Folder
WORKDIR /CoreDocker
ENV TERM xterm-256color
RUN printf 'export PS1="\[\e[0;34;0;33m\][DCKR]\[\e[0m\] \\t \[\e[40;38;5;28m\][\w]\[\e[0m\] \$ "' >> ~/.bashrc
