## Setup
First you will need to setup the postgresql database. <br/>
To setup this, you can eater [download and setup postgresql](https://www.postgresql.org/download/) or
run this command to start a docker container already setup for this project.
````shell
docker compose -f docker-compose-local-database-only.yml up
````

## Running Tests
#### Unit tests
To run the unit tests you only need to run this command in the test folder:
````shell
dotnet test SimpleCustomerApi.Tests.Unit/
````
![passing unit tests](https://i.imgur.com/P7BVedb.png)

#### Integration tests
To run the integration tests, you will need to have images in your docker.
````shell
docker pull testcontainers/ryuk:0.3.4
docker pull postgres:11.14
````

With these 2 images, the integration tests will be able to spin up docker container with postgresql to do its tests.<br>
To run the integration tests you will need to run this command in the test folder:
````shell
dotnet test SimpleCustomerApi.Tests.Integration/
````

![passing integration tests](https://i.imgur.com/nZpMRyI.png)





## Postman collection
The postman collection uses a variable called 'url'. 
This variable needs to be set to the proper base url. 
It will change depending on which tool you use to run the application. <br/>
ex:
- dev => https://localhost:7287
- docker container => http://localhost:5000 

## Run release locally
````shell
docker compose up
````





## Docker hub image
The docker image is available [here](https://hub.docker.com/r/samishoux/simplecustomerapi-api).


