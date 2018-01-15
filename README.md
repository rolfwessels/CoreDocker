# CoreDocker

[![Build Status](https://travis-ci.org/rolfwessels/CoreDocker.svg?branch=master)](https://travis-ci.org/rolfwessels/CoreDocker)
[![Build status](https://ci.appveyor.com/api/projects/status/tumprt66bbfxb22o?svg=true)](https://ci.appveyor.com/project/rolfwessels/coredocker)

This project contains some scafolding code that I use whenever I start a new project. It followes some best practices.

## Features
 * RESTful web API.
 * Authorization (OpenId with integrated identity server).
 * Swagger for API documentation.
 * MongoDB document storage database.
 * CI Appvayor && Travis for build automation
 * Developed using TDD practices.

## Todo
 * Psake for windows builds.
 * Use Git to determine version number.
 * Code coverage.
 * Code analytics - look at resharper cli tools.
 * Better API with integrated website.
 * Decide what to do about logging, still not sold on the injection method (and log4net not using appsettings.json).
 * Signal-r replacement.
 * 3rd party authentication - github or google would be awesome (Tired of always writing own user management system).
