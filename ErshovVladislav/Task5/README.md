## Использование с maven

- Рекомендуемая Java 17: https://download.bell-sw.com/java/17.0.6+10/bellsoft-jdk17.0.6+10-windows-amd64-full.msi

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

#### Запуск

- Из корневой папки

```
mvn exec:java -Dexec.mainClass="ershov.Main"
```

#### Работа с чатом

![image-20230525151304519](C:\Users\ershov.e\AppData\Roaming\Typora\typora-user-images\image-20230525151304519.png)

- Чтобы подключиться к другому пользователю, нужно ввести его ip и нажать кнопку `Connect`
- Чтобы отправить сообщение в чат его надо ввести и нажать кнопку `Send`
- Чтобы обновить список сообщений надо нажать кнопку `Refresh chat`
- Чтобы выйти из чата нужно закрыть приложение