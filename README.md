# CoreDocker

[![Build Status](https://travis-ci.org/rolfwessels/CoreDocker.svg?branch=master)](https://travis-ci.org/rolfwessels/CoreDocker)
[![Build status](https://ci.appveyor.com/api/projects/status/tumprt66bbfxb22o?svg=true)](https://ci.appveyor.com/project/rolfwessels/coredocker)

This project contains some scafolding code that I use whenever I start a new project. It followes some best practices.

## Features
 * RESTful web API.
 * GraphQL
 * Reactjs Dashboad UI integrated
 * Authorization (OpenId with integrated identity server).
 * Swagger for API documentation.
 * MongoDB document storage database.
 * CI Appvayor && Travis for build automation
 * Docker files to compile and compose a server
 * Developed using TDD practices.
 

## Todo
 * Code coverage in build process.
 * Code analytics - look at resharper cli tools.
 * Better API with integrated website.
 * Decide what to do about logging, still not sold on the injection method (and log4net not using appsettings.json).
 * Signal-r replacement.
 * 3rd party authentication - github or google would be awesome (Tired of always writing own user management system).
 * GraphQL - Fix UTC date
 * GraphQL - Better error logging
 * GraphQL - Fix the properties
 * GraphQL - Look at recursive queries

## Gettings started with docker

 ```
 git clone https://github.com/rolfwessels/CoreDocker.git
 cd CoreDocker
 git submodule update --init --recursive
 ```

# Themes 

 * http://coreui.io/
 * https://github.com/akveo/ngx-admin
  