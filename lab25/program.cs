using System;
using System.Collections.Generic;
using System.IO;

namespace lab25
{
    // ==========================================
    // 1. ПАТЕРНИ FACTORY METHOD ТА LOGGER
    // ==========================================
    public interface ILogger { void Log(string message); }

    public class ConsoleLogger : ILogger 
    {
        public void Log(string message) => Console.WriteLine($"[Console Log]: {message}");
    }

    public class FileLogger : ILogger 
    {
        private readonly string _path = "log.txt";
        public void Log(string message) 
        {
            File.AppendAllText(_path, $"[File Log {DateTime.Now}]: {message}{Environment.NewLine}");
            Console.WriteLine($"[System]: Повідомлення записано у файл {_path}");
        }
    }

    public abstract class LoggerFactory { public abstract ILogger CreateLogger(); }
    public class ConsoleLoggerFactory : LoggerFactory { public override ILogger CreateLogger() => new ConsoleLogger(); }
    public class FileLoggerFactory : LoggerFactory { public override ILogger CreateLogger() => new FileLogger(); }

    // ==========================================
    // 2. ПАТЕРН SINGLETON (LoggerManager)
    // ==========================================
    public class LoggerManager
    {
        private static LoggerManager _instance;
        private LoggerFactory _factory;
        private ILogger _currentLogger;

        private LoggerManager() { }

        public static LoggerManager Instance => _instance ??= new LoggerManager();

        public void SetFactory(LoggerFactory factory)
        {
            _factory = factory;
            _currentLogger = _factory.CreateLogger();
            Console.WriteLine($"\n[Manager]: Фабрику логування змінено на {factory.GetType().Name}");
        }

        public void Log(string message) => _currentLogger?.Log(message);
    }

    // ==========================================
    // 3. ПАТЕРН STRATEGY (Data Processing)
    // ==========================================
    public interface IDataProcessorStrategy { string Process(string data); }

    public class EncryptDataStrategy : IDataProcessorStrategy 
    {
        public string Process(string data) => $"Encrypted({Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data))})";
    }

    public class CompressDataStrategy : IDataProcessorStrategy 
    {
        public string Process(string data) => $"Compressed({data.Length} bytes -> {data.Length / 2} bytes)";
    }

    public class DataContext
    {
        private IDataProcessorStrategy _strategy;
        public DataContext(IDataProcessorStrategy strategy) => _strategy = strategy;
        public void SetStrategy(IDataProcessorStrategy strategy) => _strategy = strategy;
        public string ExecuteStrategy(string data) => _strategy.Process(data);
    }

    // ==========================================
    // 4. ПАТЕРН OBSERVER (Data Notification)
    // ==========================================
    public class DataPublisher
    {
        public event Action<string> DataProcessed;
        public void PublishDataProcessed(string result) => DataProcessed?.Invoke(result);
    }

    public class ProcessingLoggerObserver
    {
        public void OnDataProcessed(string result)
        {
            // Використання Singleton для логування результату
            LoggerManager.Instance.Log($"Спостерігач отримав результат: {result}");
        }
    }

    // ==========================================
    // 5. ТОЧКА ВХОДУ (MAIN SCENARIOS)
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // --- СЦЕНАРІЙ 1: Повна інтеграція ---
            PrintHeader("СЦЕНАРІЙ 1: Повна інтеграція");
            
            LoggerManager.Instance.SetFactory(new ConsoleLoggerFactory());
            var context = new DataContext(new EncryptDataStrategy());
            var publisher = new DataPublisher();
            var observer = new ProcessingLoggerObserver();

            publisher.DataProcessed += observer.OnDataProcessed;

            string rawData = "SecretData123";
            string processed = context.ExecuteStrategy(rawData);
            publisher.PublishDataProcessed(processed);


            // --- СЦЕНАРІЙ 2: Динамічна зміна логера ---
            PrintHeader("СЦЕНАРІЙ 2: Динамічна зміна логера");
            
            LoggerManager.Instance.SetFactory(new FileLoggerFactory());
            processed = context.ExecuteStrategy("NextDataBatch");
            publisher.PublishDataProcessed(processed);


            // --- СЦЕНАРІЙ 3: Динамічна зміна стратегії ---
            PrintHeader("СЦЕНАРІЙ 3: Динамічна зміна стратегії");
            
            context.SetStrategy(new CompressDataStrategy());
            processed = context.ExecuteStrategy("LargeDataStringExample");
            publisher.PublishDataProcessed(processed);

            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine("Всі сценарії завершені успішно.");
            Console.ReadKey();
        }

        static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n--- {title} ---");
            Console.ResetColor();
        }
    }
}