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
