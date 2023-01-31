using CCAuthServer.Models;
using CCAuthServer.OauthRequest;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace CCAuthServer.Services.CodeService
{
    public class CodeStoreService : ICodeStoreService
    {
        private readonly ConcurrentDictionary<string, AuthorizationCode> _codeIssued = new ConcurrentDictionary<string, AuthorizationCode>();
        private readonly ClientStore _clientStore = new ClientStore();

        // Here I genrate the code for authorization, and I will store it 
        // in the Concurrent Dictionary

        public string GenerateAuthorizationCode(Client client, IList<string> requestedScope)
        {
            if (client != null)
            {
                var code = Guid.NewGuid().ToString();

                var authoCode = new AuthorizationCode
                {
                    ClientId = client.ClientId,
                    RedirectUri = client.RedirectUri,
                    RequestedScopes = requestedScope,
                };

                // then store the code is the Concurrent Dictionary
                _codeIssued[code] = authoCode;

                return code;
            }
            return null;

        }

        public AuthorizationCode GetClientDataByCode(string key)
        {
            AuthorizationCode authorizationCode;
            if (_codeIssued.TryGetValue(key, out authorizationCode))
            {
                return authorizationCode;
            }
            return null;
        }

        public AuthorizationCode RemoveClientDataByCode(string key)
        {
            AuthorizationCode authorizationCode;
            _codeIssued.TryRemove(key, out authorizationCode);
            return null;
        }

        // TODO
        // Before updated the Concurrent Dictionary I have to Process User Sign In,
        // and check the user credienail first
        // But here I merge this process here inside update Concurrent Dictionary method
        public AuthorizationCode UpdatedClientDataByCode(OpenIdConnectLoginRequest loginRequest)
        {
            //loginRequest.Code, loginRequest.RequestedScopes,
            //        loginRequest.UserName, nonce: loginRequest.Nonce, systemSettingsId: loginRequest.SystemSettingId
            var oldValue = GetClientDataByCode(loginRequest.Code);

            if (oldValue != null)
            {
                // check the requested scopes with the one that are stored in the Client Store 
                var client = _clientStore.Clients.Where(x => x.ClientId == oldValue.ClientId).FirstOrDefault();

                if (client != null)
                {
                    if (loginRequest.RequestedScopes != null)
                    {
                        var clientScope = (from m in client.AllowedScopes
                                           where loginRequest.RequestedScopes.Contains(m)
                                           select m).ToList();

                        if (!clientScope.Any())
                            return null;
                    }



                    AuthorizationCode newValue = new AuthorizationCode
                    {
                        ClientId = oldValue.ClientId,
                        CreationTime = oldValue.CreationTime,
                        IsOpenId = true,// requestdScopes.Contains("openId") || requestdScopes.Contains("profile"),
                        RedirectUri = oldValue.RedirectUri,
                        RequestedScopes = loginRequest.RequestedScopes,
                        Nonce = loginRequest.Nonce,
                    };


                    // ------------------ I suppose the user name and password is correct  -----------------
                    var claims = new List<Claim>();

                    claims.Add(new Claim("Tenant", loginRequest.SystemSettingId.ToString()));
                    claims.Add(new Claim("TenantDatabase", loginRequest.TenantDatabase));

                    if (newValue.IsOpenId)
                    {
                        // TODO
                        // Add more claims to the claims

                    }

                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    newValue.Subject = new ClaimsPrincipal(claimIdentity);
                    // ------------------ -----------------------------------------------  -----------------

                    var result = _codeIssued.TryUpdate(loginRequest.Code, newValue, oldValue);

                    if (result)
                        return newValue;
                    return null;
                }
            }
            return null;
        }
    }
}
