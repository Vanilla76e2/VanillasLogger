# VanillasLogger

**VanillasLogger** — лёгкая и автономная библиотека логирования для .NET, основанная на Serilog. Поддерживает сессии, краш-логи и автоматическую обработку необработанных исключений.

---

## Особенности

- Минимальный интерфейс — только `ILogger`.

- Уровни логов: Debug, Information, Warning, Error, Critical.

- Логи сессии автоматически создаются при старте приложения.

- Краш-логи содержат полную историю сессии и информацию об ошибке.

- Автоматическое определение контекста `[Класс].[Метод]`.

- Подписка на необработанные исключения (`AppDomain` и `TaskScheduler`).

- Прост в использовании — не требует DI.

---

## Установка

Склонируйте репозиторий и добавьте проект в решение:

```bash
git clone https://github.com/Vanilla76e2/VanillasLogger.git
```

---

## Быстрый старт

Минимальный пример:

```csharp
using VanillasLogger;

ILogger logger = new VanillasLogger.Logger();

// Логирование
logger.LogInformation("Приложение запущено");
logger.LogWarning("Что-то пошло не так...");
logger.LogError("Ошибка во время выполнения");
```

Логи автоматически сохраняются в папку `Logs/Session/`.

---

## Настройки

Вы можете изменить директории хранения через `LoggerOptions`:

```cs
var options = new VanillasLogger.LoggerOptions
{
    SessionLogsDirectory = "MyLogs/Session",
    CrashLogsDirectory = "MyLogs/Crash"
};

ILogger logger = new VanillasLogger.Logger(options);
```

| Свойство               | Описание                                                                                              |
| ---------------------- | ----------------------------------------------------------------------------------------------------- |
| `SessionLogsDirectory` | Директория хранения сессионных логов. (По умолчанию `Logs/Session`)                                 |
| `CrashLogsDirectory`   | Директория хранения краш-логов. (По умолчанию `Logs/Crash`)                                         |

---

## Краш-логи

VanillasLogger автоматически подписывается на необработанные исключения:

```cs
throw new InvalidOperationException("Тестовый краш");
```

При падении приложения будет создан файл в `Logs/Crash/` с полной сессией и стек-трейсом.

## Структура логов

* **Сессия:** `Logs/Session/session.log`
* **Краш-лог:** `Logs/Crash/crash_yyyyMMdd_HHmmss.log`
* Краш-лог содержит **всю сессию** + информацию об исключении.

---

## Пример вывода

После запуска приложения и записи сообщений в лог, файл сессии будет содержать примерно следующее:
```log
2025-09-24 11:08:53.981 +05:00 [DBG] Program.Main: Debug сообщение
2025-09-24 11:08:54.004 +05:00 [INF] Program.Main: Info сообщение
2025-09-24 11:08:54.005 +05:00 [WRN] Program.Main: Warn сообщение
2025-09-24 11:08:54.007 +05:00 [ERR] Program.Main: Error сообщение
2025-09-24 11:08:54.009 +05:00 [FTL] Program.Main: Fatal сообщение
2025-09-24 14:56:28.700 +05:00 [ERR] Program.Main: Ошибка с исключением
System.InvalidOperationException: Тестовое исключение
   at Program.Main() in C:\Users\Vanilla76e2\source\repos\VanillasLogger\ConsoleApp1\Program.cs:line 27
2025-09-24 14:56:28.701 +05:00 [FTL] Program.Main: Критическая ошибка с исключением
System.InvalidOperationException: Тестовое исключение
   at Program.Main() in C:\Users\Vanilla76e2\source\repos\VanillasLogger\ConsoleApp1\Program.cs:line 27
```


Если произойдёт необработанное исключение, будет создан отдельный краш-лог в `Logs/Crash/`, который содержит всю сессию + информацию об ошибке:

```log
=== Журнал сессии ===
2025-09-24 11:08:53.981 +05:00 [DBG] Program.Main: Debug сообщение
2025-09-24 11:08:54.004 +05:00 [INF] Program.Main: Info сообщение
2025-09-24 11:08:54.005 +05:00 [WRN] Program.Main: Warn сообщение
2025-09-24 11:08:54.007 +05:00 [ERR] Program.Main: Error сообщение
2025-09-24 11:08:54.009 +05:00 [FTL] Program.Main: Fatal сообщение
2025-09-24 14:56:28.700 +05:00 [ERR] Program.Main: Ошибка с исключением
System.InvalidOperationException: Тестовое исключение
   at Program.Main() in C:\Users\Vanilla76e2\source\repos\VanillasLogger\ConsoleApp1\Program.cs:line 27
2025-09-24 14:56:28.701 +05:00 [FTL] Program.Main: Критическая ошибка с исключением
System.InvalidOperationException: Тестовое исключение
   at Program.Main() in C:\Users\Vanilla76e2\source\repos\VanillasLogger\ConsoleApp1\Program.cs:line 27

=== Причина аварии ===
Сообщение: Создание краш-лога из теста
Дата: 2025-09-24 15:00:49.929 +05:00
Исключение:
System.InvalidOperationException: Тестовое исключение
   at Program.Main() in C:\Users\Vanilla76e2\source\repos\VanillasLogger\ConsoleApp1\Program.cs:line 27
```

---

## Лицензия

MIT License.
