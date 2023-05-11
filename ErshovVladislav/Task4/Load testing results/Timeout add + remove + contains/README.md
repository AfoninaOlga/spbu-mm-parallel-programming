### Конфигурация нагрузочного тестирования
- Для OptimisticSet
- 8000 пользователей
- Каждый делает по 10 запросов
- Количество запросов в секунду - примерно 1200
- Соотношение запросов Add - 9%, Remove - 1%, Contains - 90%
- Тестирование длится примерно 60 секунд
- После тестирования остается примерно 7200 записей
- Отказы обслуживания по тайм-ауту (больше 10 секунд) начиная примерно с 2757 записей в множестве

![Query execution time distribution diagram.png](https://github.com/Stanislav-Sartasov/spbu-mm-parallel-programming/blob/ErshovVladislav/ErshovVladislav/Task4/Load%20testing%20results/Timeout%20add%20%2B%20remove%20%2B%20contains/Query%20execution%20time%20distribution%20diagram.png)
