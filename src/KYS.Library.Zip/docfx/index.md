---
_layout: landing
---

# KYS.Library.Zip Documentation

Welcome to **KYS.Library.Zip**, an add-on to **KYS.Library** for creating and extracting zip archives in internal .NET applications.  
It zips in-memory file contents (optionally password-protected) and extracts zip files to a directory, loading the extracted items back for further use.

This site contains a high-level overview of the library and a full API reference automatically generated from XML documentation comments.

---

## 📦 Project Modules

### **KYS.Library.Zip**

Contains the shared model:

- `ZipFileItem` – a file entry (name and byte content) to be zipped, or one that was extracted from a zip file.

### **KYS.Library.Zip.Services**

Contains the zip services:

- `IZipCreator` / `ZipCreator` – zips a list of `ZipFileItem` (with an optional password) into a `byte[]` (`Zip`) or saves it to a directory (`ZipAndToFile`).
- `IZipExtractor` / `ZipExtractor` – extracts a zip file (with an optional password) to a directory (`Unzip`) and exposes the extracted `FileName` and `FileItems`.

Operations return a `Result` / `Result<T>` (CSharpFunctionalExtensions).

---

## ⚡ Quick Example

```csharp
using KYS.Library.Zip;
using KYS.Library.Zip.Services;

// Create a password-protected zip
var creator = new ZipCreator(
    "documents",
    new List<ZipFileItem>
    {
        new() { Name = "readme.txt", Contents = File.ReadAllBytes("readme.txt") }
    },
    password: "secret");

Result saved = creator.ZipAndToFile("output");

// Extract it again
var extractor = new ZipExtractor { Password = "secret" };
Result unzipped = extractor.Unzip("output/documents.zip", "extracted");
```

---

## 🚀 Using This Library (Internal Use)

Since this library is not currently published to NuGet, it can be consumed by:

1. Cloning or downloading the repository
2. Building the solution
3. Locating the generated DLLs under:
   - _KYS.Library.Zip/bin/Debug/net8.0/_ or
   - _KYS.Library.Zip/bin/Release/net8.0/_
4. Adding the DLL references to your project manually (**KYS.Library.Zip** depends on **KYS.Library**, so reference both):

- In Visual Studio:  
  **Project → Add Reference → Browse → Select DLLs**
- In .NET CLI-based project:
  ```
  <ItemGroup>
    <Reference Include="KYS.Library">
      <HintPath>path/to/KYS.Library.dll</HintPath>
    </Reference>
    <Reference Include="KYS.Library.Zip">
      <HintPath>path/to/KYS.Library.Zip.dll</HintPath>
    </Reference>
  </ItemGroup>
  ```

5. Adding the third-party NuGet packages used by the library: `SharpZipLib` and `CSharpFunctionalExtensions`.

---

## 📘 API Reference

Detailed class, method, and property documentation is available under the **API** section of this site.  
All content is generated automatically based on XML documentation comments in the source code.

---

## 📂 Project Structure

KYS.Library.Zip/ <br />
├── Models/ <br />
└── Services/

_(Classes and namespaces inside each module can be viewed through the API reference.)_

---

## 🛠 Documentation

This documentation is generated using **DocFX** and will stay updated as new XML comments are added or modified.
