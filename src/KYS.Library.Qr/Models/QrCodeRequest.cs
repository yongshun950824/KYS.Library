namespace KYS.Library.Qr.Models;

public sealed record QrCodeRequest(
    string Value,
    int Width = 200,
    int Height = 200,
    int Margin = 0,
    string? LogoPath = null,
    int? LogoWidth = null,
    int? LogoHeight = null
)
{
    public string Value { get; init; } = !string.IsNullOrEmpty(Value)
        ? Value
        : throw new ArgumentException($"{nameof(Value)} is required.");

    public int Width { get; init; } = Width > 0
        ? Width
        : throw new ArgumentException($"{nameof(Width)} must be greater than zero.");

    public int Height { get; init; } = Height > 0
        ? Height
        : throw new ArgumentException($"{nameof(Height)} must be greater than zero.");

    public int Margin { get; init; } = Margin >= 0
        ? Margin
        : throw new ArgumentException($"{nameof(Margin)} must be a positive number.");

    private readonly string? _logoPath = ValidateLogoPath(LogoPath);
    public string? LogoPath
    {
        get => _logoPath;
        init => _logoPath = ValidateLogoPath(value);
    }

    private readonly int? _logoWidth = ValidateLogoWidth(LogoWidth);
    public int? LogoWidth
    {
        get => _logoWidth;
        init => _logoWidth = ValidateLogoWidth(value);
    }

    private readonly int? _logoHeight = ValidateLogoHeight(LogoHeight);
    public int? LogoHeight
    {
        get => _logoHeight;
        init => _logoHeight = ValidateLogoHeight(value);
    }

    private static string? ValidateLogoPath(string? value)
    {
        if (value != null && !File.Exists(value))
        {
            throw new FileNotFoundException($"Logo file not found: {value}.");
        }

        return value;
    }

    private static int? ValidateLogoWidth(int? value)
    {
        if (value.HasValue && value <= 0)
        {
            throw new ArgumentException($"{nameof(LogoWidth)} must be greater than zero.");
        }

        return value;
    }

    private static int? ValidateLogoHeight(int? value)
    {
        if (value.HasValue && value <= 0)
        {
            throw new ArgumentException($"{nameof(LogoHeight)} must be greater than zero.");
        }

        return value;
    }
}
