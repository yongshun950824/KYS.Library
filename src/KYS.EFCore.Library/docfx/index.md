---
_layout: landing
---

# KYS.EFCore.Library Documentation

Welcome to **KYS.EFCore.Library**, a utility library for building data-access layers with **Entity Framework Core** in internal .NET applications.  
It provides Domain-Driven Design (DDD) building blocks, an audit-aware `DbContext`, an explicit Unit of Work, and the Specification pattern to reduce repeated persistence code and keep domain models consistent.

This site contains a high-level overview of the library and a full API reference automatically generated from XML documentation comments.

---

## 📦 Project Modules

### **KYS.EFCore.Library**

Core DDD building blocks:

- <xref:KYS.EFCore.Library.Entity`1>: base entity with identity-based equality and domain events
- <xref:KYS.EFCore.Library.ValueObject>: base value object compared by its components
- <xref:KYS.EFCore.Library.Enumeration>: "smart enum" base class
- <xref:KYS.EFCore.Library.Specification`1>: composable query criteria (`And`/`Or`)
- <xref:KYS.EFCore.Library.IUnitOfWork> / <xref:KYS.EFCore.Library.DbContextUnitOfWork`1>: explicit transaction handling
- `IRepository<T>`, `IAggregateRoot`, `INotification`: repository and domain event contracts

### **KYS.EFCore.Library.DBContext**

Provides <xref:KYS.EFCore.Library.DBContext.ApplicationDbContext>, a base `DbContext` that records inserts, updates and deletes into an `ActionLog` table through `SaveChangesWithAuditAsync`. Table and column naming adapts automatically to PostgreSQL (snake_case) or other providers.

### **KYS.EFCore.Library.Converters**

Contains EF Core value converters, such as storing an `enum` by its `[Description]` value instead of its numeric value.

### **KYS.EFCore.Library.Extensions**

Extension methods for registering the Unit of Work in dependency injection and for querying a `DbContext` with a `Specification<T>`.

---

## ⚡ Quick Start

```csharp
// 1. Derive your DbContext from ApplicationDbContext
public class AppDbContext : ApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // keeps the ActionLog mapping
        // your mappings...
    }
}

// 2. Register services
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddExplicitUnitOfWork<AppDbContext>();

// 3. Query with a specification
var orders = await context.ApplySpecification(new ActiveOrdersSpecification()).ToListAsync();

// 4. Save with audit logging
await context.SaveChangesWithAuditAsync(actionBy: userId, formatting: Formatting.SnakeCase);
```

---

## 🚀 Using This Library (Internal Use)

Since this library is not currently published to NuGet, it can be consumed by:

1. Cloning or downloading the repository
2. Building the solution
3. Locating the generated DLLs under:
   - _KYS.EFCore.Library/bin/Debug/net8.0/_ or
   - _KYS.EFCore.Library/bin/Release/net8.0/_
4. Adding the DLL references to your project manually:

- In Visual Studio:  
  **Project → Add Reference → Browse → Select DLLs**
- In .NET CLI-based project:
  ```
  <ItemGroup>
    <Reference Include="KYS.EFCore.Library">
      <HintPath>path/to/KYS.EFCore.Library.dll</HintPath>
    </Reference>
    <Reference Include="KYS.Library">
      <HintPath>path/to/KYS.Library.dll</HintPath>
    </Reference>
  </ItemGroup>
  ```

> **Note:** `KYS.EFCore.Library` depends on `KYS.Library`, so both DLLs must be referenced.  
> Manually referenced DLLs do not bring in their NuGet dependencies, so your project must also install:
> `Microsoft.EntityFrameworkCore` (9.x), `Microsoft.EntityFrameworkCore.Relational` (9.x), `CSharpFunctionalExtensions` and `Newtonsoft.Json`, plus your database provider (e.g. `Npgsql.EntityFrameworkCore.PostgreSQL`).

---

## 📘 API Reference

Detailed class, method, and property documentation is available under the **API** section of this site.  
All content is generated automatically based on XML documentation comments in the source code.

---

## 📂 Project Structure

KYS.EFCore.Library/ <br />
├── Converters/ <br />
├── DBContext/ <br />
│&nbsp;&nbsp;&nbsp;└── Partials/ <br />
├── Extensions/ <br />
├── Entity.cs, ValueObject.cs, Enumeration.cs <br />
├── Specification.cs <br />
└── IUnitOfWork.cs, DbContextUnitOfWork.cs, IRepository.cs

_(Classes and namespaces inside each module can be viewed through the API reference.)_

---

## 🛠 Documentation

This documentation is generated using **DocFX** and will stay updated as new XML comments are added or modified.
