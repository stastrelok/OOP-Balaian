using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork16
{
    // --- 1. МОДЕЛІ ДАНИХ ---
    public class RawData 
    { 
        public string Id { get; set; } 
        public string Name { get; set; } 
        public string Value { get; set; } 
    }

    public class ProcessedData 
    { 
        public int Id { get; set; } 
        public string Name { get; set; } 
        public decimal Price { get; set; } 
    }

    // --- 2. ІНТЕРФЕЙСИ (Розподіл відповідальностей) ---
    public interface ICsvReader { IEnumerable<RawData> Read(string path); }
    public interface IDataValidator { bool IsValid(RawData data); }
    public interface IDataTransformer { ProcessedData Transform(RawData data); }
    public interface IDataRepository { void Save(ProcessedData data); }

    // --- 3. РЕАЛІЗАЦІЯ КОМПОНЕНТІВ ---
    public class CsvReader : ICsvReader 
    {
        public IEnumerable<RawData> Read(string path) => new List<RawData> {
            new RawData { Id = "1", Name = "Laptop", Value = "1500" },
            new RawData { Id = "2", Name = "", Value = "500" }, // Невалідний (пусте ім'я)
            new RawData { Id = "3", Name = "Smartphone", Value = "800" }
        };
    }

    public class DataValidator : IDataValidator 
    {
        public bool IsValid(RawData data) => !string.IsNullOrEmpty(data.Name);
    }

    public class DataTransformer : IDataTransformer 
    {
        public ProcessedData Transform(RawData data) => new ProcessedData {
            Id = int.Parse(data.Id), 
            Name = data.Name.ToUpper(), 
            Price = decimal.Parse(data.Value)
        };
    }

    public class DataRepository : IDataRepository 
    {
        public void Save(ProcessedData data) => 
            Console.WriteLine($"[БАЗА ДАНИХ] Збережено: {data.Name} за ціною {data.Price} грн.");
    }

    // --- 4. ГОЛОВНИЙ СЕРВІС (Оркестратор - SRP & DIP) ---
    public class DataImportService
    {
        private readonly ICsvReader _reader;
        private readonly IDataValidator _validator;
        private readonly IDataTransformer _transformer;
        private readonly IDataRepository _repository;

        // Впровадження залежностей через конструктор
        public DataImportService(ICsvReader reader, IDataValidator validator, IDataTransformer transformer, IDataRepository repository)
        {
            _reader = reader;
            _validator = validator;
            _transformer = transformer;
            _repository = repository;
        }

        public void ExecuteImport(string path)
        {
            Console.WriteLine($"--- Початок імпорту з файлу: {path} ---");
            var rawRecords = _reader.Read(path);

            foreach (var record in rawRecords)
            {
                if (_validator.IsValid(record))
                {
                    var processed = _transformer.Transform(record);
                    _repository.Save(processed);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[ПРОПУЩЕНО] Запис з ID {record.Id} не пройшов валідацію.");
                    Console.ResetColor();
                }
            }
            Console.WriteLine("--- Імпорт завершено ---");
        }
    }

    // --- 5. ТОЧКА ВХОДУ ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Створюємо екземпляри конкретних реалізацій
            var reader = new CsvReader();
            var validator = new DataValidator();
            var transformer = new DataTransformer();
            var repository = new DataRepository();

            // Збираємо головний сервіс (Manual Dependency Injection)
            var importer = new DataImportService(reader, validator, transformer, repository);

            // Виконуємо роботу
            importer.ExecuteImport("products_data.csv");

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
} 