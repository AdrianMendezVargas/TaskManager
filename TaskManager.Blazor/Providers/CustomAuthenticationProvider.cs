using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Blazor.Providers {
    //1. Install Microsoft.AspNetCore.Components.WebAssembly.Authentication

    //2. Creating the custom authentication provider
    public class CustomAuthenticationProvider : AuthenticationStateProvider {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configutarion;

        public CustomAuthenticationProvider(ILocalStorageService localStorageService, HttpClient httpClient, IConfiguration configuration) {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
            _configutarion = configuration;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {

            string token = await _localStorageService.GetItemAsync<string>("token");
            if (string.IsNullOrEmpty(token)) {
                var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                return anonymous;
            }
            var claimRequest = new GetClaimsRequest() { Token = token };
            var response = await _httpClient.PostAsJsonAsync(_configutarion["API:Account:Token"] , claimRequest);
            var operationResponse = await response.Content.ReadFromJsonAsync<OperationResponse<Dictionary<string, string>>>();

            if (!operationResponse.IsSuccess) {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" , token); //Set the token in every request by default in the httpClient

            var claims = operationResponse.Record.Select(kvp => new Claim(kvp.Key , kvp.Value.ToString()));
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims , "serverauth")));
        }

        public async Task<ClaimsPrincipal> GetClaimsPrincipal() {
            return (await GetAuthenticationStateAsync()).User;
        }

        public void Notify() {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
