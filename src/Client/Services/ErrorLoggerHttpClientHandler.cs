using Blazored.Toast.Services;
using Client.Common;

namespace Client.Services;

public class ErrorLoggerHttpClientHandler(HttpMessageHandler innerHandler, IToastService toast) : DelegatingHandler(innerHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var resp = await base.SendAsync(request, cancellationToken);
            resp.EnsureSuccessStatusCode();
            return resp;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            AppComponentBase.ShowToast(toast, ToastLevel.Error, $"დაფიქსირდა შეცდომა, გთხოვთ გადატვირთოთ გვერდი! {ex.Message}");
            return null!;
        }
    }
}