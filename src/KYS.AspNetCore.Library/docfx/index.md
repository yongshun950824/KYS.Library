---
_layout: landing
---

# KYS.AspNetCore.Library Documentation

Welcome to **KYS.AspNetCore.Library**, a modular utility library built for internal ASP.NET (Core) applications.  
It provides reusable helpers, extensions, and middlewares to reduce repeated code and improve application consistency.

This site contains a high-level overview of the library and a full API reference automatically generated from XML documentation comments.

---

## 📦 Project Modules

### **KYS.AspNetCore.Library.Extensions**

Provides extension methods that enhance built-in .NET types, making everyday code more expressive and easier to read.

### **KYS.AspNetCore.Library.Helpers**

Includes helper classes focused on simplifying repetitive or boilerplate tasks such as parsing, conversions, or calculations.

### **KYS.AspNetCore.Library.Middlewares**

Contains reusable middlewares to be assembled into an application pipeline to handle HTTP requests and responses.

---

## 🚀 Using This Library (Internal Use)

Since this library is not currently published to NuGet, it can be consumed by:

1. Cloning or downloading the repository
2. Building the solution
3. Locating the generated DLLs under:
   - _KYS.AspNetCore.Library/bin/Debug/net8.0/_ or
   - _KYS.AspNetCore.Library/bin/Release/net8.0/_
4. Adding the DLL references to your project manually:

- In Visual Studio:  
  **Project → Add Reference → Browse → Select DLLs**
- In .NET CLI-based project:
  ```
  <ItemGroup>
    <Reference Include="KYS.AspNetCore.Library">
      <HintPath>path/to/KYS.AspNetCore.Library.dll</HintPath>
    </Reference>
  </ItemGroup>
  ```

---

## 📘 API Reference

Detailed class, method, and property documentation is available under the **API** section of this site.  
All content is generated automatically based on XML documentation comments in the source code.

---

## 📂 Project Structure

KYS.Library/ <br />
├── KYS.AspNetCore.Library.Extensions/ <br />
├── KYS.AspNetCore.Library.Helpers/ <br />
├── KYS.AspNetCore.Library.Middlewares/ <br />
├── KYS.AspNetCore.Library.Models/ <br />
└── KYS.AspNetCore.Library.TagHelpers/

_(Classes and namespaces inside each module can be viewed through the API reference.)_

---

## 🛠 Documentation

This documentation is generated using **DocFX** and will stay updated as new XML comments are added or modified.
