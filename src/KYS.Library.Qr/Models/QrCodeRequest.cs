using System;

namespace KYS.Library.Qr.Models;

public sealed record QrCodeRequest(
    string Value,
    int Width = 200,
    int Height = 200,
    int Margin = 0,
    string? LogoPath = null,
    int? LogoWidth = null,
    int? LogoHeight = null
);
