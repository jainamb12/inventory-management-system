# SmartInventory: Enterprise SaaS Platform

![.NET Core](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap_5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)

SmartInventory is a robust, cloud-ready, web-based inventory operations center. It transitions businesses away from error-prone spreadsheets into a secure, automated, and relational database environment. 

This project was built to demonstrate full-stack enterprise architecture, including Role-Based Access Control (RBAC), automated business logic, and real-time data analytics.

## ✨ Key Features

* **Automated Business Logic (Smart Ledger):** Inventory quantities are never manually overwritten. Users record "IN" or "OUT" transactions in an immutable Audit Ledger, which automatically calculates and updates the primary product stock in real-time. Includes safeguards against negative inventory.
* **Role-Based Access Control (RBAC):** Secured by ASP.NET Core Identity. The application routes unauthenticated users to a custom login portal. Only users assigned the specific `InventoryManager` role can access the dashboard and manipulate data.
* **Real-Time Analytics Dashboard:** The home screen acts as a command center, calculating live metrics directly from the SQL database (Total Value, Low Stock Alerts, Total Entities) without degrading performance.
* **Relational Data Architecture:** Products are strictly bound to Categories and Suppliers using Foreign Key constraints, ensuring 100% data integrity and preventing orphaned records.
* **Enterprise UX/UI:** Features multi-column dynamic search (filtering by Name, Category, or Supplier), server-side pagination, centered authentication cards, and TempData success notifications.

## 🛠️ Technology Stack

* **Backend:** C#, ASP.NET Core 8 MVC
* **Database:** Microsoft SQL Server
* **ORM:** Entity Framework Core (Code-First Migrations)
* **Security:** ASP.NET Core Identity
* **Frontend:** HTML5, CSS3, Razor Views, Bootstrap 5

## 📸 Application Previews

*(Replace these links with actual screenshots of your running application once uploaded to GitHub)*
* [Screenshot 1: The Live Analytics Dashboard]
* [Screenshot 2: The Inventory Data Table with Search/Pagination]
* [Screenshot 3: The Immutable Audit Ledger]

## 🚀 Getting Started (Local Setup)

Follow these steps to run the application on your local machine.

### Prerequisites
* Visual Studio 2022 (or later)
* .NET 8 SDK
* SQL Server Management Studio (SSMS)

### Installation

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/yourusername/SmartInventory.git](https://github.com/yourusername/SmartInventory.git)
   ```

2. **Configure the Database Connection**:
Open <mark>appsettings.json</mark> and ensure the <mark>DefaultConnection</mark> string points to your local SQL Server instance
(e.g., <mark>Server=localhost;Database=SmartInventoryDB;Trusted_Connection=True;MultipleActiveResultSets=true</mark>).

3. **Apply Database Migrations:**
Open the Package Manager Console in Visual Studio and run:
```bash
Update-Database
```
4. **Run the Application:**
Hit the <mark>Play</mark> button in Visual Studio (or run <mark>dotnet run</mark> in the CLI).

Note: On the very first startup, the application's DbSeeder script will automatically inject 50 realistic products, categories, and suppliers into your SQL database for testing purposes.

### 🔐 Default Testing Credentials
Upon startup, the application's programmatic seeder will look for a specific email to grant the InventoryManager role. Register an account with the following email to gain full Admin access:

Manager Email: your_email@test.com (Replace this with the email you hardcoded in Program.cs)

Password: (Create any password meeting the system requirements during registration)

### 🧠 Core Architecture Highlights
The Stock Ledger Automation
The system utilizes a transactional approach to inventory management. Instead of simple UPDATE queries, stock movements are handled via the StockTransactionsController.

C#
// Example Logic Flow:
1. User submits an "OUT" transaction for 2 Laptops.
2. System validates the ProductId and ensures CurrentStock >= 2.
3. System logs the Date, UserEmail, TransactionType, and Quantity to the Audit Ledger.
4. System subtracts 2 from the Product's main inventory.
5. EF Core commits BOTH changes simultaneously to SQL Server.
