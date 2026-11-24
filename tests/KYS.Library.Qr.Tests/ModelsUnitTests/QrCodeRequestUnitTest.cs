using KYS.Library.Qr.Models;

namespace KYS.Library.Qr.Tests.ModelsUnitTests;

public class QrCodeRequestUnitTest
{
    [Test]
    public void QrCodeRequest_WithNullValue_ShouldThrowException()
    {
        // Act
        var ex = Assert.Throws<ArgumentException>(
            () =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                QrCodeRequest qrCodeRequest = new QrCodeRequest(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        );

        // Assert
        Assert.IsInstanceOf<ArgumentException>(ex);
    }

    [Test]
    public void QrCodeRequest_WithInvalidWidth_ShouldThrowException()
    {
        // Act
        var ex = Assert.Throws<ArgumentException>(
            () =>
            {
                QrCodeRequest qrCodeRequest = new QrCodeRequest("test", Width: -10);
            }
        );

        // Assert
        Assert.IsInstanceOf<ArgumentException>(ex);
        Assert.AreEqual($"{nameof(QrCodeRequest.Width)} must be greater than zero.", ex!.Message);
    }

    [Test]
    public void QrCodeRequest_WithInvalidHeight_ShouldThrowException()
    {
        // Act
        var ex = Assert.Throws<ArgumentException>(
            () =>
            {
                QrCodeRequest qrCodeRequest = new QrCodeRequest("test", Height: -10);
            }
        );

        // Assert
        Assert.IsInstanceOf<ArgumentException>(ex);
        Assert.AreEqual($"{nameof(QrCodeRequest.Height)} must be greater than zero.", ex!.Message);
    }

    [Test]
    public void QrCodeRequest_WithInvalidMargin_ShouldThrowException()
    {
        // Act
        var ex = Assert.Throws<ArgumentException>(
            () =>
            {
                QrCodeRequest qrCodeRequest = new QrCodeRequest("test", Margin: -10);
            }
        );

        // Assert
        Assert.IsInstanceOf<ArgumentException>(ex);
        Assert.AreEqual($"{nameof(QrCodeRequest.Margin)} must be a positive number.", ex!.Message);
    }

    [Test]
    public void QrCodeRequest_WithInvalidLogoPath_ShouldThrowException()
    {
        // Arrange
        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "dummy-image.png");

        // Act
        var ex = Assert.Throws<FileNotFoundException>(
            () =>
            {
                QrCodeRequest qrCodeRequest = new("test", LogoPath: logoPath);
                qrCodeRequest = qrCodeRequest with { LogoPath = logoPath };
            }
        );

        // Assert
        Assert.IsInstanceOf<FileNotFoundException>(ex);
        Assert.AreEqual($"Logo file not found: {logoPath}.", ex!.Message);
    }

    [Test]
    public void QrCodeRequest_WithValidLogoPath_ShouldAllow()
    {
        // Arrange
        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "logo.png");

        // Act
        QrCodeRequest qrCodeRequest = new("test", LogoPath: logoPath);
        qrCodeRequest = qrCodeRequest with { LogoPath = logoPath };

        // Assert
        Assert.IsNotNull(qrCodeRequest);
        Assert.AreEqual(logoPath, qrCodeRequest.LogoPath);
    }

    [Test]
    public void QrCodeRequest_WithInvalidLogoWidth_ShouldThrowException()
    {
        // Act
        var ex = Assert.Throws<ArgumentException>(
            () =>
            {
                QrCodeRequest qrCodeRequest = new("test");
                qrCodeRequest = qrCodeRequest with { LogoWidth = 0 };
            }
        );

        // Assert
        Assert.IsInstanceOf<ArgumentException>(ex);
        Assert.AreEqual($"{nameof(QrCodeRequest.LogoWidth)} must be greater than zero.", ex!.Message);
    }

    [Test]
    public void QrCodeRequest_WithValidLogoWidth_ShouldAllow()
    {
        // Act
        QrCodeRequest qrCodeRequest = new("test");
        qrCodeRequest = qrCodeRequest with { LogoWidth = 20 };

        // Assert
        Assert.IsNotNull(qrCodeRequest);
        Assert.AreEqual(20, qrCodeRequest.LogoWidth);
    }

    [Test]
    public void QrCodeRequest_WithInvalidLogoHeight_ShouldThrowException()
    {
        var ex = Assert.Throws<ArgumentException>(
            () =>
            {
                QrCodeRequest qrCodeRequest = new("test");
                qrCodeRequest = qrCodeRequest with { LogoHeight = 0 };
            }
        );

        Assert.IsInstanceOf<ArgumentException>(ex);
        Assert.AreEqual($"{nameof(QrCodeRequest.LogoHeight)} must be greater than zero.", ex!.Message);
    }

    [Test]
    public void QrCodeRequest_WithValidLogoHeight_ShouldAllow()
    {
        // Act
        QrCodeRequest qrCodeRequest = new("test");
        qrCodeRequest = qrCodeRequest with { LogoHeight = 20 };

        // Assert
        Assert.IsNotNull(qrCodeRequest);
        Assert.AreEqual(20, qrCodeRequest.LogoHeight);
    }
}
