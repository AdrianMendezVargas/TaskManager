using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManager.Blazor.Providers;

namespace TaskManager.Blazor.AppState {
    public class Appstate {

        private readonly CustomAuthenticationProvider _customAuthentication;
        public Appstate(AuthenticationStateProvider authenticationState) {
            _customAuthentication = authenticationState as CustomAuthenticationProvider;
        }

        public Task<ClaimsPrincipal> PrincipalAsync { 
            get {
                return _customAuthentication.GetClaimsPrincipal();
            } 
        }
    }
}
