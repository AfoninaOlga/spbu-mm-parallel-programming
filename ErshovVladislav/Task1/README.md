## Как использовать

#### Установка MPJ Express

- Скачать MPJ Express https://sourceforge.net/projects/mpjexpress/files/releases/mpj-v0_44.zip/download
- Распаковать его
- Добавить корень распакованного архива в `MPJ_HOME`
- Добавить папку `bin` из корня распакованного архива в `Path`

#### Использование

- Запустить из папки `src`
```
javac -cp .;%MPJ_HOME%/lib/mpj.jar main/java/quicksort/Main.java
```
где `MPJ_HOME` это путь до MPJ Express

- Затем запустить
```
mpjrun.bat -np n main.java.quicksort.Main path/to/input/file path/to/output/file
```
где:

- `n` это количество вычислительных узлов

- `path/to/input/file` это путь от `src` до файла с изначальным массивом
- `path/to/output/file` это путь от `src` до файла, куда запишется отсортированный массив



В корне проекта в папке `examples` есть примеры начальных массивов

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

- `mpj` version 0.44
- `junit4` version 4.13.2

#### Использование

- Из корня запустить установку mpj для maven
```
installmpj.bat
```
- Из корневой папки
```
mvn clean package
```
