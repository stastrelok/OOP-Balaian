using System;
using System.Collections.Generic;

namespace hw1
{
    // Клас для обгортки даних у кеші з міткою часу
    public class CacheItem<T>
    {
        public T Value { get; set; }
        public DateTime AddedAt { get; set; }

        public CacheItem(T value)
        {
            Value = value;
            AddedAt = DateTime.Now;
        }
    }

    // Узагальнений кеш з обмеженнями: тільки класи з конструктором
    public class Cache<T> where T : class, new()
    {
        private readonly List<CacheItem<T>> _items = new List<CacheItem<T>>();
        private readonly int _maxSize;

        public Cache(int maxSize)
        {
            _maxSize = maxSize;
        }

        public void Add(T item)
        {
            // Алгоритм видалення старого елемента, якщо кеш повний (FIFO)
            if (_items.Count >= _maxSize)
            {
                Console.WriteLine("Кеш переповнений. Видаляємо найстаріший елемент...");
                _items.RemoveAt(0); 
            }

            _items.Add(new CacheItem<T>(item));
        }

        // Алгоритм сортування бульбашкою (Bubble Sort) без LINQ
        // Сортуємо за часом додавання (від нових до старих)
        public void SortByTime()
        {
            int n = _items.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (_items[j].AddedAt < _items[j + 1].AddedAt)
                    {
                        var temp = _items[j];
                        _items[j] = _items[j + 1];
                        _items[j + 1] = temp;
                    }
                }
            }
        }

        public void PrintCache()
        {
            foreach (var item in _items)
            {
                Console.WriteLine($"[{item.AddedAt:HH:mm:ss.fff}] - {item.Value}");
            }
        }
    }

    // Тестовий клас, що відповідає обмеженням (class, new())
    public class LogEntry 
    {
        public string Message { get; set; }
        public override string ToString() => Message;
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var myCache = new Cache<LogEntry>(3); // Кеш на 3 елементи

            Console.WriteLine("--- Додавання елементів ---");
            myCache.Add(new LogEntry { Message = "Запис 1" });
            System.Threading.Thread.Sleep(100); // Пауза для різниці в часі
            myCache.Add(new LogEntry { Message = "Запис 2" });
            myCache.Add(new LogEntry { Message = "Запис 3" });
            
            // Цей виклик видалить "Запис 1"
            myCache.Add(new LogEntry { Message = "Запис 4" });

            Console.WriteLine("\n--- Вміст кешу ---");
            myCache.PrintCache();

            Console.WriteLine("\n--- Сортування (Bubble Sort) ---");
            myCache.SortByTime();
            myCache.PrintCache();
        }
    }
}