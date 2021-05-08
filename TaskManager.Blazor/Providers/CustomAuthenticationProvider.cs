using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Blazor.Helpers;
using TaskManager.Services;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Blazor.Providers {
    //1. Install Microsoft.AspNetCore.Components.WebAssembly.Authentication

    //2. Creating the custom authentication provider
    public class CustomAuthenticationProvider : AuthenticationStateProvider {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;

        public CustomAuthenticationProvider(ILocalStorageService localStorageService, HttpClient httpClient) {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {

            string token = await _localStorageService.GetItemAsync<string>("token");
            if (string.IsNullOrEmpty(token)) {
                var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                return anonymous;
            }
            var claimRequest = new GetClaimsRequest() { Token = token };
            var response = await _httpClient.PostAsJsonAsync("https://localhost:44386/api/account/token" , claimRequest);
            var operationResponse = await response.Content.ReadFromJsonAsync<OperationResponse<Dictionary<string, string>>>();

            if (!operationResponse.IsSuccess) {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

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
