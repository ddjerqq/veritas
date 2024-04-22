using Blazored.Toast.Services;

namespace Client.Services;

public class ErrorLoggerHttpClientHandler(HttpMessageHandler innerHandler, IToastService toast) : DelegatingHandler(innerHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine("error client start");

        try
        {
            var resp = await base.SendAsync(request, cancellationToken);
            resp.EnsureSuccessStatusCode();

            Console.WriteLine("error client end");
            return resp;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            toast.ShowError("დაფიქსირდა შეცდომა, გთხოვთ გადატვირთოთ გვერდი!");
            return null!;
        }
    }
}