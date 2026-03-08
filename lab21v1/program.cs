using System;
using System.Collections.Generic;

namespace lab21
{
    // --- 1. ІНТЕРФЕЙС СТРАТЕГІЇ (OCP) ---
    public interface ITaxiStrategy
    {
        decimal CalculatePrice(double distance, int idleMinutes);
    }

    // --- 2. РЕАЛІЗАЦІЇ СТРАТЕГІЙ ---
    public class EconomyTaxiStrategy : ITaxiStrategy
    {
        public decimal CalculatePrice(double distance, int idleMinutes) =>
            (decimal)(distance * 8.0 + idleMinutes * 2.0 + 30.0); // 30 - подача
    }

    public class StandardTaxiStrategy : ITaxiStrategy
    {
        public decimal CalculatePrice(double distance, int idleMinutes) =>
            (decimal)(distance * 12.0 + idleMinutes * 3.5 + 45.0);
    }

    public class PremiumTaxiStrategy : ITaxiStrategy
    {
        public decimal CalculatePrice(double distance, int idleMinutes) =>
            (decimal)(distance * 25.0 + idleMinutes * 6.0 + 100.0);
    }

    // --- 3. ДЕМОНСТРАЦІЯ РОЗШИРЮВАНОСТІ (4-та стратегія без зміни сервісу) ---
    public class NightTaxiStrategy : ITaxiStrategy
    {
        public decimal CalculatePrice(double distance, int idleMinutes) =>
            (decimal)(distance * 15.0 + idleMinutes * 4.0 + 60.0) * 1.2m; // +20% нічний коефіцієнт
    }

    // --- 4. ФАБРИКА СТРАТЕГІЙ ---
    public static class TaxiStrategyFactory
    {
        public static ITaxiStrategy CreateStrategy(string taxiType)
        {
            return taxiType.ToLower() switch
            {
                "economy" => new EconomyTaxiStrategy(),
                "standard" => new StandardTaxiStrategy(),
                "premium" => new PremiumTaxiStrategy(),
                "night" => new NightTaxiStrategy(),
                _ => throw new ArgumentException("Невідомий тип таксі")
            };
        }
    }

    // --- 5. СЕРВІС (КОНТЕКСТ) ---
    public class TaxiRideService
    {
        // Клас не залежить від конкретних реалізацій, тільки від інтерфейсу
        public decimal CalculateTotal(double distance, int idleMinutes, ITaxiStrategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return strategy.CalculatePrice(distance, idleMinutes);
        }
    }

    // --- 6. ГОЛОВНИЙ ЗАПУСК ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var rideService = new TaxiRideService();

            Console.WriteLine("=== Калькулятор вартості таксі (OCP Demo) ===");
            
            try 
            {
                Console.Write("Введіть тип таксі (Economy, Standard, Premium, Night): ");
                string type = Console.ReadLine();

                Console.Write("Введіть відстань (км): ");
                double dist = double.Parse(Console.ReadLine());

                Console.Write("Введіть час простою (хв): ");
                int idle = int.Parse(Console.ReadLine());

                // Використання Фабрики
                ITaxiStrategy selectedStrategy = TaxiStrategyFactory.CreateStrategy(type);

                // Розрахунок через Сервіс
                decimal finalPrice = rideService.CalculateTotal(dist, idle, selectedStrategy);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nВартість поїздки ({type}): {finalPrice:F2} грн.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Помилка: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}