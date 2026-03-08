using System;

namespace lab22
{
    // ==========================================
    // 1. ПОЧАТКОВА ІЄРАРХІЯ (ПОРУШЕННЯ LSP)
    // ==========================================
    
    public class Rectangle
    {
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public int GetArea() => Width * Height;
    }

    public class Square : Rectangle
    {
        // Порушення: Квадрат змушений змінювати обидві сторони, 
        // щоб підтримувати свою цілісність, але це ламає контракт прямокутника.
        public override int Width
        {
            get => base.Width;
            set { base.Width = value; base.Height = value; }
        }

        public override int Height
        {
            get => base.Height;
            set { base.Width = value; base.Height = value; }
        }
    }

    // ==========================================
    // 2. РЕФАКТОРИНГ (ДОТРИМАННЯ LSP - Зміна ієрархії)
    // ==========================================
    
    // Виділяємо спільну абстракцію, яка не гарантує незалежність сторін
    public abstract class Shape
    {
        public abstract int GetArea();
    }

    public class LspRectangle : Shape
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public override int GetArea() => Width * Height;
    }

    public class LspSquare : Shape
    {
        public int Side { get; set; }
        public override int GetArea() => Side * Side;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // --- Демонстрація проблеми (Bad Example) ---
            Console.WriteLine("=== Порушення LSP ===");
            Rectangle rect = new Square(); // Підстановка Квадрата замість Прямокутника
            TestRectangleArea(rect);

            // --- Демонстрація рішення (Good Example) ---
            Console.WriteLine("\n=== Дотримання LSP (Рефакторинг) ===");
            Shape lspRect = new LspRectangle { Width = 5, Height = 10 };
            Shape lspSquare = new LspSquare { Side = 5 };

            Console.WriteLine($"Площа прямокутника: {lspRect.GetArea()}"); // 50
            Console.WriteLine($"Площа квадрата: {lspSquare.GetArea()}");   // 25
        }

        // Клієнтський метод, що очікує вигляд Прямокутника
        static void TestRectangleArea(Rectangle r)
        {
            r.Width = 5;
            r.Height = 10;

            // Клієнт очікує, що площа буде 5 * 10 = 50.
            // Але якщо r — це Square, площа буде 10 * 10 = 100!
            Console.WriteLine($"Очікувана площа для 5x10: 50");
            Console.WriteLine($"Фактична площа: {r.GetArea()}");

            if (r.GetArea() != 50)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ПОМИЛКА: Поведінка об'єкта не відповідає вигляду Прямокутника!");
                Console.ResetColor();
            }
        }
    }
}