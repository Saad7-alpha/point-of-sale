using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace PosInventorySystem.Services;

public interface IBarcodeService
{
    string GenerateBarcode(string value, int width = 300, int height = 100);
    string GenerateQRCode(string value, int width = 200, int height = 200);
    byte[] GenerateBarcodeBytes(string value, int width = 300, int height = 100);
    byte[] GenerateQRCodeBytes(string value, int width = 200, int height = 200);
}

public class BarcodeService : IBarcodeService
{
    private readonly string _barcodePath;

    public BarcodeService(IWebHostEnvironment environment)
    {
        _barcodePath = Path.Combine(environment.WebRootPath, "barcodes");
        
        if (!Directory.Exists(_barcodePath))
            Directory.CreateDirectory(_barcodePath);
    }

    public string GenerateBarcode(string value, int width = 300, int height = 100)
    {
        var fileName = $"barcode_{Guid.NewGuid()}.png";
        var filePath = Path.Combine(_barcodePath, fileName);

        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 1,
                PureBarcode = false
            }
        };

        var bitmap = writer.Write(value);
        bitmap.Save(filePath);
        bitmap.Dispose();

        return $"/barcodes/{fileName}";
    }

    public string GenerateQRCode(string value, int width = 200, int height = 200)
    {
        var fileName = $"qrcode_{Guid.NewGuid()}.png";
        var filePath = Path.Combine(_barcodePath, fileName);

        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 1
            }
        };

        var bitmap = writer.Write(value);
        bitmap.Save(filePath);
        bitmap.Dispose();

        return $"/barcodes/{fileName}";
    }

    public byte[] GenerateBarcodeBytes(string value, int width = 300, int height = 100)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 1,
                PureBarcode = false
            }
        };

        using var bitmap = writer.Write(value);
        using var ms = new MemoryStream();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms.ToArray();
    }

    public byte[] GenerateQRCodeBytes(string value, int width = 200, int height = 200)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 1
            }
        };

        using var bitmap = writer.Write(value);
        using var ms = new MemoryStream();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms.ToArray();
    }
}
