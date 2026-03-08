using System;
using System.Collections.Generic;
using System.Linq;

namespace lab20
{
    // --- 1. МОДЕЛІ ДАНИХ ---
    public enum OrderStatus { New, PendingValidation, Processed, Shipped, Delivered, Cancelled }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order(int id, string customerName, decimal totalAmount)
        {
            Id = id;
            CustomerName = customerName;
            TotalAmount = totalAmount;
            Status = OrderStatus.New;
        }
    }

    // --- 2. ІНТЕРФЕЙСИ (ДЕКОМПОЗИЦІЯ SRP) ---
    public interface IOrderValidator
    {
        bool IsValid(Order order);
    }

    public interface IOrderRepository
    {
        void Save(Order order);
        Order GetById(int id);
    }

    public interface IEmailService
    {
        void SendOrderConfirmation(Order order);
    }

    // --- 3. РЕАЛІЗАЦІЯ СЕРВІСІВ (ЗАГЛУШКИ) ---
    public class SimpleOrderValidator : IOrderValidator
    {
        public bool IsValid(Order order) => order.TotalAmount > 0;
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = new List<Order>();
        public void Save(Order order) 
        {
            _orders.Add(order);
            Console.WriteLine($"[Repository] Замовлення #{order.Id} збережено в базу даних.");
        }
        public Order GetById(int id) => _orders.FirstOrDefault(o => o.Id == id);
    }

    public class ConsoleEmailService : IEmailService
    {
        public void SendOrderConfirmation(Order order) =>
            Console.WriteLine($"[Email] Надіслано підтвердження для {order.CustomerName}: Замовлення #{order.Id} прийнято.");
    }

    // --- 4. ORDER SERVICE (КООРДИНАТОР / DI) ---
    public class OrderService
    {
        private readonly IOrderValidator _validator;
        private readonly IOrderRepository _repository;
        private readonly IEmailService _emailService;

        // Dependency Injection через конструктор
        public OrderService(IOrderValidator validator, IOrderRepository repository, IEmailService emailService)
        {
            _validator = validator;
            _repository = repository;
            _emailService = emailService;
        }

        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"\n>>> Обробка замовлення #{order.Id} (Клієнт: {order.CustomerName})");

            if (!_validator.IsValid(order))
            {
                order.Status = OrderStatus.Cancelled;
                Console.WriteLine($"[Error] Замовлення #{order.Id} не валідне (Сума: {order.TotalAmount}). Скасовано.");
                return;
            }

            order.Status = OrderStatus.Processed;
            _repository.Save(order);
            _emailService.SendOrderConfirmation(order);
            
            Console.WriteLine($"[Success] Замовлення #{order.Id} успішно оброблено.");
        }
    }

    // --- 5. ТОЧКА ВХОДУ ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Ініціалізація компонентів
            var validator = new SimpleOrderValidator();
            var repository = new InMemoryOrderRepository();
            var emailService = new ConsoleEmailService();

            // Створення головного сервісу (DI)
            var orderService = new OrderService(validator, repository, emailService);

            // Сценарій 1: Валідне замовлення
            Order validOrder = new Order(101, "Олександр Бабаян", 1500.50m);
            orderService.ProcessOrder(validOrder);

            // Сценарій 2: Невалідне замовлення (від'ємна сума)
            Order invalidOrder = new Order(102, "Тестовий Користувач", -50.00m);
            orderService.ProcessOrder(invalidOrder);

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}
