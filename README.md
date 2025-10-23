# 🧅 .NET Onion CQRS API Template

> A feature-rich, production-ready starter template for building .NET Web APIs. It's built on a foundation of **Clean (Onion) Architecture** and **CQRS** patterns to ensure a clean, maintainable, and scalable codebase right from the start.

This template isn't just an empty shell; it's a fully-configured boilerplate with essential services, security, and database management already built-in.

---

## ✨ Core Features

This template comes pre-configured with a powerful set of modern .NET tools:

* **Architecture:**
    * 🧅 **Onion Architecture** (Clean Architecture) for a clean, decoupled structure.
    * 🎯 **CQRS** (Command Query Responsibility Segregation) using **MediatR**.
* **Database:**
    * 💿 **Entity Framework Core** for data access.
    * 🚀 **DbUp** for **SQL-first** database migrations (perfect for handling tables, views, and stored procedures). Migrations run automatically on startup.
* **API & Security:**
    * 🔐 **Authentication** pre-configured for **JWT Bearer**, **Cookies**, and **Google**.
    * 🎨 **Swagger (OpenAPI)** configured with UI for testing auth (`Bearer` token) and to read `Annotations`.
    * ⛓️ **CORS** (Cross-Origin Resource Sharing) policy enabled.
* **Background Jobs:**
    * ⏳ **Hangfire** is fully set up with a dashboard (`/hangfire`) and SQL Server storage.
* **Best Practices:**
    * 🧩 **AutoMapper** for DTO-to-Entity mapping.
    * 🚦 **FluentValidation** (ready to be integrated with the MediatR pipeline).
    * 🛡️ **Global Error Handling** via custom middleware.
    * 📋 **Request Logging** via custom middleware.
    * 📦 **`dotnet new` Template** configuration included (`.template.config`).

---

## 🚀 Getting Started

This project is configured as a `dotnet new` template, allowing you to generate a new, clean solution from it in seconds.

### 1. Install the Template (One-Time Setup)

Install this template onto your local machine by pointing `dotnet new` to your GitHub repository:

```bash
dotnet new install https://github.com/abdelmonemanwr/dotnet-onion-cqrs-template
```

### 2. Install the Template (from Local Path)

Navigate into the cloned repository's root folder and run the dotnet new install command. This registers the template with your .NET SDK.

```bash
# Navigate into the folder you just cloned
cd dotnet-onion-cqrs-template

# Install the template from the current folder
dotnet new install .
```

### 3. Create Your New Project

Now that the template is installed locally, you can use it from any folder to generate your new project structure. The template automatically renames projects and namespaces.

```bash
# Go to your projects directory (e.g., D:\myProjects)
cd D:\myProjects

# "my-onion-api" is the 'shortName' from template.json.
# "MyAwesomeProject" is your new project name.
dotnet new my-onion-api -n MyAwesomeProject
```

### 4. Configure appsettings.json

Before you can run the project, you must update the appsettings.Development.json file inside the generated API project (MyAwesomeProject.Api) with your local settings.

```json
{
  "ConnectionStrings": {
    // 👇 Update this with your local SQL Server details
    "DefaultConnection": "Server=.;Database=MyAwesomeDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Issuer": "MyAwesomeApi",
    "Audience": "MyAwesomeApiClient",
    "Key": "REPLACE_THIS_WITH_A_VERY_LONG_AND_SECRET_KEY_123456" // 👈 CRITICAL
  },
  "GoogleSettings": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID_GOES_HERE",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET_GOES_HERE"
  }
}
```

### 5. Run the Project

That's it! Open the new solution (MyAwesomeProject.sln), set MyAwesomeProject.Api as the startup project, and hit Run (F5).

The application will start, automatically run database migrations (DbUp), and launch a browser opening the Swagger UI, ready for testing.
