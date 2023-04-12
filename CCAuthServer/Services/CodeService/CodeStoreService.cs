using CCAuthServer.Context;
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
        IServiceProvider _serviceProvider;

        public CodeStoreService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

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
        public async Task<AuthorizationCode> UpdatedClientDataByCode(OpenIdConnectLoginRequest loginRequest, UserData userData)
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
                    //claims_at.Add(new Claim("SystemId", "f2a46b64-ab2e-47b1-84fe-8cc91b241165"));
                    //claims_at.Add(new Claim("UserGroupId", "05e652ff-c4b6-4966-8da4-08cb9d102081"));
                    List<Guid> ids = null;
                    using (IServiceScope service = _serviceProvider.CreateScope())
                    {
                        ids = await service.ServiceProvider.GetService<IUserRepository>().GetUserClaims(userData, loginRequest.TenantDatabase).ConfigureAwait(false);
                    }
                    if (ids != null)
                    {
                        claims.Add(new Claim("UserGroupId", ids[0].ToString()));
                        claims.Add(new Claim("ProductPoolId", ids[1].ToString()));
                        claims.Add(new Claim("CategoryPoolId", ids[2].ToString()));
                    }
                    claims.Add(new Claim("UserId", userData.Id.ToString()));
                    claims.Add(new Claim("TenantId", loginRequest.SystemSettingId.ToString()));
                    claims.Add(new Claim("SystemId", loginRequest.SystemSettingId.ToString()));
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
