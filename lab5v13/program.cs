using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab5ScheduleApp
{
    public class TimeOverlapException : Exception
    {
        public TimeOverlapException(string message) : base(message) { }
    }

    public interface IRepository<T>
    {
        void Add(T item);
        bool Remove(T item);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Func<T, bool> predicate);
    }

    public class Repository<T> : IRepository<T>
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T item) => _items.Add(item);
        public bool Remove(T item) => _items.Remove(item);
        public IEnumerable<T> GetAll() => _items.AsReadOnly();
        public IEnumerable<T> Find(Func<T, bool> predicate) => _items.Where(predicate);
    }

    public class Lesson
    {
        public string Subject { get; set; }
        public string Teacher { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public double DurationHours => (EndTime - StartTime).TotalHours;

        public override string ToString() => 
            $"{Day,-10} | {StartTime:hh\\:mm}-{EndTime:hh\\:mm} | {Subject} ({Teacher})";
    }

    public class Schedule
    {
        // Композиція: Розклад володіє репозиторієм занять
        private readonly IRepository<Lesson> _lessonRepo = new Repository<Lesson>();

        public void AddLesson(Lesson newLesson)
        {
            // Валідація: перевірка на перетин часу за допомогою LINQ
            var hasOverlap = _lessonRepo.GetAll().Any(l => 
                l.Day == newLesson.Day && 
                l.StartTime < newLesson.EndTime && 
                newLesson.StartTime < l.EndTime);

            if (hasOverlap)
                throw new TimeOverlapException($"УВАГА! Конфлікт розкладу: '{newLesson.Subject}' перетинається з іншим заняттям на {newLesson.Day}.");

            _lessonRepo.Add(newLesson);
        }

        // Обчислення за допомогою LINQ
        public double GetTotalWeeklyHours() => _lessonRepo.GetAll().Sum(l => l.DurationHours);

        public double GetTeacherLoad(string teacherName) => 
            _lessonRepo.Find(l => l.Teacher == teacherName).Sum(l => l.DurationHours);

        public IEnumerable<Lesson> GetAllLessons() => _lessonRepo.GetAll();
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Налаштування кодування для коректного виводу кирилиці в консоль
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Schedule mySchedule = new Schedule();

            Console.WriteLine("=== СИСТЕМА УПРАВЛІННЯ РОЗКЛАДОМ ===\n");

            try
            {
                // Додаємо успішні заняття
                mySchedule.AddLesson(new Lesson { 
                    Subject = "Об'єктно-орієнтоване програмування", 
                    Teacher = "Василенко О.М.", 
                    Day = DayOfWeek.Monday, 
                    StartTime = new TimeSpan(8, 30, 0), 
                    EndTime = new TimeSpan(10, 0, 0) 
                });

                mySchedule.AddLesson(new Lesson { 
                    Subject = "Вища математика", 
                    Teacher = "Петренко І.В.", 
                    Day = DayOfWeek.Monday, 
                    StartTime = new TimeSpan(10, 10, 0), 
                    EndTime = new TimeSpan(11, 40, 0) 
                });

                Console.WriteLine("✅ Початкові заняття додано успішно.");

                // ДЕМОНСТРАЦІЯ ПОМИЛКИ: Додаємо заняття, що перетинається за часом
                Console.WriteLine("\nСпроба додати конфліктне заняття (Фізика)...");
                mySchedule.AddLesson(new Lesson { 
                    Subject = "Фізика", 
                    Teacher = "Сидоренко П.С.", 
                    Day = DayOfWeek.Monday, 
                    StartTime = new TimeSpan(9, 30, 0), // Перетинається з першою парою
                    EndTime = new TimeSpan(11, 0, 0) 
                });

            }
            catch (TimeOverlapException ex)
            {
                Console.WriteLine($"❌ Помилка: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Сталася непередбачена помилка: {ex.Message}");
            }

            // ВИВІД РЕЗУЛЬТАТІВ (LINQ)
            Console.WriteLine("\n--- ПОТОЧНИЙ РОЗКЛАД ---");
            foreach (var item in mySchedule.GetAllLessons())
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("\n--- СТАТИСТИКА ---");
            double totalHours = mySchedule.GetTotalWeeklyHours();
            Console.WriteLine($"Загальна кількість годин на тиждень: {totalHours:F1} год.");

            string teacher = "Василенко О.М.";
            double teacherHours = mySchedule.GetTeacherLoad(teacher);
            double workload = (teacherHours / 40.0) * 100; // Розрахунок відносно 40-годинного тижня
            
            Console.WriteLine($"Навантаження викладача {teacher}: {teacherHours} год. ({workload:F1}% від норми)");
            
            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}
