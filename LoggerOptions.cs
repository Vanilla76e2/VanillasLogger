namespace VanillasLogger
{
    public class LoggerOptions
    {
        /// <summary>
        /// Директория хранения логов сессии. <para/>
        /// По умолчанию: "Logs/Session" в текущем каталоге приложения.
        /// </summary>
        public string SessionLogsDirectory { get; set; } = "Logs/Session";

        /// <summary>
        /// Директория хранения краш-логов.<para/>
        /// По умолчанию: "Logs/Crashes" в текущем каталоге приложения.<para/>
        /// Каждый краш-лог создаётся в отдельном файле с меткой времени.
        /// </summary>
        public string CrashLogsDirectory { get; set; } = "Logs/Crash";
    }
}
