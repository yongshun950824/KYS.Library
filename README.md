# KYS.Library

Set of helpers, extension methods, services and validations (attributes) to help the developers not to reinvent the wheel. 

---

## Projects

| Project | Build | Documentation | Quality Gate | Coverage |
| :--- | :--- | :---: | :---: | :---: |
| [KYS.Library][1] | [![CI - Build KYS.Library][2]][3] | [View Docs][7] | [![Quality Gate Status][4]][5] | [![Coverage][6]][5] |
| [KYS.AspNetCore.Library][21] | [![CI - Build KYS.AspNetCore Library][22]][23] | [View Docs][27] | [![Quality Gate Status][24]][25] | [![Coverage][26]][25] |
| [KYS.EFCore.Library][41] | [![CI - Build KYS.EFCore Library][42]][43] | Coming Soon | Coming Soon | Coming Soon |

---

## Sonar Cloud Stats

![SonarQube Cloud](https://sonarcloud.io/images/project_badges/sonarcloud-light.svg)

### 📦 KYS.Library
[![Quality Gate Status][4]][5] [![Coverage][6]][5]

<details>
<summary>🔍 View Full SonarQube Report</summary>

| Metric | Badge |
| :--- | :--- |
| **Reliability** | [![Reliability Rating][8]][5] |
| **Security** | [![Security Rating][9]][5] |
| **Vulnerabilities** | [![Vulnerabilities][10]][5] |
| **Technical Debt** | [![Technical Debt][11]][5] |
| **Duplicated Lines** | [![Duplicated Lines (%)][12]][5] |
| **Code Smells** | [![Code Smells][13]][5] |
| **Maintainability** | [![Maintainability Rating][14]][5] |

</details>

### 📦 KYS.AspNetCore.Library
[![Quality Gate Status][24]][25] [![Coverage][26]][25]

<details>
<summary>🔍 View Full SonarQube Report</summary>

| Metric | Badge |
| :--- | :--- |
| **Reliability** | [![Reliability Rating][28]][25] |
| **Security** | [![Security Rating][29]][25] |
| **Vulnerabilities** | [![Vulnerabilities][30]][25] |
| **Technical Debt** | [![Technical Debt][31]][25] |
| **Duplicated Lines** | [![Duplicated Lines (%)][32]][25] |
| **Code Smells** | [![Code Smells][33]][25] |
| **Maintainability** | [![Maintainability Rating][34]][25] |

</details>

---

## 🛠️ Getting Started

### 📋 Prerequisites

- **Runtime:** .NET 8.0 (whichever you are targeting)
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

## 📖 Usage Examples
Detailed examples for helpers, extensions, and etc are available in the [Documentation](#projects).

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