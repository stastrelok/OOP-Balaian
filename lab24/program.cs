using System;
using System.Collections.Generic;

namespace lab24
{
    // ==========================================
    // 1. ПАТЕРН STRATEGY
    // ==========================================
    
    public interface INumericOperationStrategy
    {
        string Name { get; }
        double Execute(double value);
    }

    public class SquareOperationStrategy : INumericOperationStrategy
    {
        public string Name => "Квадрат";
        public double Execute(double value) => value * value;
    }

    public class CubeOperationStrategy : INumericOperationStrategy
    {
        public string Name => "Куб";
        public double Execute(double value) => value * value * value;
    }

    public class SquareRootOperationStrategy : INumericOperationStrategy
    {
        public string Name => "Квадратний корінь";
        public double Execute(double value) => Math.Sqrt(value);
    }

    public class NumericProcessor
    {
        private INumericOperationStrategy _strategy;

        public NumericProcessor(INumericOperationStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(INumericOperationStrategy strategy)
        {
            _strategy = strategy;
            Console.WriteLine($"\n[Processor]: Стратегію змінено на '{_strategy.Name}'");
        }

        public double Process(double input) => _strategy.Execute(input);
        public string GetCurrentStrategyName() => _strategy.Name;
    }

    // ==========================================
    // 2. ПАТЕРН OBSERVER (Subject через події)
    // ==========================================

    public class ResultPublisher
    {
        // Подія, на яку будуть підписуватися спостерігачі
        public event Action<double, string> ResultCalculated;

        public void PublishResult(double result, string operationName)
        {
            // Виклик події (якщо є підписники)
            ResultCalculated?.Invoke(result, operationName);
        }
    }

    // --- Спостерігачі (Observers) ---

    public class ConsoleLoggerObserver
    {
        public void OnResultCalculated(double result, string opName)
        {
            Console.WriteLine($"[Console Observer]: Результат операції '{opName}' дорівнює {result:F2}");
        }
    }

    public class HistoryLoggerObserver
    {
        private readonly List<string> _history = new List<string>();

        public void OnResultCalculated(double result, string opName)
        {
            string entry = $"{DateTime.Now:HH:mm:ss} - {opName}: {result:F2}";
            _history.Add(entry);
            Console.WriteLine($"[History Observer]: Запис додано до історії (Всього записів: {_history.Count})");
        }
    }

    public class ThresholdNotifierObserver
    {
        private readonly double _threshold;
        public ThresholdNotifierObserver(double threshold) => _threshold = threshold;

        public void OnResultCalculated(double result, string opName)
        {
            if (result > _threshold)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Alert Observer]: УВАГА! Результат {result:F2} перевищує поріг {_threshold}!");
                Console.ResetColor();
            }
        }
    }

    // ==========================================
    // 3. ТОЧКА ВХОДУ (MAIN)
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Ініціалізація Subject та Processor
            var publisher = new ResultPublisher();
            var processor = new NumericProcessor(new SquareOperationStrategy());

            // Ініціалізація спостерігачів
            var consoleLog = new ConsoleLoggerObserver();
            var historyLog = new HistoryLoggerObserver();
            var alertLog = new ThresholdNotifierObserver(100.0);

            // Підписка спостерігачів на подію (Observer pattern)
            publisher.ResultCalculated += consoleLog.OnResultCalculated;
            publisher.ResultCalculated += historyLog.OnResultCalculated;
            publisher.ResultCalculated += alertLog.OnResultCalculated;

            // Демонстрація роботи
            double[] testValues = { 5, 12, 4 };

            foreach (var val in testValues)
            {
                // 1. Обробка
                double res = processor.Process(val);
                
                // 2. Публікація (сповіщення всіх підписників)
                publisher.PublishResult(res, processor.GetCurrentStrategyName());
            }

            // Зміна стратегії в рантаймі (Strategy pattern)
            processor.SetStrategy(new CubeOperationStrategy());
            double cubeRes = processor.Process(6);
            publisher.PublishResult(cubeRes, processor.GetCurrentStrategyName());

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}