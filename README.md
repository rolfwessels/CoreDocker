# CoreDocker

![CoreDocker Logo](https://github.com/rolfwessels/CoreDocker/raw/master/logo/coredocker_logo.png)

[![Build Status](https://travis-ci.org/rolfwessels/CoreDocker.svg?branch=master)](https://travis-ci.org/rolfwessels/CoreDocker)
[![Build status](https://ci.appveyor.com/api/projects/status/tumprt66bbfxb22o?svg=true)](https://ci.appveyor.com/project/rolfwessels/coredocker)
[![Dockerhub Status](https://img.shields.io/badge/dockerhub-ok-blue.svg)](https://hub.docker.com/r/rolfwessels/coredocker/)

This project contains some scafolding code that I use whenever I start a new project. It followes some best practices.

## Features

- RESTful web API.
- GraphQL (+ authorization + permissions) using hot chocolate
- Reactjs Dashboad UI integrated
- Authorization (OpenId with integrated identity server).
- Swagger for API documentation.
- MongoDB document storage database.
- CI Appvayor && Travis for build automation
- Docker files to compile and compose a server
- Developed using TDD practices.

## Todo

- Version the binaries that get built in docker.
- Deploy with CDN
- Code coverage in build process.
- Code analytics - look at resharper cli tools.
- Find and clean unused code. See if we can automate report
- 3rd party authentication - github or google would be awesome (Tired of always writing own user management system).
- More https://shields.io/#/

## Inspection

```cmd
 dotnet add .\test\CoreDocker.Utilities.Tests\ package JetBrains.ReSharper.CommandLineTools --package-directory .\build\tools
 build\tools\jetbrains.resharper.commandlinetools\2018.2.3\tools\InspectCode.exe --caches-home="C:\Temp\Cache" -f=html -o="report.html" .\CoreDocker.sln
 build\tools\jetbrains.resharper.commandlinetools\2018.2.3\tools\dupfinder.exe --caches-home="C:\Temp\Cache"  -o="duplicates.xml" .\CoreDocker.sln
 build\tools\jetbrains.resharper.commandlinetools\2018.2.3\tools\cleanupcode.exe .\CoreDocker.sln
```

## Getting started

Open the docker environment to do all development and deployment

```bash
# bring up dev environment
make build up
# to test the app
make test
# to run the app
make start
```

## Available make commands

### Commands outside the container

- `make up` : brings up the container & attach to the default container
- `make down` : stops the container
- `make build` : builds the container

### Commands to run inside the container

- `make start` : Run the app
- `make test` : To test the app

## Create certificates

see https://benjii.me/2017/06/creating-self-signed-certificate-identity-server-azure/

```cmd
cd src/CoreDocker.Api/Certificates
openssl req -x509 -newkey rsa:4096 -sha256 -nodes -keyout development.key -out development.crt -subj "/CN=localhost" -days 3650
openssl pkcs12 -export -out development.pfx -inkey development.key -in development.crt -certfile development.crt
rm development.crt
rm development.key

openssl req -x509 -newkey rsa:4096 -sha256 -nodes -keyout production.key -out production.crt -subj "/CN=localhost" -days 3650
openssl pkcs12 -export -out production.pfx -inkey production.key -in production.crt -certfile production.crt
rm production.crt
rm production.key

```

## Deploy docker files

```cmd
cd src
docker-compose build;
docker-compose up;
```

Debugging

```cmd
cd src
docker-compose up -d;
docker-compose exec api bash
```

## Deploy lambda

```cmd
cd src/CoreDocker.Api.Lambda
dotnet build -v=q
dotnet lambda deploy-serverless --s3-bucket coredocker-serverless coredocker-sample
dotnet lambda delete-serverless Stage
## Note that there is some circular depenency so you will need to update the origin url manually for now :-(

```

## Logo

- Special thanks to @baranpirincal for the logo. https://github.com/baranpirincal

## Themes

- http://coreui.io/
- https://github.com/akveo/ngx-admin
