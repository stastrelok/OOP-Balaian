using System;
using System.Collections.Generic;
using Polly;
using Polly.CircuitBreaker;

namespace IndependentWork11
{
    class Program
    {
        // Лічильники для імітації помилок
        private static int _dbAttempts = 0;
        private static int _serviceAttempts = 0;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // СЦЕНАРІЙ 1: Circuit Breaker для Бази Даних
            ExecuteDatabaseScenario();

            Console.WriteLine("\n" + new string('-', 50) + "\n");

            // СЦЕНАРІЙ 2: Fallback для отримання курсу валют
            ExecuteCurrencyFallbackScenario();
        }

        #region Сценарій 1: Circuit Breaker (Розривач ланцюга)
        /*
         * ПРОБЛЕМА: Якщо БД "лежить", постійні спроби підключення лише перевантажують її 
         * та змушують користувача чекати тайм-аутів.
         * ПОЛІТИКА: Circuit Breaker. Якщо стається 2 помилки поспіль, "ланцюг" розмикається 
         * на 5 секунд, і всі наступні запити відхиляються миттєво без спроби підключення.
         */
        static void ExecuteDatabaseScenario()
        {
            Console.WriteLine(">>> СЦЕНАРІЙ 1: Circuit Breaker (Захист БД)");

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(5),
                    onBreak: (ex, breakDelay) => Console.WriteLine($"[BREAK] Ланцюг розімкнуто на {breakDelay.TotalSeconds}с! Причина: {ex.Message}"),
                    onReset: () => Console.WriteLine("[RESET] Ланцюг знову замкнуто. БД працює."),
                    onHalfOpen: () => Console.WriteLine("[HALF-OPEN] Перевірочна спроба...")
                );

            for (int i = 1; i <= 5; i++)
            {
                try
                {
                    Console.WriteLine($"\nСпроба запиту №{i}:");
                    circuitBreakerPolicy.Execute(() => ConnectToDb());
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine("⚠️ Запит відхилено політикою Polly: Ланцюг розімкнуто (БД недоступна).");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Помилка: {ex.Message}");
                }
                System.Threading.Thread.Sleep(1000); // Крок у 1 секунду
            }
        }

        static void ConnectToDb()
        {
            _dbAttempts++;
            // Імітуємо, що БД лежить довгий час
            throw new Exception("Timeout: Не вдалося встановити з'єднання з SQL Server.");
        }
        #endregion

        #region Сценарій 2: Fallback (Резервний варіант)
        /*
         * ПРОБЛЕМА: Зовнішній сервіс курсів валют не відповідає. Додаток не може зупинити роботу.
         * ПОЛІТИКА: Fallback. Якщо основний запит провалюється, повертаємо заздалегідь 
         * кешоване значення або значення за замовчуванням.
         */
        static void ExecuteCurrencyFallbackScenario()
        {
            Console.WriteLine(">>> СЦЕНАРІЙ 2: Fallback (Резервні дані)");

            var fallbackPolicy = Policy<double>
                .Handle<Exception>()
                .Fallback(
                    fallbackValue: 41.5, // Значення за замовчуванням (курс з кешу)
                    onFallback: (result) => Console.WriteLine("⚠️ Сервіс валют недоступний. Використовуємо курс із КЕШУ.")
                );

            double currentRate = fallbackPolicy.Execute(() => GetLiveExchangeRate());
            Console.WriteLine($"Поточний курс USD: {currentRate} грн.");
        }

        static double GetLiveExchangeRate()
        {
            _serviceAttempts++;
            throw new Exception("500 Internal Server Error");
        }
        #endregion
    }
}