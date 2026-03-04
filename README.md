# EduPortal - Web Application

![.NET](https://img.shields.io/badge/.NET-8.0%2B-blue)
![Architecture](https://img.shields.io/badge/Architecture-MVC%20%7C%20Repository-orange)
![Testing](https://img.shields.io/badge/Test_Coverage-Growing-brightgreen)

## Overview
**EduPortal** is a custom-built educational management web application. I developed this project from scratch to showcase my full-stack development skills, with a strong emphasis on clean code, scalable architecture, and software quality. 

The application is built using the **ASP.NET Core MVC** framework and implements the **Repository Design Pattern** to ensure a decoupled and highly testable codebase.

## Architecture & Design Patterns
* **MVC (Model-View-Controller):** Ensures a clear separation of concerns between the user interface, routing logic, and underlying data models.
* **Repository Pattern:** Abstracts the data access layer. This keeps the controllers clean and makes the business logic incredibly easy to unit test by allowing database operations to be mocked.
* **Entity Framework Core (EF Core):** Used as the primary ORM for robust and secure database interactions, migrations, and LINQ-based querying.

## Key Features
* User authentication and secure access management.
* Intuitive dashboard for managing educational entities.
* Optimized data access via custom Repositories.

## Technology Stack
**Backend & Data Access:**
* C# / .NET (ASP.NET Core MVC)
* Entity Framework Core
* Repository Pattern
* MSql

**Frontend:**
* HTML5, CSS3, Razor Views (`.cshtml`)
* Bootstrap

**Quality Assurance & Testing:**
* NUnit & NSubstitute

---

## Quality Assurance & Test Strategy
Because the application leverages the Repository Pattern, the business logic is decoupled from the database, making it an ideal candidate for the **Test Automation Pyramid**.

### Phase 1: Unit Testing (Current State)
* Fast, isolated unit tests verifying the core business logic and controllers.
* Data access layers (Repositories) are mocked using [Moq/NSubstitute] to ensure tests run reliably without requiring a live database connection.

### Phase 2: Integration Testing (Planned)
* Testing the actual Entity Framework Repositories against a test database (e.g., using In-Memory DB or Testcontainers).
* Validating the complete request-response lifecycle.

### Phase 3: UI / End-to-End Testing (Planned)
* Automated browser testing for critical user journeys (e.g., Login, Form submissions).
* Planned stack: Selenium WebDriver + Reqnroll (BDD framework) executing in Headless mode.

---

## 🚀 Getting Started

### Prerequisites
* [.NET SDK](https://dotnet.microsoft.com/download) (v8.0 or later)
* [Ide írd be, ha kell adatbázis szerver, pl. SQL Server LocalDB]

### Installation & Run
1. Clone the repository:
   ```bash
   git clone [https://github.com/yourusername/EduPortal.git](https://github.com/yourusername/EduPortal.git)
   cd EduPortal
