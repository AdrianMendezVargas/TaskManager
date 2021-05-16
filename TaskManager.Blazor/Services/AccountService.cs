using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskManager.Blazor.Providers;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Blazor.Services {
	public class AccountService : IAccountService {

		private readonly CustomAuthenticationProvider _customAuthenticationProvider;
		private readonly ILocalStorageService _localStorageService;
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		public AccountService(  ILocalStorageService localStorageService ,
								AuthenticationStateProvider authenticationProvider, 
								HttpClient httpClient,
								IConfiguration configuration) {

			_localStorageService = localStorageService;
			_customAuthenticationProvider = authenticationProvider as CustomAuthenticationProvider;
			_httpClient = httpClient;
			_configuration = configuration; //TODO: Get the API URL from a .json
		}
		public async Task<OperationResponse<TokenResponse>> LoginAsync(LoginRequest login) {
			var response = await _httpClient.PostAsJsonAsync("https://localhost:44386/api/account/login" , login);
			var operationResponse = await response.Content.ReadFromJsonAsync<OperationResponse<TokenResponse>>();
			if (operationResponse.IsSuccess) {
				await _localStorageService.SetItemAsync("token" , operationResponse.Record.Token);
				_customAuthenticationProvider.Notify();
				return operationResponse;
			}
			return operationResponse;
		}

		public async Task LogoutAsync() {
			await _localStorageService.RemoveItemAsync("token");
			_customAuthenticationProvider.Notify();
		}

        public async Task<OperationResponse<TokenResponse>> SignUpAsync(RegisterUserRequest request) {
			var response = await _httpClient.PostAsJsonAsync("https://localhost:44386/api/account/register" , request);
			var operationResponse = await response.Content.ReadFromJsonAsync<OperationResponse<TokenResponse>>();
			return operationResponse;
		}

		public async Task<EmptyOperationResponse> ValidateAccountAsync(EmailVerificationRequest verificationRequest) {
			var response = await _httpClient.PostAsJsonAsync("https://localhost:44386/api/account/emailValidation" , verificationRequest);
			var operationResponse = await response.Content.ReadFromJsonAsync<OperationResponse<TokenResponse>>();
			if (operationResponse.IsSuccess) {
				await _localStorageService.SetItemAsync("token" , operationResponse.Record.Token);
				_customAuthenticationProvider.Notify();
				return operationResponse;
			}
			return operationResponse;
		}
    }
}
