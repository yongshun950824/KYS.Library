---
_layout: landing
---

# KYS.Library.Pdf Documentation

Welcome to **KYS.Library.Pdf**, an add-on to **KYS.Library** for working with PDF documents in internal .NET applications.  
It converts HTML (with custom fonts) to PDF, exports a `DataTable` to a PDF table, and checks, locks, or unlocks password-protected PDF files.

This site contains a high-level overview of the library and a full API reference automatically generated from XML documentation comments.

---

## 📦 Project Modules

### **KYS.Library.Pdf.Helpers**

Includes the helper classes for PDF generation and manipulation:

- `PdfHelper`
  - `ConvertHtmlToPdf` – renders HTML to a PDF `MemoryStream`, with options to register standard, shipped, system, and custom (`.ttf`) font families.
  - `IsPasswordProtected` – checks whether a PDF (as `byte[]` or `PdfReader`) requires a password.
  - `LockPdfDocument` / `UnlockPdfDocument` – encrypts a PDF with owner/user passwords (AES-128) or decrypts it with the owner password.
- `PdfDataTableHelper`
  - `WriteToPdfFile` – writes a `DataTable` into a PDF table with optional header and body styles, returning a `Result<byte[]>`.

---

## ⚡ Quick Example

```csharp
using KYS.Library.Pdf.Helpers;

// HTML to PDF using only a custom font family
using MemoryStream pdf = PdfHelper.ConvertHtmlToPdf(
    "<h1 style=\"font-family: Montserrat\">Hello</h1>",
    registerStandardPdfFonts: false,
    registerShippedFonts: false,
    registerSystemFonts: false,
    importCustomFontFamilyFilePaths: new List<string> { "fonts/Montserrat-Regular.ttf" });

// Protect a PDF with a password
byte[] locked = PdfHelper.LockPdfDocument(pdf.ToArray(), ownerPass: "owner", userPass: "user");

// DataTable to PDF
Result<byte[]> table = DataTableHelper.WriteToPdfFile(dataTable);
```

---

## ⚠️ Third-party License

This library is built on **iText 7** (`itext7`, `itext7.pdfhtml`), which is licensed under **AGPL**. A commercial license is available from iText for closed-source use.

---

## 🚀 Using This Library (Internal Use)

Since this library is not currently published to NuGet, it can be consumed by:

1. Cloning or downloading the repository
2. Building the solution
3. Locating the generated DLLs under:
   - _KYS.Library.Pdf/bin/Debug/net8.0/_ or
   - _KYS.Library.Pdf/bin/Release/net8.0/_
4. Adding the DLL references to your project manually (**KYS.Library.Pdf** depends on **KYS.Library**, so reference both):

- In Visual Studio:  
  **Project → Add Reference → Browse → Select DLLs**
- In .NET CLI-based project:
  ```
  <ItemGroup>
    <Reference Include="KYS.Library">
      <HintPath>path/to/KYS.Library.dll</HintPath>
    </Reference>
    <Reference Include="KYS.Library.Pdf">
      <HintPath>path/to/KYS.Library.Pdf.dll</HintPath>
    </Reference>
  </ItemGroup>
  ```

5. Adding the third-party NuGet packages used by the library: `itext7`, `itext7.pdfhtml`, and `CSharpFunctionalExtensions`.

---

## 📘 API Reference

Detailed class, method, and property documentation is available under the **API** section of this site.  
All content is generated automatically based on XML documentation comments in the source code.

---

## 📂 Project Structure

KYS.Library.Pdf/ <br />
└── Helpers/

_(Classes and namespaces inside each module can be viewed through the API reference.)_

---

## 🛠 Documentation

This documentation is generated using **DocFX** and will stay updated as new XML comments are added or modified.
