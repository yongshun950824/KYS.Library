---
_layout: landing
---

# KYS.Library.Excel Documentation

Welcome to **KYS.Library.Excel**, an add-on to **KYS.Library** for generating Excel workbooks in internal .NET applications.  
It turns a `DataTable` or `DataSet` into an `.xlsx` file (as a `byte[]`), with optional column formatting, header and summary row styling, and additional worksheets.

This site contains a high-level overview of the library and a full API reference automatically generated from XML documentation comments.

---

## 📦 Project Modules

### **KYS.Library.Excel**

Contains the options and models used to customize the generated workbook:

- `ExcelColumnFormat` – selects, renames, formats, aligns, and sums columns.
- `ExcelRowStyle` – styles a whole row (alignment, bold, font/background color, fill pattern, auto filter). Ships with `DefaultHeaderRowStyle` and `DefaultSummaryRowStyle`.
- `AdditionalExcelSheet` – describes an extra worksheet (data, column formats, header and summary styles).

### **KYS.Library.Excel.Helpers**

Includes the helper classes that build the workbook:

- `ExcelHelper` – `CreateExcelBook` from a `DataTable` or `DataSet` (one worksheet per table).
- `ExcelDataTableHelper` – `WriteToExcelFile` shortcut for a single `DataTable`.

All methods return a `Result<byte[]>` (CSharpFunctionalExtensions) instead of throwing.

---

## ⚡ Quick Example

```csharp
using KYS.Library.Excel.Helpers;
using KYS.Library.Excel.Options;
using OfficeOpenXml;

// Required once at application startup (see license note below)
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

Result<byte[]> excel = ExcelHelper.CreateExcelBook(
    dataTable,
    excelColumnFormats: new List<ExcelColumnFormat>
    {
        new() { ColumnName = "Name", DisplayedColumnName = "Student Name" },
        new() { ColumnName = "Score", Format = "#,##0.00", HasSumColumn = true }
    },
    headerRowStyle: ExcelRowStyle.DefaultHeaderRowStyle,
    summaryRowStyle: ExcelRowStyle.DefaultSummaryRowStyle);

if (excel.IsSuccess)
    File.WriteAllBytes("report.xlsx", excel.Value);
```

---

## ⚠️ Third-party License

This library is built on **EPPlus 6**, which is licensed under **PolyForm Noncommercial**. A commercial license is required for commercial use.

**The consuming application must set the EPPlus license context** before calling any helper. This library does not set it:

```csharp
ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial
```

---

## 🚀 Using This Library (Internal Use)

Since this library is not currently published to NuGet, it can be consumed by:

1. Cloning or downloading the repository
2. Building the solution
3. Locating the generated DLLs under:
   - _KYS.Library.Excel/bin/Debug/net8.0/_ or
   - _KYS.Library.Excel/bin/Release/net8.0/_
4. Adding the DLL references to your project manually (**KYS.Library.Excel** depends on **KYS.Library**, so reference both):

- In Visual Studio:  
  **Project → Add Reference → Browse → Select DLLs**
- In .NET CLI-based project:
  ```
  <ItemGroup>
    <Reference Include="KYS.Library">
      <HintPath>path/to/KYS.Library.dll</HintPath>
    </Reference>
    <Reference Include="KYS.Library.Excel">
      <HintPath>path/to/KYS.Library.Excel.dll</HintPath>
    </Reference>
  </ItemGroup>
  ```

5. Adding the third-party NuGet packages used by the library: `EPPlus` and `CSharpFunctionalExtensions`.

---

## 📘 API Reference

Detailed class, method, and property documentation is available under the **API** section of this site.  
All content is generated automatically based on XML documentation comments in the source code.

---

## 📂 Project Structure

KYS.Library.Excel/ <br />
├── Helpers/ <br />
├── Models/ <br />
└── Options/

_(Classes and namespaces inside each module can be viewed through the API reference.)_

---

## 🛠 Documentation

This documentation is generated using **DocFX** and will stay updated as new XML comments are added or modified.
