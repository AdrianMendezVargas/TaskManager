using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Blazor.Providers;

namespace TaskManager.Blazor.Services {
	public class AccountService : IAccountService {

		private readonly AuthenticationStateProvider _customAuthenticationProvider;
		private readonly ILocalStorageService _localStorageService;

		public AccountService(ILocalStorageService localStorageService ,
		  AuthenticationStateProvider customAuthenticationProvider) {
			_localStorageService = localStorageService;
			_customAuthenticationProvider = customAuthenticationProvider;
		}
		public async Task<bool> LoginAsync() {
			string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im5hdmVlbkBnbWFpbC5jb20iLCJwaG9uZSI6IjEyMzQ1Njc4OTAiLCJleHAiOjE2MDMxOTQ2MTIsImlzcyI6ImxvY2FsaG9zdDo1MDAwIiwiYXVkIjoiQVBJIn0.wRPD4THnUzLhJZhu4eNMx1ztbNAABQ9rkIEJaWBZX_c";
			await _localStorageService.SetItemAsync("token" , token);
			(_customAuthenticationProvider as CustomAuthenticationProvider).Notify();
			return true;
		}

		public async Task<bool> LogoutAsync() {
			await _localStorageService.RemoveItemAsync("token");
			(_customAuthenticationProvider as CustomAuthenticationProvider).Notify();
			return true;
		}
	}
}
