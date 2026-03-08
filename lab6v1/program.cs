using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6v1
{
    // --- 1. ВЛАСНИЙ ДЕЛЕГАТ ---
    // Делегат для виконання операцій над ціною (наприклад, знижка або податок)
    public delegate double PriceOperation(double price);

    // КЛАС СУТНОСТІ
    public class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }

        public override string ToString() => $"{Name} [{Category}] - {Price:C}";
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Налаштування консолі для виводу валюти та спецсимволів
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Створення колекції товарів
            List<Product> products = new List<Product>
            {
                new Product { Name = "Laptop", Price = 25000, Category = "Electronics" },
                new Product { Name = "Smartphone", Price = 15000, Category = "Electronics" },
                new Product { Name = "Coffee Maker", Price = 3000, Category = "Appliances" },
                new Product { Name = "Monitor", Price = 8000, Category = "Electronics" },
                new Product { Name = "Bread", Price = 25, Category = "Food" }
            };

            Console.WriteLine("=== Лабораторна робота №6: Лямбда-вирази та Делегати ===\n");

            // --- 2. ВИКОРИСТАННЯ ВЛАСНОГО ДЕЛЕГАТА ---
            // Лямбда-вираз для розрахунку ціни з ПДВ (20%)
            PriceOperation calculateTax = (p) => p * 1.2;
            Console.WriteLine($"Ціна Laptop з ПДВ: {calculateTax(25000):F2}\n");


            // --- 3. ВБУДОВАНІ ДЕЛЕГАТИ (Func, Action, Predicate) ---

            // Predicate<T>: Перевірка, чи є товар дорогим (> 5000)
            Predicate<Product> isExpensive = p => p.Price > 5000;

            // Func<T, TResult>: Форматування назви товару у верхній регістр
            Func<Product, string> upperName = p => p.Name.ToUpper();

            // Action<T>: Простий вивід інформації про товар
            Action<Product> printProduct = p => Console.WriteLine($"-> {p}");


            // --- 4. ОБРОБКА КОЛЕКЦІЇ ТА LINQ ---

            // Фільтрація товарів за ціною (Where)
            Console.WriteLine("Товари дорожче 5000 грн (Predicate):");
            var expensiveItems = products.Where(p => isExpensive(p)).ToList();
            expensiveItems.ForEach(printProduct);

            // Пошук найдорожчого товару (OrderByDescending + FirstOrDefault)
            var mostExpensive = products.OrderByDescending(p => p.Price).FirstOrDefault();
            Console.WriteLine($"\nНайдорожчий товар: {mostExpensive?.Name} ({mostExpensive?.Price} грн)");

            // Обчислення середньої вартості (Average)
            // Використовуємо Func<Product, double> всередині методу Average
            double averagePrice = products.Average(p => p.Price);
            Console.WriteLine($"Середня вартість товарів: {averagePrice:F2} грн");

            // Агрегація: Підрахунок загальної вартості електроніки (Aggregate або Sum)
            double electronicsTotal = products
                .Where(p => p.Category == "Electronics")
                .Sum(p => p.Price);
            Console.WriteLine($"Загальна вартість категорії Electronics: {electronicsTotal} грн");


            // --- 5. ПРИКЛАД АНОНІМНОГО МЕТОДУ ---
            // (Використовується рідше за лямбди, але є у вимогах)
            Action<string> welcomeMessage = delegate (string name) 
            {
                Console.WriteLine($"\nЗвіт сформовано користувачем: {name}");
            };
            welcomeMessage("Admin");

            Console.ReadKey();
        }
    }
}