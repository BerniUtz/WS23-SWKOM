# WS23-SWKOM

## Project Setup

### Services

For this project, you need to have the following services running:
- PostgreSQL
- RabbitMQ
- MinIO
- Elasticsearch

For your convenience, you can use the provided docker-compose file to start the services:
```
docker-compose up -d
```

### Integration Tests

#### Pre-requisites
To be able to run the integration tests, you need to install the newman package 
globally (feel free to replace npm with your preferred package manager): 
```
npm install -g newman
```

#### Running the tests
To run the integration tests, you need to start the server first, as well as the
required services (e.g. the database). Then, you can run the tests with the following 
command:
```
newman run paperless.postman_collection.json --environment DEVELOPMENT.postman_environment.json
```