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
- Из папки `installmpj` запустить
```
install.bat
```
- Из корневой папки
```
mvn clean package
```
