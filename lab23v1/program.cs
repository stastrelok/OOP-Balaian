using System;

namespace lab23
{
    // =================================================================
    // 1. ПОЧАТКОВА СТРУКТУРА (ПОРУШЕННЯ ISP ТА DIP)
    // =================================================================
    
    // Конкретні класи (модулі нижнього рівня)
    public class LegacyLogger 
    { 
        public void LogToConsole(string msg) => Console.WriteLine($"[Legacy Log]: {msg}"); 
    }

    public class LegacyEmailSender 
    { 
        public void Send(string to, string body) => Console.WriteLine($"[Legacy Email]: To {to}, Msg: {body}"); 
    }

    // Клас вищого рівня з жорсткими залежностями
    public class BadSystemMonitor
    {
        // Порушення DIP: створюємо об'єкти всередині (tight coupling)
        private readonly LegacyLogger _logger = new LegacyLogger();
        private readonly LegacyEmailSender _email = new LegacyEmailSender();

        public void Run()
        {
            _logger.LogToConsole("Перевірка системи...");
            _email.Send("admin@test.com", "Система працює");
        }
    }

    // =================================================================
    // 2. РЕФАКТОРИНГ (ЗАСТОСУВАННЯ ISP ТА DIP)
    // =================================================================

    // --- ISP: Розділення інтерфейсів на вузькоспеціалізовані ---
    public interface ILogger 
    { 
        void Log(string message); 
    }

    public interface INotifier 
    { 
        void Notify(string target, string subject); 
    }

    public interface IReportGenerator 
    { 
        void Generate(string fileName); 
    }

    // --- Реалізація інтерфейсів ---
    public class ConsoleLogger : ILogger 
    {
        public void Log(string message) => Console.WriteLine($"[LOG]: {message}");
    }

    public class EmailNotifier : INotifier 
    {
        public void Notify(string target, string message) => 
            Console.WriteLine($"[NOTIFICATION]: Надіслано Email на {target} з текстом: {message}");
    }

    public class FileReportGenerator : IReportGenerator 
    {
        public void Generate(string fileName) => 
            Console.WriteLine($"[REPORT]: Звіт '{fileName}' успішно згенеровано.");
    }

    // --- DIP: Інверсія залежностей через конструктор (Dependency Injection) ---
    public class GoodSystemMonitor
    {
        private readonly ILogger _logger;
        private readonly INotifier _notifier;
        private readonly IReportGenerator _reportGenerator;

        // Тепер клас залежить від абстракцій (інтерфейсів), а не від конкретики
        public GoodSystemMonitor(ILogger logger, INotifier notifier, IReportGenerator reportGenerator)
        {
            _logger = logger;
            _notifier = notifier;
            _reportGenerator = reportGenerator;
        }

        public void CheckSystem()
        {
            _logger.Log("Ініціалізація перевірки компонентів...");
            _reportGenerator.Generate("SystemHealth_March2026.pdf");
            _notifier.Notify("it-support@company.com", "Моніторинг завершено без помилок.");
        }
    }

    // =================================================================
    // 3. ТОЧКА ВХОДУ (MAIN)
    // =================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ДЕМОНСТРАЦІЯ ДО РЕФАКТОРИНГУ (Жорсткі зв'язки) ===");
            var badMonitor = new BadSystemMonitor();
            badMonitor.Run();

            Console.WriteLine("\n" + new string('-', 50) + "\n");

            Console.WriteLine("=== ДЕМОНСТРАЦІЯ ПІСЛЯ РЕФАКТОРИНГУ (ISP + DIP + DI) ===");
            
            // 1. Створюємо залежності (можуть бути легко змінені на інші реалізації)
            ILogger myLogger = new ConsoleLogger();
            INotifier myNotifier = new EmailNotifier();
            IReportGenerator myReportGen = new FileReportGenerator();

            // 2. Впроваджуємо їх у головний клас (Dependency Injection)
            var goodMonitor = new GoodSystemMonitor(myLogger, myNotifier, myReportGen);

            // 3. Виконуємо роботу
            goodMonitor.CheckSystem();

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}