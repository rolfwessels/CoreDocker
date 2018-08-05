# CoreDocker

[![Build Status](https://travis-ci.org/rolfwessels/CoreDocker.svg?branch=master)](https://travis-ci.org/rolfwessels/CoreDocker)
[![Build status](https://ci.appveyor.com/api/projects/status/tumprt66bbfxb22o?svg=true)](https://ci.appveyor.com/project/rolfwessels/coredocker)
[![Dockerhub Status](https://img.shields.io/badge/dockerhub-ok-blue.svg)](https://hub.docker.com/r/rolfwessels/coredocker/)

This project contains some scafolding code that I use whenever I start a new project. It followes some best practices.

## Features
 * RESTful web API.
 * GraphQL (+ authorization + permissions)
 * Reactjs Dashboad UI integrated
 * Authorization (OpenId with integrated identity server).
 * Swagger for API documentation.
 * MongoDB document storage database.
 * CI Appvayor && Travis for build automation
 * Docker files to compile and compose a server
 * Developed using TDD practices.
 

## Todo
 * UI should build docker image.
 * Api Build docker image with no web.
 * Link the two for the site.
 * Zip data
 * Deploy lambda
 * Deploy with CDN
 * Prettier for the website
 * Security headers ? 
 * Code coverage in build process.
 * Code analytics - look at resharper cli tools.
 * Find and clean unused code. See if we can automate report
 * Decide what to do about logging, still not sold on the injection method (and log4net not using appsettings.json).
 * 3rd party authentication - github or google would be awesome (Tired of always writing own user management system).
 * Apply some CQRS patterns.
 * More https://shields.io/#/

## Gettings started with docker

 ```
 git clone https://github.com/rolfwessels/CoreDocker.git
 cd CoreDocker
 git submodule update --init --recursive
 ```

# Themes 

 * http://coreui.io/
 * https://github.com/akveo/ngx-admin
  