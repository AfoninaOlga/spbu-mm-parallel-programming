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

### Usage example


```kotlin
val pool = ThreadPool(nThreads, WorkStrategy.SHARING)

val task1 = ThreadPoolTask("task1", pool) { 2 * 2 }
val task2 = ThreadPoolTask("task2", pool) { 3 * 3 }

val task1Continued = task1.continueWith { prev -> prev.toString() }

pool.enqueue(task1)
pool.enqueue(task2)
//no need to enqueue continued task

pool.use {
    task1Continued.result()
}
```
