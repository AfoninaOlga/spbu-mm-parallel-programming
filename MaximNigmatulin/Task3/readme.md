# Thread pool with collaborative shutdown

### Launch

on linux:
```sh
./gradlew run --args="<n_threads>"
```

for example:
```sh
./gradlew run --args="5"
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
