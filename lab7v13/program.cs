using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Lab7v13
{
    // --- 1. ДОПОМІЖНИЙ КЛАС RETRY HELPER ---
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation, 
            int retryCount = 5, 
            TimeSpan initialDelay = default, 
            Func<Exception, bool> shouldRetry = null)
        {
            if (initialDelay == default) initialDelay = TimeSpan.FromMilliseconds(200);
            
            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    return operation();
                }
                catch (Exception ex) when (shouldRetry == null || shouldRetry(ex))
                {
                    if (attempt > retryCount)
                    {
                        Console.WriteLine($"[Retry] Всі {retryCount} спроб вичерпано. Остання помилка: {ex.Message}");
                        throw;
                    }

                    // Експоненційна затримка: initialDelay * 2^(attempt-1)
                    int delayMs = (int)(initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[Спроба {attempt}] Помилка: {ex.GetType().Name}. Наступна спроба через {delayMs}мс...");
                    Console.ResetColor();

                    Thread.Sleep(delayMs);
                }
            }
        }
    }

    // --- 2. FILE PROCESSOR (Варіант 13) ---
    public class FileProcessor
    {
        private int _readAttempts = 0;

        public double ReadSensorData(string path)
        {
            _readAttempts++;
            // Імітація: перші 2 рази IOException, потім успіх
            if (_readAttempts <= 2)
            {
                throw new IOException("Файл сенсора заблоковано іншим процесом.");
            }

            Console.WriteLine("✅ Дані з файлу успішно зчитано.");
            return 24.5; // Імітація температури
        }
    }

    // --- 3. NETWORK CLIENT (Варіант 13) ---
    public class NetworkClient
    {
        private int _requestAttempts = 0;

        public double GetRemoteSensorData(string url)
        {
            _requestAttempts++;
            // Імітація: перші 4 рази HttpRequestException, потім успіх
            if (_requestAttempts <= 4)
            {
                throw new HttpRequestException("Помилка підключення до віддаленого сенсора (503 Service Unavailable).");
            }

            Console.WriteLine("✅ Мережевий запит виконано успішно.");
            return 26.8;
        }
    }

    // --- 4. MAIN ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var fileProc = new FileProcessor();
            var netClient = new NetworkClient();

            Console.WriteLine("=== Робота з FileProcessor (Retry для IOException) ===");
            try
            {
                double fileData = RetryHelper.ExecuteWithRetry(
                    () => fileProc.ReadSensorData("sensor_log.txt"),
                    retryCount: 3,
                    shouldRetry: ex => ex is IOException
                );
                Console.WriteLine($"Результат з файлу: {fileData}°C\n");
            }
            catch (Exception ex) { Console.WriteLine($"Фінальна помилка файлу: {ex.Message}\n"); }

            Console.WriteLine("=== Робота з NetworkClient (Retry для HttpRequestException) ===");
            try
            {
                double netData = RetryHelper.ExecuteWithRetry(
                    () => netClient.GetRemoteSensorData("https://api.sensors.com/v1"),
                    retryCount: 5,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: ex => ex is HttpRequestException
                );
                Console.WriteLine($"Результат з мережі: {netData}°C");
            }
            catch (Exception ex) { Console.WriteLine($"Фінальна помилка мережі: {ex.Message}"); }

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}
