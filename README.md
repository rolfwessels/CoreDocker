# Core docker

This makes core docker happen

## Getting started

Open the docker environment to do all development and deployment

```bash
# bring up dev environment
make build up
# build the project ready for publish
make publish
```

## Available make commands

### Commands outside the container

- `make up` : brings up the container & attach to the default container
- `make down` : stops the container
- `make build` : builds the container

### Commands to run inside the container

- `make config` : Used to create aws config files
- `make init` : Initialize terraform locally
- `make plan` : Run terraform plan
- `make apply` : Run terraform Apply

## Research

- <https://opensource.com/article/18/8/what-how-makefile> What is a Makefile and how does it work?
