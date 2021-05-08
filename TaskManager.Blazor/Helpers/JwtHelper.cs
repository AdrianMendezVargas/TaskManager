using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskManager.Blazor.Helpers {
    public static class JwtHelper {

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt) {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var jsonBytes = ParseBase64WithoutPadding(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string , object>>(jsonBytes);

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key , kvp.Value.ToString())));

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64) {
            switch (base64.Length % 4) {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }
            return Convert.FromBase64String(base64);
        }

        public static ClaimsPrincipal GetPrincipalFromTokenAsync(string token) {
            const string secret = "ClaveSuperSecretAZNM82HDY7Y1PSKCMX9JD712JSGH";
            var claims = new List<Claim>();

            try {
                IJsonSerializer serializer = new JsonNetSerializer();
                var provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer , provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                IJwtDecoder decoder = new JwtDecoder(serializer , validator , urlEncoder , algorithm);

                var json = decoder.Decode(token , secret , verify: true);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string , object>>(json);
                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key , kvp.Value.ToString())));

            } catch (TokenExpiredException) {
                Console.WriteLine("Token has expired");
            } catch (SignatureVerificationException) {
                Console.WriteLine("Token has invalid signature");
            }

            return claims.Any() ? new ClaimsPrincipal(new ClaimsIdentity(claims , "serverauth"))
                                : new ClaimsPrincipal(new ClaimsIdentity());
        }
    }
}
