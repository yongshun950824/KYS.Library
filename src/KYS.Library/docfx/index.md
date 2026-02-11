---
_layout: landing
---

# KYS.Library Documentation

Welcome to **KYS.Library**, a modular utility library built for internal .NET applications.  
It provides reusable helpers, extensions, services, and validation tools to reduce repeated code and improve application consistency.

This site contains a high-level overview of the library and a full API reference automatically generated from XML documentation comments.

---

## 📦 Project Modules

### **KYS.Library.Extensions**

Provides extension methods that enhance built-in .NET types, making everyday code more expressive and easier to read.

### **KYS.Library.Helpers**

Includes helper classes focused on simplifying repetitive or boilerplate tasks such as parsing, conversions, or calculations.

### **KYS.Library.Services**

Contains reusable service-style components that encapsulate common operations or shared logic used across applications.

### **KYS.Library.Validations**

Offers a collection of validation utilities used to enforce consistent input checks and data validation rules.

---

## 🚀 Using This Library (Internal Use)

Since this library is not currently published to NuGet, it can be consumed by:

1. Cloning or downloading the repository
2. Building the solution
3. Locating the generated DLLs under:
   - _KYS.Library/bin/Debug/net8.0/_ or
   - _KYS.Library/bin/Release/net8.0/_
4. Adding the DLL references to your project manually:

- In Visual Studio:  
  **Project → Add Reference → Browse → Select DLLs**
- In .NET CLI-based project:
  ```
  <ItemGroup>
    <Reference Include="KYS.Library">
      <HintPath>path/to/KYS.Library.dll</HintPath>
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
├── KYS.Library.Services/ <br />
├── KYS.Library.Extensions/ <br />
├── KYS.Library.Helpers/ <br />
└── KYS.Library.Validations/

_(Classes and namespaces inside each module can be viewed through the API reference.)_

---

## 🛠 Documentation

This documentation is generated using **DocFX** and will stay updated as new XML comments are added or modified.