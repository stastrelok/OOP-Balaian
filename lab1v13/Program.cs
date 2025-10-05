using System;
using System.Text;
using System.Collections.Generic; 

public class Library
{
    public string Name;
    public string Address;

    private List<string> _books;

    public int BooksCount
    {
        get { return _books.Count; }
    }

    public Library(string name, string address)
    {
        Name = name;
        Address = address;
        _books = new List<string>(); 
        Console.WriteLine($"Конструктор: Бібліотека '{Name}' створена за адресою '{Address}'.");
    }

    ~Library()
    {
        Console.WriteLine($"Деструктор: Бібліотека '{Name}' очищується. Кількість книг: {_books.Count}.");
    }

    public void AddBook(string bookTitle)
    {
        _books.Add(bookTitle);
        Console.WriteLine($"Книга '{bookTitle}' додана до бібліотеки '{Name}'.");
    }

    public void PrintLibraryInfo()
    {
        Console.WriteLine($"\n--- Інформація про бібліотеку ---");
        Console.WriteLine($"Назва: {Name}");
        Console.WriteLine($"Адреса: {Address}");
        Console.WriteLine($"Кількість книг: {BooksCount}");
        if (_books.Count > 0)
        {
            Console.WriteLine("Список книг:");
            foreach (var book in _books)
            {
                Console.WriteLine($"- {book}");
            }
        }
        else
        {
            Console.WriteLine("В бібліотеці немає книг.");
        }
        Console.WriteLine("---------------------------------");
    }
}
public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        Console.WriteLine("Створення об'єктів бібліотек:");
        Library cityLibrary = new Library("Міська Бібліотека #1", "Вул. Центральна, 10");
        Library universityLibrary = new Library("Бібліотека Університету", "Пл. Свободи, 5");
        Library onlineLibrary = new Library("Онлайн Бібліотека ReadAll", "Веб-сервер");

        Console.WriteLine("\nДодавання книг:");
        cityLibrary.AddBook("Хоббіт");
        cityLibrary.AddBook("Володар Перснів");
        cityLibrary.AddBook("1984");
        universityLibrary.AddBook("Програмування на C#");
        universityLibrary.AddBook("Алгоритми та структури даних");
        onlineLibrary.AddBook("Машина часу");
        cityLibrary.PrintLibraryInfo();
        universityLibrary.PrintLibraryInfo();
        onlineLibrary.PrintLibraryInfo();
        Console.WriteLine("\nЗавершення роботи програми. Деструктори будуть викликані GC пізніше.");
    }
}