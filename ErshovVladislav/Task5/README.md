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

#### Необходимые приготовления

- Убедиться, что нет проблем с ip адресацией

- Убедиться, что порт 8093 не занят другими приложениями (Ищем `0.0.0.0:8093` в `Локальный адрес`)

```
netstat -ano
```



- Псоле запуска приложения убедиться, что порт 8093 открыт (если `telnet` не работает - включить по инструкции https://help.keenetic.com/hc/ru/articles/213965809-%D0%92%D0%BA%D0%BB%D1%8E%D1%87%D0%B5%D0%BD%D0%B8%D0%B5-%D1%81%D0%BB%D1%83%D0%B6%D0%B1-Telnet-%D0%B8-TFTP-%D0%B2-Windows)

```
telnet localhost 8093
```

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

![image-20230525151304519](https://github.com/Stanislav-Sartasov/spbu-mm-parallel-programming/blob/ErshovVladislav/ErshovVladislav/Task5/application.PNG)

- Чтобы подключиться к другому пользователю, нужно ввести его ip и нажать кнопку `Connect`
- Чтобы отправить сообщение в чат его надо ввести и нажать кнопку `Send`
- Чтобы обновить список сообщений надо нажать кнопку `Refresh chat`
- Чтобы выйти из чата нужно закрыть приложение
- В чате будут отображаться сообщения от пользователей и информация об их подключениях
