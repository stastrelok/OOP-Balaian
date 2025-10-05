using System;
using System.Collections.Generic; // Для використання List<T>
using System.Linq; // Для використання LINQ-запитів, таких як GroupBy, Average, OrderByDescending

// Базовий клас: Movie - представляє загальний фільм
public class Movie
{
    // Приватні поля для зберігання даних про фільм
    private string _title;
    private int _releaseYear;
    private double _rating; // Рейтинг фільму (наприклад, від 1.0 до 10.0)

    // Публічні властивості (properties) з get/set аксесорами
    // Використовуються для контрольованого доступу до приватних полів
    public string Title
    {
        get { return _title; }
        set
        {
            // Валідація: назва фільму не може бути пустою
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Назва фільму не може бути пустою.");
            _title = value;
        }
    }

    public int ReleaseYear
    {
        get { return _releaseYear; }
        set
        {
            // Валідація: рік випуску в розумних межах
            if (value < 1888 || value > DateTime.Now.Year + 5)
                throw new ArgumentOutOfRangeException("Рік випуску недійсний.");
            _releaseYear = value;
        }
    }

    public double Rating
    {
        get { return _rating; }
        set
        {
            // Валідація: рейтинг має бути в межах від 1.0 до 10.0
            if (value < 1.0 || value > 10.0)
                throw new ArgumentOutOfRangeException("Рейтинг має бути від 1.0 до 10.0.");
            _rating = value;
        }
    }

    // Конструктор базового класу Movie
    // Викликається при створенні нового об'єкта Movie або похідних класів
    public Movie(string title, int releaseYear, double rating)
    {
        Title = title;          // Присвоєння значень через властивості для використання валідації
        ReleaseYear = releaseYear;
        Rating = rating;
        Console.WriteLine($"Базовий конструктор Movie викликано для '{Title}'.");
    }

    // Віртуальний метод для виведення інформації про фільм
    // 'virtual' дозволяє похідним класам перевизначати цей метод ('override')
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Фільм: {Title} ({ReleaseYear}), Рейтинг: {Rating:F1}");
    }

    // Віртуальний метод для отримання жанру фільму
    // Також 'virtual' для перевизначення у похідних класах
    public virtual string GetGenre()
    {
        return "Загальний"; // Жанр за замовчуванням для базового класу
    }
}

// Похідний клас 1: ActionMovie - представляє бойовик
public class ActionMovie : Movie // Успадковує всі члени від класу Movie
{
    // Додаткова властивість, специфічна для бойовика
    public int StuntCount { get; set; }

    // Конструктор ActionMovie
    // ': base(title, releaseYear, rating)' викликає конструктор базового класу Movie
    public ActionMovie(string title, int releaseYear, double rating, int stuntCount)
        : base(title, releaseYear, rating)
    {
        StuntCount = stuntCount; // Ініціалізація власної властивості
        Console.WriteLine($"Конструктор ActionMovie викликано для '{Title}'.");
    }

    // Перевизначення методу DisplayInfo для ActionMovie
    // 'override' вказує, що цей метод замінює віртуальний метод з базового класу
    public override void DisplayInfo()
    {
        base.DisplayInfo(); // Виклик базової реалізації DisplayInfo з класу Movie
        Console.WriteLine($"  Жанр: Бойовик, Кількість трюків: {StuntCount}");
    }

    // Перевизначення методу GetGenre для ActionMovie
    public override string GetGenre()
    {
        return "Бойовик"; // Повертає конкретний жанр
    }
}

// Похідний клас 2: ComedyMovie - представляє комедію
public class ComedyMovie : Movie // Успадковує всі члени від класу Movie
{
    // Додаткова властивість, специфічна для комедії
    public int LaughCount { get; set; }

    // Конструктор ComedyMovie
    // ': base(title, releaseYear, rating)' викликає конструктор базового класу Movie
    public ComedyMovie(string title, int releaseYear, double rating, int laughCount)
        : base(title, releaseYear, rating)
    {
        LaughCount = laughCount; // Ініціалізація власної властивості
        Console.WriteLine($"Конструктор ComedyMovie викликано для '{Title}'.");
    }

    // Перевизначення методу DisplayInfo для ComedyMovie
    // 'override' вказує, що цей метод замінює віртуальний метод з базового класу
    public override void DisplayInfo()
    {
        base.DisplayInfo(); // Виклик базової реалізації DisplayInfo з класу Movie
        Console.WriteLine($"  Жанр: Комедія, Кількість жартів: {LaughCount}");
    }

    // Перевизначення методу GetGenre для ComedyMovie
    public override string GetGenre()
    {
        return "Комедія"; // Повертає конкретний жанр
    }
}

// Головний клас програми
public class Program
{
    public static void Main(string[] args)
    {
        // Встановлення кодування для коректного відображення символів (наприклад, валюти, якщо використовується)
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Створення колекції об'єктів базового типу (Movie)
        // Це дозволяє зберігати об'єкти як ActionMovie, так і ComedyMovie разом,
        // демонструючи поліморфізм
        List<Movie> movies = new List<Movie>();

        Console.WriteLine("--- Створення об'єктів фільмів ---");
        // Додаємо об'єкти похідних класів до колекції базового типу
        movies.Add(new ActionMovie("Матриця", 1999, 9.5, 250));
        movies.Add(new ComedyMovie("Один вдома", 1990, 8.2, 120));
        movies.Add(new ActionMovie("Термінатор 2", 1991, 9.0, 300));
        movies.Add(new ComedyMovie("Аероплан!", 1980, 8.0, 150));
        movies.Add(new ActionMovie("Джон Уік", 2014, 8.5, 400));
        movies.Add(new ComedyMovie("Зомбіленд", 2009, 7.6, 90));
        Console.WriteLine("----------------------------------\n");

        Console.WriteLine("--- Демонстрація поліморфізму (виклик DisplayInfo для кожного об'єкта) ---");
        // При ітерації по колекції Movie, викликається коректна (перевизначена)
        // версія методу DisplayInfo для кожного типу об'єкта (ActionMovie або ComedyMovie)
        foreach (var movie in movies)
        {
            movie.DisplayInfo();
        }
        Console.WriteLine("------------------------------------------------------------------\n");

        Console.WriteLine("--- Середній рейтинг за жанрами ---");
        var averageRatingsByGenre = movies
            .GroupBy(movie => movie.GetGenre()) // Групуємо фільми за їхнім жанром (поліморфний виклик GetGenre)
            .Select(group => new // Створюємо анонімний об'єкт для результату
            {
                Genre = group.Key,
                AverageRating = group.Average(movie => movie.Rating) // Обчислюємо середній рейтинг для кожної групи
            });

        foreach (var item in averageRatingsByGenre)
        {
            Console.WriteLine($"Жанр: {item.Genre}, Середній рейтинг: {item.AverageRating:F2}");
        }
        Console.WriteLine("----------------------------------\n");

        // Обчислення: топ-1 фільм (фільм з найвищим рейтингом)
        Console.WriteLine("--- Топ-1 фільм за рейтингом ---");
        // OrderByDescending сортує за рейтингом спадання, FirstOrDefault бере перший елемент
        Movie topMovie = movies.OrderByDescending(m => m.Rating).FirstOrDefault();
        if (topMovie != null)
        {
            Console.WriteLine("Найкращий фільм:");
            topMovie.DisplayInfo(); // Викликаємо DisplayInfo для знайденого топ-фільму
        }
        else
        {
            Console.WriteLine("Немає фільмів у колекції.");
        }
        Console.WriteLine("----------------------------------\n");

        Console.WriteLine("Програма завершила роботу.");
    }
}