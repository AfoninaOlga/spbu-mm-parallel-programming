## Как пользоваться

### Необходимые инструменты

- `Docker` - скачать с официального сайта https://www.docker.com/products/docker-desktop/
- Опционально `Postman` (для проверки REST запросов) - https://www.postman.com/downloads/

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

#### Примеры запросов

- Добавление

```
http://127.0.0.1:8092/add?student=111&exam=123
```

- Удаление

```
http://127.0.0.1:8092/remove?student=111&exam=123
```

- Проверка

```
http://127.0.0.1:8092/contains?student=111&exam=123
```

- Количество

```
http://127.0.0.1:8092/count
```



