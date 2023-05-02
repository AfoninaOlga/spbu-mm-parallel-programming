## Как использовать

### Использование средствами Java

- Рекомендуемая Java 17: https://download.bell-sw.com/java/17.0.6+10/bellsoft-jdk17.0.6+10-windows-amd64-full.msi

### В классе Main приведен небольшой пример использования ThreadPool

```
ThreadPool threadPool = new ThreadPool(numOfThreads, workStrategy);

List<IMyTask<?>> tasks = new ArrayList<>();
for (int i = 0; i < 2; i++) {
    IMyTask<String> task = new MyTask<>(s -> "$", threadPool);
    task.continueWith(s -> s + "#");
    task.continueWith(s -> s + "&");
    tasks.add(task);

    IMyTask<Integer> task2 = new MyTask<>(s -> 2, threadPool);
    task2.continueWith(s -> s + 3);
    tasks.add(task2);
}

for (IMyTask<?> task: tasks) {
    threadPool.enqueue(task);
}
threadPool.run();

BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
while(reader.readLine() == null) {
    System.out.println("Step");
    Thread.sleep(1000);
}
reader.close();

threadPool.close();
```



#### Сборка

- Из папки `src` запустить:

```
javac -d bin -sourcepath ../src main/java/ershov/Main.java
```

#### Использование

- Из папки `src`
```
java -classpath ./bin main.java.ershov.Main numOfThreads workStrategy
```
где `numOfThreads` это количество потоков, а `workStrategy` это выбор стратегии работы потоков (“workStealing”/“workShearing”)

- В консоль будет выводиться информация о исполняемых задачах
- Чтобы завершить исполнение надо нажать `Enter`, после чего в консоль выведется информация о завершении работы потоков

## Использование с maven

#### Установка maven

- Убедиться, что maven есть и добавлен в Path: из корня проекта вызвать
```
mvn -v
```
- Если нет, то исправить это:
- Скачать архив https://dlcdn.apache.org/maven/maven-3/3.9.0/binaries/apache-maven-3.9.0-bin.zip
- Распаковать его
- Добавить папку `bin` из корня распакованного архива в `Path`

#### Необходимые зависимости из pom.xml

- `java` version 17

- `junit-jupiter` version 5.8.1

- `mockito-core` version 4.2.0

#### Сборка

- Из корневой папки
```
mvn clean package
```

- Отчет о покрытии тестов из корня: `target/site/jacoco/index.html`
