using FishingSpot.PWA.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace FishingSpot.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public void NewService_StartsUnauthenticated()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Supabase:Url"] = "https://example.supabase.co",
                ["Supabase:Key"] = "anon-key"
            })
            .Build();

        var service = new AuthService(new HttpClient(), configuration, new NoopJSRuntime());

        Assert.False(service.IsAuthenticated);
        Assert.Null(service.CurrentUser);
        Assert.Null(service.AccessToken);
    }

    private sealed class NoopJSRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            throw new NotSupportedException();
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            throw new NotSupportedException();
        }
    }
}
