using System;
using System.Collections.Generic;
using System.Linq;

public class Product
{

    private string _name;
    private decimal _price;

    public string Name
    {
        get { return _name; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Назва товару не може бути пустою.");
            }
            _name = value;
        }
    }

    public decimal Price
    {
        get { return _price; }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Ціна товару не може бути від'ємною.");
            }
            _price = value;
        }
    }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public override string ToString()
    {
        return $"[Назва: {Name}, Ціна: {Price:C}]";
    }
}

public class ProductCatalog
{
    private List<Product> _products;

    public int Count
    {
        get { return _products.Count; }
    }

    public ProductCatalog()
    {
        _products = new List<Product>();
        Console.WriteLine("Каталог товарів створено.");
    }

    ~ProductCatalog()
    {
        Console.WriteLine($"Деструктор: Каталог '{GetHashCode()}' очищується. Кількість товарів: {Count}.");
    }

    public Product this[int index]
    {
        get
        {
            if (index < 0 || index >= _products.Count)
            {
                throw new IndexOutOfRangeException("Індекс товару виходить за межі діапазону.");
            }
            return _products[index];
        }
        set
        {
            if (index < 0 || index >= _products.Count)
            {
                throw new IndexOutOfRangeException("Індекс товару виходить за межі діапазону.");
            }
            _products[index] = value;
        }
    }

    public Product this[string name]
    {
        get
        {
            return _products.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        set
        {
            int index = _products.FindIndex(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (index != -1)
            {
                _products[index] = value;
            }
            else
            {
                _products.Add(value);
                Console.WriteLine($"Товар '{name}' не знайдено, додано новий товар.");
            }
        }
    }

    public static ProductCatalog operator +(ProductCatalog catalog, Product product)
    {
        if (catalog == null || product == null) return catalog;
        if (catalog._products.Any(p => p.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine($"Попередження: Товар '{product.Name}' вже існує в каталозі. Не додано дублікат.");
        }
        else
        {
            catalog._products.Add(product);
            Console.WriteLine($"Оператор '+': Товар '{product.Name}' додано.");
        }
        return catalog;
    }

    public static ProductCatalog operator -(ProductCatalog catalog, string productName)
    {
        if (catalog == null || string.IsNullOrWhiteSpace(productName)) return catalog;
        Product productToRemove = catalog._products.FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
        if (productToRemove != null)
        {
            catalog._products.Remove(productToRemove);
            Console.WriteLine($"Оператор '-': Товар '{productName}' видалено.");
        }
        else
        {
            Console.WriteLine($"Оператор '-': Товар '{productName}' не знайдено в каталозі.");
        }
        return catalog;
    }
   
    public void DisplayCatalog()
    {
        Console.WriteLine($"\n--- Каталог товарів (Кількість: {Count}) ---");
        if (Count == 0)
        {
            Console.WriteLine("Каталог порожній.");
            return;
        }
        for (int i = 0; i < _products.Count; i++)
        {
            Console.WriteLine($"[{i}] {_products[i]}");
        }
        Console.WriteLine("----------------------------------");
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        ProductCatalog myCatalog = new ProductCatalog();

        Console.WriteLine("\n--- Демонстрація оператора '+' (додавання товарів) ---");
        
        myCatalog = myCatalog + new Product("Молоко", 35.50m);
        myCatalog = myCatalog + new Product("Хліб", 20.00m);
        myCatalog = myCatalog + new Product("Яблука", 45.00m);
        myCatalog = myCatalog + new Product("Молоко", 35.50m);

        myCatalog.DisplayCatalog();

        Console.WriteLine("\n--- Демонстрація індексаторів ---");
                try
        {
            Console.WriteLine($"Товар за індексом 0: {myCatalog[0]}");
            myCatalog[0].Price = 37.00m;
            Console.WriteLine($"Товар за індексом 0 (нова ціна): {myCatalog[0]}");
        }
        catch (IndexOutOfRangeException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }

        Product apples = myCatalog["Яблука"];
        if (apples != null)
        {
            Console.WriteLine($"Знайдено товар 'Яблука': {apples}");
            apples.Price = 48.00m;
            Console.WriteLine($"'Яблука' (нова ціна через пошук): {myCatalog["Яблука"]}");
        }

        Product unknown = myCatalog["Неіснуючий товар"];
        if (unknown == null)
        {
            Console.WriteLine("Товар 'Неіснуючий товар' не знайдено.");
        }

        myCatalog.DisplayCatalog();

        Console.WriteLine("\n--- Демонстрація оператора '-' (видалення товарів) ---");
        myCatalog = myCatalog - "Хліб";
        myCatalog = myCatalog - "Неіснуючий товар";
        myCatalog.DisplayCatalog();

        Console.WriteLine("\n--- Демонстрація зміни товару через індексатор за назвою ---");
        myCatalog["Молоко"] = new Product("Молоко", 39.90m);
        myCatalog["Сир"] = new Product("Сир", 120.00m);

        myCatalog.DisplayCatalog();

        Console.WriteLine("\nПрограма завершила роботу.");
    }
}