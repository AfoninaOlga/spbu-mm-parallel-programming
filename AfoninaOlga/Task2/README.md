# Producer-consumer problem

Implementation of the _producer-consumer problem_ using `Semaphore`.

## Dependencies

- [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Build and Run Locally

### Build project

```sh
dotnet build
```

### Run project

```sh
dotnet run --project ProducerConsumer <Number of producers> <Number of consumers>
```

#### Program output
```sh
dotnet run --project ProducerConsumer 2 3
```
Producers and consumers creation messages:
```
Producer #0 created
Producer #1 created
Consumer #0 created
Consumer #1 created
Consumer #2 created
```
Producers and consumers data processing messages:
```
Producer #0 added 0 to buffer
Consumer #2 took 0 from buffer
Producer #1 added 1 to buffer
Consumer #0 took 1 from buffer
Producer #0 added 0 to buffer
Consumer #1 took 0 from buffer
Producer #1 added 1 to buffer
Consumer #2 took 1 from buffer
Producer #0 added 0 to buffer
Producer #1 added 1 to buffer
Consumer #0 took 0 from buffer
Consumer #1 took 1 from buffer
Producer #0 added 0 to buffer
Consumer #2 took 0 from buffer
Producer #1 added 1 to buffer
Consumer #0 took 1 from buffer
Producer #0 added 0 to buffer
Consumer #1 took 0 from buffer
Producer #1 added 1 to buffer
Consumer #2 took 1 from buffer
Producer #0 added 0 to buffer
Producer #1 added 1 to buffer
```
Message after any key is pressed:
```
All producers and consumers finished
```
## Run tests

```sh
dotnet test
```