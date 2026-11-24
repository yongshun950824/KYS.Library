---
_layout: landing
---

# KYS.Library.Qr Documentation

Welcome to **KYS.Library.Qr**, an add-on to **KYS.Library** for generating QR codes in internal .NET applications.  
It draws QR codes (optionally with a centered logo) and outputs them as a bitmap, stream, Base64 string, data URI, or PNG file.

This site contains a high-level overview of the library and a full API reference automatically generated from XML documentation comments.

---

## 📦 Project Modules

### **KYS.Library.Qr.Services**

Contains the QR code generation service:

- `IQrGenerator` – the contract for drawing a QR code and converting it to different outputs.
- `QrGenerator` – the implementation, created through validated factory methods:
  - `Create(value)` / `Create(value, width, height, margin)`
  - `CreateWithLogo(value, logoPath)` / `CreateWithLogo(value, logoPath, width, height, margin, logoWidth, logoHeight)`
- Output methods: `Draw` (`SKBitmap`), `DrawAndToMemoryStream`, `DrawAndToBase64`, `DrawAndToDataUri`, and `DrawAndToFile` (PNG).

QR codes are generated with error correction level **H**, so a logo can cover the center without breaking scanning.  
All factory and output methods return a `Result` / `Result<T>` (CSharpFunctionalExtensions) instead of throwing.

---

## ⚡ Quick Example

```csharp
using KYS.Library.Qr.Services;

// Data URI, ready for an <img src="..."> tag
Result<string> dataUri = QrGenerator.Create("https://example.com")
    .Bind(qr => qr.DrawAndToDataUri());

// QR code with a logo, saved as a PNG file
Result saved = QrGenerator.CreateWithLogo("https://example.com", "logo.png")
    .Bind(qr => qr.DrawAndToFile("qr-code.png"));
```

---

## 🚀 Using This Library (Internal Use)

Since this library is not currently published to NuGet, it can be consumed by:

1. Cloning or downloading the repository
2. Building the solution
3. Locating the generated DLLs under:
   - _KYS.Library.Qr/bin/Debug/net8.0/_ or
   - _KYS.Library.Qr/bin/Release/net8.0/_
4. Adding the DLL references to your project manually (**KYS.Library.Qr** depends on **KYS.Library**, so reference both):

- In Visual Studio:  
  **Project → Add Reference → Browse → Select DLLs**
- In .NET CLI-based project:
  ```
  <ItemGroup>
    <Reference Include="KYS.Library">
      <HintPath>path/to/KYS.Library.dll</HintPath>
    </Reference>
    <Reference Include="KYS.Library.Qr">
      <HintPath>path/to/KYS.Library.Qr.dll</HintPath>
    </Reference>
  </ItemGroup>
  ```

5. Adding the third-party NuGet packages used by the library: `SkiaSharp`, `ZXing.Net`, `ZXing.Net.Bindings.SkiaSharp`, and `CSharpFunctionalExtensions`.  
   When running on Linux (e.g. Docker), also add `SkiaSharp.NativeAssets.Linux.NoDependencies`.

---

## 📘 API Reference

Detailed class, method, and property documentation is available under the **API** section of this site.  
All content is generated automatically based on XML documentation comments in the source code.

---

## 📂 Project Structure

KYS.Library.Qr/ <br />
└── Services/

_(Classes and namespaces inside each module can be viewed through the API reference.)_

---

## 🛠 Documentation

This documentation is generated using **DocFX** and will stay updated as new XML comments are added or modified.
