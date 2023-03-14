## Как использовать

- Запустить из папки `src/main/java/QuickSortUsingSerialSetOfSamples`
```
javac -cp .;%MPJ_HOME%/lib/mpj.jar QuickSortUsingSerialSetOfSamples/Main.java
```
где `MPJ_HOME` это путь до MPJ Express

- Затем запустить
```
mpjrun.bat -np n QuickSortUsingSerialSetOfSamples.Main
```
где `n` это количество вычислительных узлов

## Использование с maven

#### Установка maven

- Убедиться, что maven есть и добавлен в Path: из корня проекта вызвать
```
mvn -v
```
- Если нет, то исправить это:
- Скачать архив https://dlcdn.apache.org/maven/maven-3/3.9.0/binaries/apache-maven-3.9.0-bin.zip
- Распаковать его
- Добавить папку bin из корня распакованного архива в Path

#### Использование

- Из корня запустить установку mpj для maven
```
installmpj.bat
```
- Из корневой папки
```
mvn clean package
```
