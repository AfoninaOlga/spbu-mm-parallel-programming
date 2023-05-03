# ExamSystem problem

Implementation of the ExamSystem web API.

## Dependencies

- [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Docker](https://www.docker.com/)

## Build and Run with dotnet

### Build project

```sh
dotnet build
```

### Run project

```sh
docker run --project ExamSystem <System type>
```

`<System type>` may be one of the following:

- fine-grained
- lazy

## Build and Run with Docker

### Build project

```sh
docker build -t examsystem:latest .
```

### Run project

```sh
docker run -p 80:80 examsystem <System type>
```
`<System type>` may be one of the following:

- fine-grained
- lazy

## Web API usage

Add to system information that student with id=1 passed course with id=1 (ExamSystem **Add**):
```http request
POST http://localhost/api?studentId=1&courseId=1
```

Remove from system information that student with id=1 passed course with id=1 (ExamSystem **Remove**):
```http request
DELETE http://localhost/api?studentId=1&courseId=1
```
Check if student with id=1 passed course with id=1 (ExamSystem **Contains**):
```http request
GET http://localhost/api?studentId=1&courseId=1
```
Get number of records in system (ExamSystem **Count**):
```http request
GET http://localhost/api/count
```