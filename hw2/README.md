Анти-патерн "God Object" та принцип SRP
У світі об'єктно-орієнтованого проектування якість коду часто визначається тим, наскільки легко його підтримувати та розширювати. Однією з найбільших перешкод на цьому шляху є створення так званих "Божественних об'єктів" (God Objects).

1. Характеристики "God Object"
God Object — це об'єкт, який "знає занадто багато" або "робить занадто багато". Він концентрує в собі логіку всієї системи або значної її частини.

Основні ознаки:

Гігантський розмір: Клас містить сотні або тисячі рядків коду.

Надмірна кількість методів та полів: Він намагається керувати даними, обчисленнями, вводом/виводом та логікою одночасно.

Низька зв'язність (Low Cohesion): Методи всередині класу мало пов'язані між собою за змістом.

Висока залежність: Більшість інших класів системи залежать від цього "Бога", що робить систему жорсткою — зміна в одному місці ламає все.

2. Приклад класу, що порушує SRP
Принцип єдиної відповідальності (Single Responsibility Principle) стверджує: Клас повинен мати лише одну причину для зміни.

Розглянемо клас OrderProcessor, який намагається бути "майстром на всі руки":


public class OrderProcessor
{
    public void ProcessOrder(Order order)
    {
        // 1. Валідація замовлення
        if (order.Items.Count == 0) throw new Exception("Замовлення порожнє");

        // 2. Розрахунок вартості
        double total = 0;
        foreach (var item in order.Items) total += item.Price;

        // 3. Збереження в базу даних
        File.AppendAllText("orders.txt", $"Order {order.Id}: {total}");

        // 4. Відправка email-підтвердження
        Console.WriteLine($"Відправка Email для замовлення {order.Id}...");
    }
}

Чому це порушує SRP?
Цей клас має мінімум чотири причини для зміни:

Змінилися правила валідації.

Змінилася логіка розрахунку ціни (наприклад, додали податки).

Ми вирішили використовувати SQL базу даних замість текстового файлу.

Змінився спосіб сповіщення (наприклад, хочемо SMS замість Email).

3. Рефакторинг для дотримання SRP
Для виправлення ми маємо розділити відповідальності на окремі спеціалізовані класи.

Крок 1: Створюємо окремі сервіси


// Відповідає тільки за збереження
public class OrderRepository
{
    public void Save(Order order, double total) => 
        File.AppendAllText("orders.txt", $"Order {order.Id}: {total}");
}

// Відповідає тільки за сповіщення
public class NotificationService
{
    public void SendEmail(Order order) => 
        Console.WriteLine($"Email відправлено для {order.Id}");
}

// Відповідає тільки за розрахунки
public class PricingCalculator
{
    public double CalculateTotal(Order order) => order.Items.Sum(i => i.Price);
}
Крок 2: Оновлений OrderProcessor (Оркестратор)
Тепер головний клас лише координує роботу інших, не знаючи деталей їхньої реалізації.


public class OrderProcessor
{
    private readonly OrderRepository _repo = new();
    private readonly NotificationService _notifications = new();
    private readonly PricingCalculator _calculator = new();

    public void ProcessOrder(Order order)
    {
        if (order.Items.Count == 0) return;

        double total = _calculator.CalculateTotal(order);
        _repo.Save(order, total);
        _notifications.SendEmail(order);
    }
}
Висновок
Рефакторинг дозволив нам перетворити заплутаний "Божественний об'єкт" на набір малих, тестованих та зрозумілих компонентів. Тепер кожен клас відповідає за свою вузьку ділянку роботи, що є ключем до створення надійного ПЗ.