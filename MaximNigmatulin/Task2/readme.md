# Producer-Consumer pattern implementation using mutexes as synchronization mechanism

### Launch

on linux:
```sh
./gradlew run --args="<n_producers> <n_consumers>"
```

for example:
```sh
./gradlew run --args="10 10"
```

press ```enter``` to stop execution

### Code coverage

```sh
./gradlew koverReport && open ./build/reports/kover/html/index.html
```

### Tests summary

```sh
./gradlew koverReport && open ./build/reports/tests/test/index.html
```
