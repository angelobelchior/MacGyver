using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using ZXing;
using ZXing.SkiaSharp;


namespace MacGyver.Pages;

public class QrCodeBehind : ComponentBase
{
    [Inject] protected IJSRuntime? Js { get; set; }
    [Inject] protected IToastService? ToastService { get; set; }

    private string _base64 = string.Empty;

    protected int Size { get; set; } = 300;
    protected string Content { get; set; } = "https://dev.to/angelobelchior";
    protected string Image { get; set; } = string.Empty;

    protected void GenerateQrCode()
    {
        if (string.IsNullOrEmpty(Content))
        {
            ToastService!.ShowWarning(
                "Invalid content...",
                3000
            );
            return;
        }

        if (Size <= 0) Size = 300;

        var barcodeWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Height = Size,
                Width = Size
            }
        };
        using var bitmap = barcodeWriter.Write(Content);
        using var memoryStream = new MemoryStream();
        bitmap.Encode(memoryStream, SkiaSharp.SKEncodedImageFormat.Png, 100);
        var byteImage = memoryStream.ToArray();
        _base64 = Convert.ToBase64String(byteImage);
        Image = $"data:image/png;base64,{_base64}";

        ToastService!.ShowSuccess(
            "QR Code generated... Click on image to download.",
            3000
        );
    }

    protected async Task DownloadQrCodeImage()
    {
        var bytes = Convert.FromBase64String(_base64);
        var fileStream = new MemoryStream(bytes);
        var fileName = $"qrcode-{DateTime.Now:yyyyMMddhhmmss}.png";
        using var streamRef = new DotNetStreamReference(stream: fileStream);
        await Js!.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }
}