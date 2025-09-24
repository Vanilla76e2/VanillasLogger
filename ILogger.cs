using System.Runtime.CompilerServices;

namespace VanillasLogger
{
    /// <summary>
    /// Интерфейс для логирования сообщений и исключений с поддержкой различных уровней логирования и краш-логов.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Логирует сообщение уровня Debug с контекстом и информацией о вызывающем.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (заполняется автоматически).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (заполняется автоматически).</param>
        public void LogDebug(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "");
        
        /// <summary>
        /// Логирует сообщение уровня Information с контекстом и информацией о вызывающем.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (заполняется автоматически).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (заполняется автоматически).</param>
        public void LogInformation(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "");

        /// <summary>
        /// Логирует сообщение уровня Warning с контекстом и информацией о вызывающем.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (заполняется автоматически).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (заполняется автоматически).</param>
        public void LogWarning(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "");

        /// <summary>
        /// Логирует сообщение уровня Error с контекстом и информацией о вызывающем.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (заполняется автоматически).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (заполняется автоматически).</param>
        public void LogError(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "");

        /// <summary>
        /// Логирует сообщение уровня Error с исключением, контекстом и информацией о вызывающем.
        /// </summary>
        /// <param name="ex">Исключение для логирования.</param>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (заполняется автоматически).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (заполняется автоматически).</param>
        public void LogError(Exception ex, string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "");

        /// <summary>
        /// Логирует сообщение уровня Critical с контекстом и информацией о вызывающем.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (заполняется автоматически).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (заполняется автоматически).</param>
        public void LogCritical(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "");

        /// <summary>
        /// Логирует сообщение уровня Critical с исключением, контекстом и информацией о вызывающем.
        /// </summary>
        /// <param name="ex">Исключение для логирования.</param>
        /// <param name="message">Сообщение для логирования.</param>
        /// <param name="memberName">Имя вызывающего метода (заполняется автоматически).</param>
        /// <param name="filePath">Путь к файлу вызывающего кода (заполняется автоматически).</param>
        public void LogCritical(Exception ex, string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "");

        /// <summary>
        /// Создает файл лога краша, включая содержимое лога сессии и детали исключения.
        /// </summary>
        /// <param name="ex">Исключение для логирования, если есть.</param>
        /// <param name="message">Сообщение, описывающее краш.</param>
        void CreateCrashLog(Exception? ex, string message);
    }
}
