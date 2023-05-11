## Как пользоваться

### Необходимые инструменты

- `Docker` - скачать с официального сайта https://www.docker.com/products/docker-desktop/
- Опционально `Postman` (для проверки REST запросов) - https://www.postman.com/downloads/
- Опционально `JMetter` (для нагрузочного тестирования) - https://jmeter.apache.org/download_jmeter.cgi
- В папке `Load testing results` лежит конфигурация для `JMetter`

### Запуск docker-image

- Запустить Docker (на windows открыть Docker Dekstop)

- Из корня Task4 (в корне лежит еще одна папка Task4 и dockerfile) запустить

```
docker build -t examsystem .
```

- Затем

```
docker run -p 8092:8092 examsystem
```

- Через поисковую строку браузера или `Postman` можно проверить работоспособность сервиса
- С помощью `JMeter` можно провести нагрузочное тестирование

#### Примеры запросов

- Существует две реализации множества - `OptimisticSet` и `LazySet`
- Чтобы выбрать вариант множества нужно использовать префикс `/v1` и `/v2` соответственно

- Добавление

```
http://127.0.0.1:8092/v1/add?student=111&course=123
```

```
http://127.0.0.1:8092/v2/add?student=111&course=123
```

- Удаление

```
http://127.0.0.1:8092/v1/remove?student=111&course=123
```

```
http://127.0.0.1:8092/v2/remove?student=111&course=123
```

- Проверка

```
http://127.0.0.1:8092/v1/contains?student=111&course=123
```

```
http://127.0.0.1:8092/v2/contains?student=111&course=123
```

- Количество

```
http://127.0.0.1:8092/v1/count
```

```
http://127.0.0.1:8092/v2/count
```
