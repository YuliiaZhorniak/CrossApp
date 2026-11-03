# CrossApp

Наскрізний проєкт з крос-платформного програмування.

Предметна область: Замовлення. Сутності: Customer, Product, Order, OrderLine.  
Призначення: оформлення замовлень і підрахунок сум.

## Структура solution

```
CrossApp/
 ├── CrossApp.sln
 ├── README.md
 ├── .gitignore
 └── src/
      ├── Core/
      │    ├── Core.csproj
      │    └── EnvironmentInfo.cs
      └── Cli/
           ├── Cli.csproj
           └── Program.cs
```

## Запуск
* dotnet build src/Core/Core.csproj
* dotnet publish src/Cli -c Release -r win-x64 --self-contained true
* dotnet publish src/Cli -c Release -r win-x64 --self-contained false

## Таблиця
| RID | Режим | Розмір publish | runtime |
| --- | --- | --- | --- |
| win-x64 | self-contained | 76,7 MB | ні |
| win-x64 | framework-dependent | 0,2 MB | так (.NET 10) |

## Середовище
.NET SDK 10.0, Windows 11 x64

## Порівняння розмірів
win-x64    - 76,66 MB
linux-x64  - 78,79 MB

## Інваріанти
* `Order.Create`: Ідентифікатор замовлення та клієнта є обов'язковими (не порожні) -> `ArgumentException`.
* `OrderLine`: Ціна товару не може бути від'ємною, а кількість має бути > 0 -> `ArgumentOutOfRangeException`.
* `Order.AddLine`: Заборонено додавати товари до підтвердженого замовлення -> `InvalidOperationException`.
* `Order.Confirm`: Заборонено підтверджувати порожнє замовлення (без товарів) -> `InvalidOperationException`.
* `Order.Confirm`: Заборонено повторно підтверджувати вже підтверджене замовлення -> `InvalidOperationException`.

## Лабораторна робота 5
- **Інтерфейс сховища:** `IOrderStore`
- **Реалізації сховища:** 
  - `InMemoryOrderStore` 
  - `FileOrderStore`
- **Сервіс:** `OrderService` 
- **Composition Root / Ручний DI:** `Program.cs`

## Лабораторна робота 7
Запит у коді виконується саме тоді, коли викликається метод, наприклад, query.Count() або query.ToList().
А два виклики Count дали різні числа, бо перший виклик порахував кількість замовлень до зміни списку, другий - порахував кількість вже після додавання нового замовлення.

## Лабораторна робота 8
- **dotnet test** - запустити тести
- **Ctrl + C** - при запуску імпорту натиснути цю комбінацію

## Лабораторна робота 9
# Профілі застосунку
Застосунок підтримує два основних середовища виконання:
1. **Development:**
   - Використовує файл конфігурації `appsettings.Development.json`.
   - Встановлює детальніший рівень логування (`Debug`).
   - Застосовує локальні `user-secrets` для зберігання конфіденційних ключів.
2. **Production:**
   - Використовує базовий файл `appsettings.json`.
   - Рівень логування за замовчуванням: `Information`.
   - Конфіденційні дані передаються виключно через змінні середовища.

   ## Таблиця ключів конфігурації

| Шлях ключа у JSON | Тип | За замовчуванням | Опис |
| :--- | :--- | :--- | :--- |
| `EnvironmentName` | `string` | `Development` | Активне середовище застосунку |
| `Logging:MinimumLevel` | `string` | `Information` | Мінімальний рівень логування |
| `Storage:DataPath` | `string` | `data` | Відносний або абсолютний шлях до каталогу даних |
| `Storage:FileName` | `string` | `items.json` | Назва файлу сховища даних |
| `Orders:Currency` | `string` | `UAH` | Валюта замовлень для доменної моделі |
| `Integration:ApiKey` | `string` | *зберігається у secrets* | Секрет інтеграції (читається з `user-secrets` або ENV) |


## Приклади змінних
### PowerShell (Windows):
```powershell
# Зміна активного профілю
\$env:CROSSAPP_ENVIRONMENT = "Production"

# Перевизначення вкладеного ключа через загальний механізм (подвійне підкреслення)
\$env:CROSSAPP_Storage__DataPath = "C:\temp\custom_data"

# Перевизначення через окрему спеціальну змінну
\$env:CROSSAPP_DATA_PATH = "C:\temp\custom_data"

# Перевизначення секрету через середовище
\$env:CROSSAPP_Integration__ApiKey = "env-secret-key"

# Запуск проєкту
dotnet run --project src/Cli

Для Bash
# Зміна активного профілю
export CROSSAPP_ENVIRONMENT="Production"

# Перевизначення вкладеного ключа через подвійне підкреслення
export CROSSAPP_Storage__DataPath="/tmp/custom_data"

# Перевизначення через окрему спеціальну змінну
export CROSSAPP_DATA_PATH="/tmp/custom_data"

# Перевизначення секрету через середовище
export CROSSAPP_Integration__ApiKey="env-secret-key"

# Запуск проєкту
dotnet run --project src/Cli