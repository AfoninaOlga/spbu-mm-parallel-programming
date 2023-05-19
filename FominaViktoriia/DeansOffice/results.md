## Нагрузочное тестирование

### Конфигурация докер образа
- Microsoft Windows [Version 10.0.17763.4252]
- .NET Core 7 runtime
- ASP.NET Core 7 runtime

Докер контейнер был запущен из Visual Studio 2022 следующей коммандой:
```
docker run --hostname=af853c1e0493 --user=ContainerUser --env=NUGET_FALLBACK_PACKAGES=c:\.nuget\fallbackpackages --env=ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS=true --env=ASPNETCORE_ENVIRONMENT=Development --env=ASPNETCORE_URLS=https://+:443;http://+:80 --env=DOTNET_USE_POLLING_FILE_WATCHER=1 --env=NUGET_PACKAGES=c:\.nuget\fallbackpackages --env=DOTNET_RUNNING_IN_CONTAINER=true --env=DOTNET_VERSION=7.0.5 --env=ASPNET_VERSION=7.0.5 --volume=C:\Users\vika\onecoremsvsmon\17.5.20124.2323:C:\remote_debugger:ro --volume=C:\Users\vika\AppData\Roaming\Microsoft\UserSecrets:C:\Users\ContainerUser\AppData\Roaming\Microsoft\UserSecrets:ro --volume=C:\Users\vika\AppData\Roaming\ASP.NET\Https:C:\Users\ContainerUser\AppData\Roaming\ASP.NET\Https:ro --volume=C:\Users\vika\github\spbu-mm-parallel-programming\FominaViktoriia\DeansOffice\DeansOffice:C:\app --volume=C:\Users\vika\github\spbu-mm-parallel-programming\FominaViktoriia\DeansOffice:c:\src --volume=C:\Users\vika\.nuget\packages\:c:\.nuget\fallbackpackages --workdir=C:\app -p 62070:443 -p 62071:80 --restart=no --label='com.microsoft.created-by=visual-studio' --label='com.microsoft.visual-studio.project-name=DeansOffice' -t -d deansoffice:dev
```

### Диаграммы распределения времени выполнения запросов

- 6000 пользователей, 10 запросов от пользователя, 96000 запросов в минуту, 1393 записи по таймауту в 10 секунд, распеделение запросов (в %): 9 - Add, 90 - Contains, 1 - Remove
![image](https://github.com/Stanislav-Sartasov/spbu-mm-parallel-programming/assets/32179813/5c6d2178-31d1-49ab-b05a-7771ab1ca980)

- 1500 пользователей, 10 запросов от пользователя, 12000 запросов в минуту
![image](https://github.com/Stanislav-Sartasov/spbu-mm-parallel-programming/assets/32179813/536ad8d6-1fc4-4a02-a853-771101a41084)

- 1500 пользователей, 10 запросов от пользователя, 12000 запросов в минуту
![image](https://github.com/Stanislav-Sartasov/spbu-mm-parallel-programming/assets/32179813/1e1d9df0-f0a6-44aa-8842-846254b48fa5)

- 1500 пользователей, 10 запросов от пользователя, 12000 запросов в минуту, распеделение запросов (в %): 9 - Add, 90 - Contains, 1 - Remove
![image](https://github.com/Stanislav-Sartasov/spbu-mm-parallel-programming/assets/32179813/23eabe9b-711f-4457-8255-e2a046c182bc)


