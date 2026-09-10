# CrossApp

Наскрізний проєкт з крос-платформного програмування.

Предметна область: Замовлення. Сутності: Customer, Product, Order, OrderLine.  
Призначення: оформлення замовлень і підрахунок сум.

## Структура solution

CrossApp/
│  CrossApp.sln
│  README.md
│  .gitignore
│
└──src/
   ├── Core/
   │     Core.csproj
   │     EnvironmentInfo.cs
   │
   └── Cli/
         Cli.csproj
         Program.cs

## Запуск
* dotnet build src/Core/Core.csproj
* dotnet publish src/Cli -c Release -r win-x64 --self-contained true
* dotnet publish src/Cli -c Release -r win-x64 --self-contained false

## Таблиця
RID      | Режим               | Розмір publish  |runtime    
win-x64   self-contained       76,7 MB           ні
win-x64   framework-dependent  0,2               так (.NET 10)


## Середовище
.NET SDK 10.0, Windows 11 x64

## Порівняння розмірів
win-x64    - 76,66 MB
linux-x64  - 78,79 MB