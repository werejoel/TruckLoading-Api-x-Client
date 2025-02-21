using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace TruckLoadingApp.Blazor.Services
{
    public class CustomAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;

        public CustomAuthorizationMessageHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            var expiration = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "tokenExpiration");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(expiration))
            {
                DateTime expTime;
                if (DateTime.TryParse(expiration, out expTime))
                {
                    if (expTime <= DateTime.UtcNow)
                    {
                        // Token expired, log out user
                        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
                        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userInfo");
                        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "tokenExpiration");
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    }
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }


    }
}
