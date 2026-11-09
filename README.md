# KYS.Library

Set of helpers, extension methods, services and validations (attributes) to help the developers not to reinvent the wheel.

---

## Projects

| Project                      | Build                                          |  Documentation  |           Quality Gate           |       Coverage        |
| :--------------------------- | :--------------------------------------------- | :-------------: | :------------------------------: | :-------------------: |
| [KYS.Library][1]             | [![CI - Build KYS.Library][2]][3]              | [View Docs][7]  |  [![Quality Gate Status][4]][5]  |  [![Coverage][6]][5]  |
| [KYS.Library.Excel][61]      | [![CI - Build KYS.Library.Excel][62]][63]      | [View Docs][67] | [![Quality Gate Status][64]][65] | [![Coverage][66]][65] |
| [KYS.Library.Pdf][81]      | [![CI - Build KYS.Library.Pdf][82]][83]      | [View Docs][87] | [![Quality Gate Status][84]][85] | [![Coverage][86]][85] |
| [KYS.Library.Qr][101]      | [![CI - Build KYS.Library.Qr][102]][103]      | [View Docs][107] | [![Quality Gate Status][104]][105] | [![Coverage][106]][105] |
| [KYS.Library.Zip][121]      | [![CI - Build KYS.Library.Zip][122]][123]      | [View Docs][127] | [![Quality Gate Status][124]][125] | [![Coverage][126]][125] |
| [KYS.AspNetCore.Library][21] | [![CI - Build KYS.AspNetCore Library][22]][23] | [View Docs][27] | [![Quality Gate Status][24]][25] | [![Coverage][26]][25] |
| [KYS.EFCore.Library][41]     | [![CI - Build KYS.EFCore Library][42]][43]     | [View Docs][47] | [![Quality Gate Status][44]][45] | [![Coverage][46]][45] |

---

## 🧰 What's Inside

| Project                    | Highlights                                                                                                                                                                                                                                 |
| :------------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **KYS.Library**            | DataTable/file/stream/JSON/XML/SQL/URI helpers, translation services, validation attributes, and extension methods                                                                                                                           |
| **KYS.Library.Excel**      | DataSet/DataTable to Excel export (`ExcelHelper`, `ExcelDataTableHelper`), column formatting, row styles, and multi-sheet support with EPPlus                                                                                                 |
| **KYS.Library.Pdf**        | HTML-to-PDF conversion, PDF password lock/unlock support, and DataTable-to-PDF rendering with styling via iText                                                                                                                               |
| **KYS.Library.Qr**         | QR code generation from text/value input, optional logo embedding, and image/data URI output via ZXing + SkiaSharp                                                                                                                           |
| **KYS.Library.Zip**        | ZIP archive creation/extraction, optional password protection, and file-item packaging through `ZipCreator` and `ZipExtractor`                                                                                                            |
| **KYS.AspNetCore.Library** | API response model, security headers middlewares (API / Web), `HttpContext` extensions, view result helper, JSON-to-HTML tag helper                                                                                                         |
| **KYS.EFCore.Library**     | DDD building blocks (`Entity`, `ValueObject`, `Enumeration`, `IAggregateRoot`), Repository / Unit of Work abstractions, Specification pattern, audit logging, enum description converter                                                    |

---

## 🧭 Design Conventions

Most helpers return `Result` / `Result<T>` from
[CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions)
instead of throwing exceptions. Check `IsSuccess` / `Error`, or chain the calls with `Bind` / `Map`:

```csharp
Result<string> result = QrGenerator.Create("https://example.com")
    .Bind(qr => qr.DrawAndToDataUri());

if (result.IsFailure)
    logger.LogWarning(result.Error);
```

---

## 🛠️ Getting Started

### 📋 Prerequisites

- **SDK:** .NET 8.0
- **IDE:** Visual Studio 2022 or VS Code

The library is not yet on NuGet (Future Plan), you can include it in your solution via Project Reference:

1. Clone the repository: `git clone https://github.com/yongshun950824/KYS.Library.git`
2. Add the desired project to your solution.
3. Reference it in your project file:

```xml
<ItemGroup>
  <ProjectReference Include="..\KYS.Library\src\KYS.Library\KYS.Library.csproj" />
</ItemGroup>
```

---

## 📖 Usage Examples

### QR code

```csharp
Result<string> base64 = QrGenerator.Create("https://example.com")
    .Bind(qr => qr.DrawAndToBase64());
```

### Excel export

```csharp
// Requires the EPPlus license to be configured at startup (see Third-party Licenses)
Result<byte[]> excel = ExcelHelper.CreateExcelBook(dataTable);

if (excel.IsSuccess)
    File.WriteAllBytes("report.xlsx", excel.Value);
```

Detailed examples for helpers, extensions, and etc are available in the [Documentation](#projects).

---

## ⚠️ Third-party Licenses

This repository is MIT licensed, but some dependencies are not:

| Dependency | License                                                                      | Used by                                           |
| :--------- | :--------------------------------------------------------------------------- | :------------------------------------------------ |
| EPPlus 6   | PolyForm Noncommercial (a commercial license is required for commercial use) | `ExcelHelper`, `ExcelDataTableHelper.WriteToExcelFile` |
| iText 7    | AGPL (commercial license available)                                          | `PdfHelper`, `PdfDataTableHelper.WriteToPdfFile`     |

**EPPlus must be configured by your application** before using the Excel helpers. The library does not set it:

```csharp
// Program.cs
ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial
```

Alternatively, configure it in `appsettings.json`:

```json
{
  "EPPlus": {
    "ExcelPackage": {
      "LicenseContext": "Commercial"
    }
  }
}
```

> Please review the license terms of these dependencies before using this library in a commercial product.

---

## 📊 Sonar Cloud Stats

![SonarQube Cloud](https://sonarcloud.io/images/project_badges/sonarcloud-light.svg)

<details>
<summary>📦 KYS.Library — View Full SonarQube Report</summary>

| Metric                  | Badge                              |
| :---------------------- | :--------------------------------- |
| **Quality Gate Status** | [![Quality Gate Status][4]][5]     |
| **Coverage**            | [![Coverage][6]][5]                |
| **Reliability**         | [![Reliability Rating][8]][5]      |
| **Security**            | [![Security Rating][9]][5]         |
| **Vulnerabilities**     | [![Vulnerabilities][10]][5]        |
| **Technical Debt**      | [![Technical Debt][11]][5]         |
| **Duplicated Lines**    | [![Duplicated Lines (%)][12]][5]   |
| **Code Smells**         | [![Code Smells][13]][5]            |
| **Maintainability**     | [![Maintainability Rating][14]][5] |

</details>

<details>
<summary>📦 KYS.Library.Excel — View Full SonarQube Report</summary>

| Metric                  | Badge                              |
| :---------------------- | :--------------------------------- |
| **Quality Gate Status** | [![Quality Gate Status][64]][65]     |
| **Coverage**            | [![Coverage][66]][65]                |
| **Reliability**         | [![Reliability Rating][68]][65]      |
| **Security**            | [![Security Rating][69]][65]         |
| **Vulnerabilities**     | [![Vulnerabilities][70]][65]        |
| **Technical Debt**      | [![Technical Debt][71]][65]         |
| **Duplicated Lines**    | [![Duplicated Lines (%)][72]][65]   |
| **Code Smells**         | [![Code Smells][73]][65]            |
| **Maintainability**     | [![Maintainability Rating][74]][65] |

</details>

<details>
<summary>📦 KYS.Library.Pdf — View Full SonarQube Report</summary>

| Metric                  | Badge                              |
| :---------------------- | :--------------------------------- |
| **Quality Gate Status** | [![Quality Gate Status][84]][85]     |
| **Coverage**            | [![Coverage][86]][85]                |
| **Reliability**         | [![Reliability Rating][88]][85]      |
| **Security**            | [![Security Rating][89]][85]         |
| **Vulnerabilities**     | [![Vulnerabilities][90]][85]        |
| **Technical Debt**      | [![Technical Debt][91]][85]         |
| **Duplicated Lines**    | [![Duplicated Lines (%)][92]][85]   |
| **Code Smells**         | [![Code Smells][93]][85]            |
| **Maintainability**     | [![Maintainability Rating][94]][85] |

</details>

<details>
<summary>📦 KYS.Library.Qr — View Full SonarQube Report</summary>

| Metric                  | Badge                              |
| :---------------------- | :--------------------------------- |
| **Quality Gate Status** | [![Quality Gate Status][104]][105]     |
| **Coverage**            | [![Coverage][106]][105]                |
| **Reliability**         | [![Reliability Rating][108]][85]      |
| **Security**            | [![Security Rating][109]][105]         |
| **Vulnerabilities**     | [![Vulnerabilities][110]][105]        |
| **Technical Debt**      | [![Technical Debt][111]][105]         |
| **Duplicated Lines**    | [![Duplicated Lines (%)][112]][105]   |
| **Code Smells**         | [![Code Smells][113]][105]            |
| **Maintainability**     | [![Maintainability Rating][114]][105] |

</details>

<details>
<summary>📦 KYS.Library.Zip — View Full SonarQube Report</summary>

| Metric                  | Badge                              |
| :---------------------- | :--------------------------------- |
| **Quality Gate Status** | [![Quality Gate Status][124]][125]     |
| **Coverage**            | [![Coverage][126]][125]                |
| **Reliability**         | [![Reliability Rating][128]][85]      |
| **Security**            | [![Security Rating][129]][125]         |
| **Vulnerabilities**     | [![Vulnerabilities][130]][125]        |
| **Technical Debt**      | [![Technical Debt][131]][125]         |
| **Duplicated Lines**    | [![Duplicated Lines (%)][132]][125]   |
| **Code Smells**         | [![Code Smells][133]][125]            |
| **Maintainability**     | [![Maintainability Rating][134]][125] |

</details>

<details>
<summary>📦 KYS.AspNetCore.Library — View Full SonarQube Report</summary>

| Metric                  | Badge                               |
| :---------------------- | :---------------------------------- |
| **Quality Gate Status** | [![Quality Gate Status][24]][25]    |
| **Coverage**            | [![Coverage][26]][25]               |
| **Reliability**         | [![Reliability Rating][28]][25]     |
| **Security**            | [![Security Rating][29]][25]        |
| **Vulnerabilities**     | [![Vulnerabilities][30]][25]        |
| **Technical Debt**      | [![Technical Debt][31]][25]         |
| **Duplicated Lines**    | [![Duplicated Lines (%)][32]][25]   |
| **Code Smells**         | [![Code Smells][33]][25]            |
| **Maintainability**     | [![Maintainability Rating][34]][25] |

</details>

<details>
<summary>📦 KYS.EFCore.Library — View Full SonarQube Report</summary>

| Metric                  | Badge                               |
| :---------------------- | :---------------------------------- |
| **Quality Gate Status** | [![Quality Gate Status][44]][45]    |
| **Coverage**            | [![Coverage][46]][45]               |
| **Reliability**         | [![Reliability Rating][48]][45]     |
| **Security**            | [![Security Rating][49]][45]        |
| **Vulnerabilities**     | [![Vulnerabilities][50]][45]        |
| **Technical Debt**      | [![Technical Debt][51]][45]         |
| **Duplicated Lines**    | [![Duplicated Lines (%)][52]][45]   |
| **Code Smells**         | [![Code Smells][53]][45]            |
| **Maintainability**     | [![Maintainability Rating][54]][45] |

</details>

---

## 📝 Changelog

See [Releases](https://github.com/yongshun950824/KYS.Library/releases) for version history and breaking changes.

## 📄 License

[MIT](LICENSE) — see [Third-party Licenses](#️-third-party-licenses) for dependency terms.

---

[1]: https://github.com/yongshun950824/KYS.Library/tree/master/src/KYS.Library
[2]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.Library-build.yml/badge.svg
[3]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.Library-build.yml
[4]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library&metric=alert_status
[5]: https://sonarcloud.io/project/overview?id=yongshun950824_kys-library
[6]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library&metric=coverage
[7]: https://yongshun950824.github.io/KYS.Library/kys-library/index.html
[8]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library&metric=reliability_rating
[9]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library&metric=security_rating
[10]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library&metric=vulnerabilities
[11]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library&metric=sqale_index
[12]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library&metric=duplicated_lines_density
[13]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library&metric=code_smells
[14]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library&metric=sqale_rating
[21]: https://github.com/yongshun950824/KYS.Library/tree/master/src/KYS.AspNetCore.Library
[22]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.AspNetCore.Library-build.yml/badge.svg
[23]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.AspNetCore.Library-build.yml
[24]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-aspnetcore-library&metric=alert_status
[25]: https://sonarcloud.io/project/overview?id=yongshun950824_kys-aspnetcore-library
[26]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-aspnetcore-library&metric=coverage
[27]: https://yongshun950824.github.io/KYS.Library/kys-aspnetcore-library/index.html
[28]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-aspnetcore-library&metric=reliability_rating
[29]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-aspnetcore-library&metric=security_rating
[30]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-aspnetcore-library&metric=vulnerabilities
[31]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-aspnetcore-library&metric=sqale_index
[32]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-aspnetcore-library&metric=duplicated_lines_density
[33]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-aspnetcore-library&metric=code_smells
[34]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-aspnetcore-library&metric=sqale_rating
[41]: https://github.com/yongshun950824/KYS.Library/tree/master/src/KYS.EFCore.Library
[42]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.EFCore.Library-build.yml/badge.svg
[43]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.EFCore.Library-build.yml
[44]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-efcore-library&metric=alert_status
[45]: https://sonarcloud.io/project/overview?id=yongshun950824_kys-efcore-library
[46]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-efcore-library&metric=coverage
[47]: https://yongshun950824.github.io/KYS.Library/kys-efcore-library/index.html
[48]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-efcore-library&metric=reliability_rating
[49]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-efcore-library&metric=security_rating
[50]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-efcore-library&metric=vulnerabilities
[51]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-efcore-library&metric=sqale_index
[52]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-efcore-library&metric=duplicated_lines_density
[53]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-efcore-library&metric=code_smells
[54]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-efcore-library&metric=sqale_rating
[61]: https://github.com/yongshun950824/KYS.Library/tree/master/src/KYS.Library.Excel
[62]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.Library.Excel-build.yml/badge.svg
[63]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.Library.Excel-build.yml
[64]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-excel&metric=alert_status
[65]: https://sonarcloud.io/project/overview?id=yongshun950824_kys-library-excel
[66]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-excel&metric=coverage
[67]: https://yongshun950824.github.io/KYS.Library/kys-library-excel/index.html
[68]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-excel&metric=reliability_rating
[69]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-excel&metric=security_rating
[70]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-excel&metric=vulnerabilities
[71]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-excel&metric=sqale_index
[72]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-excel&metric=duplicated_lines_density
[73]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-excel&metric=code_smells
[74]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-excel&metric=sqale_rating
[81]: https://github.com/yongshun950824/KYS.Library/tree/master/src/KYS.Library.Pdf
[82]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.Library.Pdf-build.yml/badge.svg
[83]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.Library.Pdf-build.yml
[84]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-pdf&metric=alert_status
[85]: https://sonarcloud.io/project/overview?id=yongshun950824_kys-library-pdf
[86]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-pdf&metric=coverage
[87]: https://yongshun950824.github.io/KYS.Library/kys-library-pdf/index.html
[88]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-pdf&metric=reliability_rating
[89]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-pdf&metric=security_rating
[90]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-pdf&metric=vulnerabilities
[91]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-pdf&metric=sqale_index
[92]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-pdf&metric=duplicated_lines_density
[93]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-pdf&metric=code_smells
[94]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-pdf&metric=sqale_rating
[101]: https://github.com/yongshun950824/KYS.Library/tree/master/src/KYS.Library.Qr
[102]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.Library.Qr-build.yml/badge.svg
[103]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.Library.Qr-build.yml
[104]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-qr&metric=alert_status
[105]: https://sonarcloud.io/project/overview?id=yongshun950824_kys-library-qr
[106]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-qr&metric=coverage
[107]: https://yongshun950824.github.io/KYS.Library/kys-library-qr/index.html
[108]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-qr&metric=reliability_rating
[109]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-qr&metric=security_rating
[110]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-qr&metric=vulnerabilities
[111]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-qr&metric=sqale_index
[112]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-qr&metric=duplicated_lines_density
[113]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-qr&metric=code_smells
[114]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-qr&metric=sqale_rating
[121]: https://github.com/yongshun950824/KYS.Library/tree/master/src/KYS.Library.Zip
[122]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.Library.Zip-build.yml/badge.svg
[123]: https://github.com/yongshun950824/KYS.Library/actions/workflows/KYS.Library.Zip-build.yml
[124]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-zip&metric=alert_status
[125]: https://sonarcloud.io/project/overview?id=yongshun950824_kys-library-zip
[126]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-zip&metric=coverage
[127]: https://yongshun950824.github.io/KYS.Library/kys-library-zip/index.html
[128]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-zip&metric=reliability_rating
[129]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-zip&metric=security_rating
[130]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-zip&metric=vulnerabilities
[131]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-zip&metric=sqale_index
[132]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-zip&metric=duplicated_lines_density
[133]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-zip&metric=code_smells
[134]: https://sonarcloud.io/api/project_badges/measure?project=yongshun950824_kys-library-zip&metric=sqale_rating