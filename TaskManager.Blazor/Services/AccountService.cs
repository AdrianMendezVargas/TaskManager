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

		public AccountService(ILocalStorageService localStorageService ,
								AuthenticationStateProvider authenticationProvider ,
								HttpClient httpClient ,
								IConfiguration configuration) {

			_localStorageService = localStorageService;
			_customAuthenticationProvider = authenticationProvider as CustomAuthenticationProvider;
			_httpClient = httpClient;
			_configuration = configuration;
		}

		public async Task<OperationResponse<TokenResponse>> LoginAsync(LoginRequest login) {
			HttpResponseMessage response;
			try {
                response = await _httpClient.PostAsJsonAsync(_configuration["API:Account:Login"] , login);
			} catch (Exception) {
				return GetNoContactOperationResponse<TokenResponse>();
			}
			var operationResponse = await response.Content.ReadFromJsonAsync<OperationResponse<TokenResponse>>();
			if (operationResponse.IsSuccess) {
				await _localStorageService.SetItemAsync("token" , operationResponse.Record.Token);
				_customAuthenticationProvider.Notify();
				return operationResponse;
			}
			return operationResponse;
		}

        public async Task<OperationResponse<TokenResponse>> SignUpAsync(RegisterUserRequest request) {
			HttpResponseMessage response;
			try {
				response = await _httpClient.PostAsJsonAsync(_configuration["API:Account:Register"] , request);
			} catch (Exception) {
				return GetNoContactOperationResponse<TokenResponse>();
			}
			var operationResponse = await response.Content.ReadFromJsonAsync<OperationResponse<TokenResponse>>();
			return operationResponse;
		}

		public async Task<EmptyOperationResponse> ValidateAccountAsync(EmailVerificationRequest verificationRequest) {
			HttpResponseMessage response;
			try {
				response = await _httpClient.PostAsJsonAsync(_configuration["API:Account:EmailValidation"] , verificationRequest);
			} catch (Exception) {
				return GetNoContactOperationResponse<TokenResponse>();
			}
			var operationResponse = await response.Content.ReadFromJsonAsync<OperationResponse<TokenResponse>>();
			if (operationResponse.IsSuccess) {
				await _localStorageService.SetItemAsync("token" , operationResponse.Record.Token);
				_customAuthenticationProvider.Notify();
				return operationResponse;
			}
			return operationResponse;
		}

		public async Task<OperationResponse<int>> ResendEmailVerificationAsync() {
            try {
				var operationResponse = await _httpClient.GetFromJsonAsync<OperationResponse<int>>(_configuration["API:Account:ResendEmailValidation"]);
				return operationResponse;
			} catch (Exception) {
				return new OperationResponse<int>() {
					Message = "Could not contact the server" ,
					IsSuccess = false ,
					Record = 30
				};
            }
		}

		public async Task LogoutAsync() {
			await _localStorageService.RemoveItemAsync("token");
			_customAuthenticationProvider.Notify();
		}

		private OperationResponse<T> GetNoContactOperationResponse<T>() {
			return new OperationResponse<T> {
				IsSuccess = false ,
				Message = "Could not contact the server" ,
				Record = default
			};
		}
	}
}
