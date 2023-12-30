using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MacGyver.Pages;

public class PasswordGeneratorCodeBehind : ComponentBase
{
    [Inject] protected IJSRuntime? Js { get; set; }
    [Inject] protected IToastService? ToastService { get; set; }

    protected int Length { get; set; } = 5;
    protected bool Numbers  { get; set; } = true;
    protected bool Uppercase  { get; set; } = true;
    protected bool Lowercase  { get; set; } = true;
    protected bool Symbols  { get; set; } = true;
    protected string Password  { get; set; } = string.Empty;

    protected void GeneratePassword()
    {
        if (!Numbers && !Uppercase && !Lowercase && !Symbols)
        {
            ToastService!.ShowWarning(
                "You must select at least one option!",
                3000
            );
            return;
        }

        var template = string.Empty;
        if (Numbers) template += "0123456789";
        if (Uppercase) template += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (Lowercase) template += "abcdefghijklmnopqrstuvwxyz";
        if (Symbols)  template += "!@#$%^&*()_+~`|}{[]:;?><,./-=";

        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            var index = Random.Shared.Next(template.Length);
            chars[i] = template[index];
        }

        Password = new string(chars);
    }

    protected async Task CopyToClipboard()
    {
        var result = await Js!.InvokeAsync<bool>("copyToClipboard", Password);
        if (!result) return;
        ToastService!.ShowSuccess(
            "Copied to clipboard!",
            3000
        );
    }
}