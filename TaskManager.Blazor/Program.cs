using Blazored.LocalStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Blazor.Providers;
using TaskManager.Blazor.Services;
using TaskManager.Blazor.AppState;
using Microsoft.IdentityModel.Tokens;

namespace TaskManager.Blazor {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBlazoredToast();
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddOptions();

            //3. Add the custom authentication state provider
            builder.Services.AddScoped<AuthenticationStateProvider , CustomAuthenticationProvider>();
            builder.Services.AddAuthorizationCore(); //enables Authorization to our application.

            builder.Services.AddScoped<Appstate>();
            builder.Services.AddScoped<IAccountService , AccountService>();
            builder.Services.AddScoped<ITaskService , TaskService>();

            

            await builder.Build().RunAsync();
        }
    }
}
