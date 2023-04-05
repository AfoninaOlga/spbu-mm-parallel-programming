## Как использовать

### Использование средствами Java

- Рекомендуемая Java https://download.bell-sw.com/java/17.0.6+10/bellsoft-jdk17.0.6+10-windows-amd64-full.msi

#### Сборка
- Из папки `src` запустить:

```
javac -d bin -sourcepath ../src main/java/ershov/Main.java
```

#### Использование

- Из папки `src`
```
java -classpath ./bin main.java.ershov.Main numOfProducers numOfConsumers
```
где `numOfProducers` это количество производителей, а `numOfConsumers` это количество потребителей

- В консоль будет выводиться информация о произведенных и извлеченных продуктах
- Чтобы завершить исполнение надо нажать `Enter`, после чего в консоль выведется информация о завершении работы производителей и потребителей

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

- `junit4` version 4.13.2

#### Сборка

- Из корневой папки
```
mvn clean package
```
