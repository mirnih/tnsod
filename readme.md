# Distributed Rate Limiter

Система управления пользователями и ограничения частоты запросов (Rate Limiting), разработанная на C# / .NET 9 с использованием микросервисной архитектуры и gRPC.

## Стек технологий

- **C# / .NET 9**, gRPC, Protocol Buffers
- **PostgreSQL**, Dapper — хранение данных пользователей
- **MongoDB** — хранение ограничений RPM
- **Apache Kafka** — передача событий запросов
- **Redis** — хранение временных блокировок
- **IMemoryCache** — кэширование пользователей
- **FluentValidation**, Mapperly — валидация и маппинг

## Функциональность

Система состоит из трёх gRPC-сервисов и генератора событий:

- **UserService** — CRUD пользователей с хранением в PostgreSQL, кэшированием данных и проверкой ограничений через gRPC Interceptors.
- **RateLimiter.Writer** — создание, получение, изменение и удаление RPM-лимитов для отдельных методов API в MongoDB.
- **RateLimiter.Reader** — загрузка лимитов в память, отслеживание изменений через MongoDB Change Streams и обработка событий Kafka. При превышении лимита пользователь блокируется для конкретного метода на 5 минут через Redis.
- **UserRequestsKafkaGenerator** — генерация запросов от нескольких пользователей с возможностью изменять RPM и методы API во время работы.

Ограничения проверяются отдельно для каждого пользователя и метода в скользящем окне продолжительностью 60 секунд. При отсутствии заголовка `user_id` или наличии активной блокировки UserService отклоняет запрос.

## Запуск

Для работы необходимы .NET 9 SDK, PostgreSQL, MongoDB (replica set), Apache Kafka и Redis.

Настройки подключений находятся в `appsettings.json`. Перед запуском необходимо поднять инфраструктуру и выполнить SQL-скрипты из `UserService/Database`.

Сборка проекта:

```bash
dotnet restore
dotnet build RateLimiter.sln
```

Запуск компонентов в отдельных терминалах:

```bash
dotnet run --project UserService
dotnet run --project RateLimiter.Writer
dotnet run --project RateLimiter.Reader
dotnet run --project UserRequestsKafkaGenerator
```

Пример команд генератора событий:

```bash
add 1 GetUserById 100
add 2 GetUserById 10
list
update 1 rpm 150
remove 1
```

Команды сборки и запуска предполагают корректно настроенные зависимости и инфраструктуру.
