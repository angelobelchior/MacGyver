using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MacGyver.Pages;

public class GuidCodeBehind : ComponentBase
{
    [Inject] protected IJSRuntime? Js { get; set; }
    [Inject] protected IToastService? ToastService { get; set; }

    protected int Max { get; set; } = 1;
    protected bool Hyphens  { get; set; } = true;
    protected bool Upper  { get; set; } 
    protected bool Braces  { get; set; }
    protected string Quotes  { get; set; } = " ";
    protected string Separator  { get; set; } = "\n";
    protected string Values  { get; set; } = string.Empty;

    protected void GenerateGuids()
    {
        var items = 
            Enumerable.Range(0, Max)
                      .Select(_ => Generator());
        Values = string.Join(Separator, items);
        return;

        string Generator() => Hyphens(Upper(Quotes(Braces(NewGuid()))));
        string NewGuid() => System.Guid.NewGuid().ToString()!;
        string Quotes(string value) => $"{this.Quotes}{value}{this.Quotes}".Trim();
        string Braces(string value) => this.Braces ? $"{{{value}}}" : value;
        string Upper(string value) => this.Upper ? value.ToUpper() : value;
        string Hyphens(string value) => this.Hyphens ? value : value.Replace("-", "");
    }

    protected async Task CopyToClipboard()
    {
        var result = await Js!.InvokeAsync<bool>("copyToClipboard", Values);
        if (!result) return;
        ToastService!.ShowSuccess(
            "Copied to clipboard!",
            3000
        );
    }
}