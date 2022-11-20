## Setup
First you will need to setup the postgresql database. <br/>
To setup this, you can eater [download and setup postgresql](https://www.postgresql.org/download/) or
run this command to start a docker container already setup for this project.
```bash
docker compose -f docker-compose-local-database-only.yml up
```





## Postman collection
The postman collection uses a variable called 'url'. 
This variable needs to be set to the proper base url. 
It will change depending on which tool you use to run the application. <br/>
ex:
- dev => https://localhost:7287
- docker container => http://localhost:5000 