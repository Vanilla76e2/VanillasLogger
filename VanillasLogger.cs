using Serilog;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace VanillasLogger
{
    /// <summary>
    /// Реализация логгера, обертывающего Serilog, с поддержкой сессионного и краш-логирования, а также автоматической обработкой исключений.
    /// </summary>
    public class VanillasLogger : ILogger
    {
        private readonly Serilog.Core.Logger _logger;
        private readonly LoggerOptions _options;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="VanillasLogger"/>.
        /// </summary>
        /// <param name="options">Опциональная конфигурация для директорий и настроек логирования. По умолчанию используется новый экземпляр <see cref="LoggerOptions"/>, если значение <see langword="null"/>.</param>
        public VanillasLogger(LoggerOptions? options = null)
        {
            _options = options ?? new LoggerOptions();

            Directory.CreateDirectory(_options.SessionLogsDirectory);
            Directory.CreateDirectory(_options.CrashLogsDirectory);

            string sessionLogPath = GetSessionLogPath();

            if (File.Exists(sessionLogPath))
                    File.Delete(sessionLogPath);

            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(
                    path: sessionLogPath,
                    rollingInterval: RollingInterval.Infinite,
                    retainedFileCountLimit: 1)
                .CreateLogger();

            Log.Logger = _logger;

            // Подписываемся на глобальные необработанные исключения
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            // Гарантируем flush всех логов при завершении процесса
            AppDomain.CurrentDomain.ProcessExit += (s, e) => _logger.Dispose();
        }

        /// <summary>
        /// Получает путь к файлу сессионного лога.
        /// </summary>
        /// <returns>Полный путь к файлу сессионного лога.</returns>
        private string GetSessionLogPath()
        {
            return Path.Combine(_options.SessionLogsDirectory, $"session.log");
        }

        /// <summary>
        /// Получает путь к файлу краш-лога с временной меткой.
        /// </summary>
        /// <returns>Полный путь к файлу краш-лога.</returns>
        private string GetCrashLogPath()
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return Path.Combine(_options.CrashLogsDirectory, $"crash_{timestamp}.log");
        }

        /// <summary>
        /// Создает файл краш-лога, включая содержимое сессионного лога и детали исключения.
        /// </summary>
        /// <param name="ex">Исключение для логирования, если есть.</param>
        /// <param name="message">Сообщение, описывающее крах.</param>
        public void CreateCrashLog(Exception? ex, string message)
        {
            try
            {
                _logger.Dispose();

                string crashLogPath = GetCrashLogPath();
                string sessionLogPath = GetSessionLogPath();

                using (var writer = new StreamWriter(crashLogPath, append: true))
                {
                    if (File.Exists(sessionLogPath))
                    {
                        string sessionContent = File.ReadAllText(sessionLogPath);
                        writer.WriteLine("=== Журнал сессии ===");
                        writer.WriteLine(sessionContent);
                        writer.WriteLine();
                    }

                    writer.WriteLine("=== Причина аварии ===");
                    writer.WriteLine($"Сообщение: {message}");
                    writer.WriteLine($"Дата: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}");
                    writer.WriteLine("Исключение:");

                    if (ex != null)
                    {
                        writer.WriteLine(ex.ToString());
                    }
                    else
                    {
                        writer.WriteLine("Исключение не было передано.");
                    }

                    writer.Flush();
                }
            }
            catch
            {
                Debug.WriteLine("Failed to create crash log.");
            }
        }

        /// <summary>
        /// Извлекает контекст (имя файла без расширения) из пути к файлу.
        /// </summary>
        /// <param name="filepath">Путь к файлу для извлечения контекста.</param>
        /// <returns>Строка контекста или "Unknown", если извлечение не удалось.</returns>
        private static string GetContextFromFilePath(string filepath)
        {
            try
            {
                return Path.GetFileNameWithoutExtension(filepath);
            }
            catch
            {
                return "Unknown";
            }
        }

        #region Обработка аварий
        /// <summary>
        /// Обработчик необработанных исключений в домене приложения.
        /// </summary>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            CreateCrashLog(ex, "Необработанное исключение (AppDomain)");
        }

        /// <summary>
        /// Обработчик необработанных исключений в задачах.
        /// </summary>
        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            var ex = e.Exception.InnerException;
            CreateCrashLog(ex, "Необработанное исключение в задаче (Task)");
        }
        #endregion

        /// <summary>
        /// Закрывает логгер, отписываясь от событий и освобождая ресурсы.
        /// </summary>
        public void Close()
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
            _logger.Dispose();
        }

        #region Реализация ILogger
        /// <summary>
        /// Логирует отладочное сообщение с контекстом и информацией о вызывающем методе.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (автоматически заполняется).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (автоматически заполняется).</param>
        public void LogDebug(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
        {
            var context = GetContextFromFilePath(filePath);
            _logger.Debug($"{context}.{memberName}: {message}");
        }

        /// <summary>
        /// Логирует информационное сообщение с контекстом и информацией о вызывающем методе.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (автоматически заполняется).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (автоматически заполняется).</param>
        public void LogInformation(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
        {
            var context = GetContextFromFilePath(filePath);
            _logger.Information($"{context}.{memberName}: {message}");
        }

        /// <summary>
        /// Логирует предупреждающее сообщение с контекстом и информацией о вызывающем методе.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (автоматически заполняется).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (автоматически заполняется).</param>
        public void LogWarning(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
        {
            var context = GetContextFromFilePath(filePath);
            _logger.Warning($"{context}.{memberName}: {message}");
        }

        /// <summary>
        /// Логирует сообщение об ошибке с контекстом и информацией о вызывающем методе.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (автоматически заполняется).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (автоматически заполняется).</param>
        public void LogError(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
        {
            var context = GetContextFromFilePath(filePath);
            _logger.Error($"{context}.{memberName}: {message}");
        }

        /// <summary>
        /// Логирует сообщение об ошибке с исключением, контекстом и информацией о вызывающем методе.
        /// </summary>
        /// <param name="ex">Исключение для логирования.</param>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (автоматически заполняется).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (автоматически заполняется).</param>
        public void LogError(Exception ex, string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
        {
            var context = GetContextFromFilePath(filePath);
            _logger.Error($"{context}.{memberName}: {message}\n{ex}");
        }

        /// <summary>
        /// Логирует критическое сообщение с контекстом и информацией о вызывающем методе.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (автоматически заполняется).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (автоматически заполняется).</param>
        public void LogCritical(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
        {
            var context = GetContextFromFilePath(filePath);
            _logger.Fatal($"{context}.{memberName}: {message}");
        }

        /// <summary>
        /// Логирует критическое сообщение с исключением, контекстом и информацией о вызывающем методе.
        /// </summary>
        /// <param name="ex">Исключение для логирования.</param>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (автоматически заполняется).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (автоматически заполняется).</param>
        public void LogCritical(Exception ex, string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
        {
            var context = GetContextFromFilePath(filePath);
            _logger.Fatal($"{context}.{memberName}: {message}\n{ex}");
        }
        #endregion
    }
}
