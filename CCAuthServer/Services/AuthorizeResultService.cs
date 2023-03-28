using CCAuthServer.Common;
using CCAuthServer.Context;
using CCAuthServer.Models;
using CCAuthServer.OauthRequest;
using CCAuthServer.OauthResponse;
using CCAuthServer.Services.CodeService;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CCAuthServer.Services
{
    public class AuthorizeResultService : IAuthorizeResultService
    {

        private static string keyAlg = "WlTqAvzb9JAOD8wS72D9WLBc1y34xMiAzMGRO/DaXxU=";
        private readonly ClientStore _clientStore = new ClientStore();
        private readonly ICodeStoreService _codeStoreService;
        IUserRepository _userRepository;

        public AuthorizeResultService(ICodeStoreService codeStoreService, IUserRepository userRepository)
        {
            _codeStoreService = codeStoreService;
            _userRepository = userRepository;
        }
        public AuthorizeResponse AuthorizeRequest(IHttpContextAccessor httpContextAccessor, AuthorizationRequest authorizationRequest)
        {
            AuthorizeResponse response = new AuthorizeResponse();

            if (httpContextAccessor == null)
            {
                response.Error = ErrorTypeEnum.ServerError.GetEnumDescription();
                return response;
            }

            var client = VerifyClientById(authorizationRequest.client_id);
            if (!client.IsSuccess)
            {
                response.Error = client.ErrorDescription;
                return response;
            }
            client.Client.RedirectUri = authorizationRequest.redirect_uri;
            if (string.IsNullOrEmpty(authorizationRequest.response_type) || authorizationRequest.response_type != "code")
            {
                response.Error = ErrorTypeEnum.InvalidRequest.GetEnumDescription();
                response.ErrorDescription = "response_type is required or is not valid";
                return response;
            }

            //if (!authorizationRequest.redirect_uri.IsRedirectUriStartWithHttps() && !httpContextAccessor.HttpContext.Request.IsHttps)
            //{
            //    response.Error = ErrorTypeEnum.InvalidRequest.GetEnumDescription();
            //    response.ErrorDescription = "redirect_url is not secure, MUST be TLS";
            //    return response;
            //}


            // check the return url is match the one that in the client store


            // check the scope in the client store with the
            // one that is comming from the request MUST be matched at leaset one

            // TODO: Think about different scopes
            //var scopes = authorizationRequest.scope.Split(' ');
            var clientScopes = new List<string>();
            //var clientScopes = from m in client.Client.AllowedScopes
            //                   where scopes.Contains(m)
            //                   select m;

            //if (!clientScopes.Any())
            //{
            //    response.Error = ErrorTypeEnum.InValidScope.GetEnumDescription();
            //    response.ErrorDescription = "scopes are invalids";
            //    return response;
            //}

            string nonce = httpContextAccessor.HttpContext.Request.Query["nonce"].ToString();

            // Verify that a scope parameter is present and contains the openid scope value.
            // (If no openid scope value is present,
            // the request may still be a valid OAuth 2.0 request, but is not an OpenID Connect request.)

            string code = _codeStoreService.GenerateAuthorizationCode(client.Client, clientScopes);
            if (code == null)
            {
                response.Error = ErrorTypeEnum.TemporarilyUnAvailable.GetEnumDescription();
                return response;
            }

            response.RedirectUri = client.Client.RedirectUri + "?response_type=code" + "&state=" + authorizationRequest.state;
            response.Code = code;
            response.State = authorizationRequest.state;
            response.RequestedScopes = clientScopes.ToList();
            response.Nonce = nonce;

            return response;

        }




        private CheckClientResult VerifyClientById(string clientId, bool checkWithSecret = false, string clientSecret = null)
        {
            CheckClientResult result = new CheckClientResult() { IsSuccess = false };

            if (!string.IsNullOrWhiteSpace(clientId))
            {
                var client = _clientStore.Clients.Where(x => x.ClientId.Equals(clientId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (client != null)
                {
                    if (checkWithSecret && !string.IsNullOrEmpty(clientSecret))
                    {
                        bool hasSamesecretId = client.ClientSecret.Equals(clientSecret, StringComparison.InvariantCulture);
                        if (!hasSamesecretId)
                        {
                            result.Error = ErrorTypeEnum.InvalidClient.GetEnumDescription();
                            return result;
                        }
                    }


                    // check if client is enabled or not

                    if (client.IsActive)
                    {
                        result.IsSuccess = true;
                        result.Client = client;

                        return result;
                    }
                    else
                    {
                        result.ErrorDescription = ErrorTypeEnum.UnAuthoriazedClient.GetEnumDescription();
                        return result;
                    }
                }
            }

            result.ErrorDescription = ErrorTypeEnum.AccessDenied.GetEnumDescription();
            return result;
        }

        public TokenResponse GenerateToken(IHttpContextAccessor httpContextAccessor)
        {
            TokenRequest request = new TokenRequest();

            request.CodeVerifier = httpContextAccessor.HttpContext.Request.Form["code_verifier"];
            request.ClientId = httpContextAccessor.HttpContext.Request.Form["client_id"];
            request.ClientSecret = httpContextAccessor.HttpContext.Request.Form["client_secret"];
            request.Code = httpContextAccessor.HttpContext.Request.Form["code"];
            request.GrantType = httpContextAccessor.HttpContext.Request.Form["grant_type"];
            request.RedirectUri = httpContextAccessor.HttpContext.Request.Form["redirect_uri"];

            var checkClientResult = this.VerifyClientById(request.ClientId, true, request.ClientSecret);
            if (!checkClientResult.IsSuccess)
            {
                return new TokenResponse { Error = checkClientResult.Error, ErrorDescription = checkClientResult.ErrorDescription };
            }

            // check code from the Concurrent Dictionary
            var clientCodeChecker = _codeStoreService.GetClientDataByCode(request.Code);
            if (clientCodeChecker == null)
                return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.GetEnumDescription() };


            // check if the current client who is one made this authentication request

            if (request.ClientId != clientCodeChecker.ClientId)
                return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.GetEnumDescription() };

            // TODO: 
            // also I have to check the rediret uri 


            // Now here I will Issue the Id_token

            JwtSecurityToken id_token = null;
            if (clientCodeChecker.IsOpenId)
            {
                // Generate Identity Token
                int iat = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;


                string[] amrs = new string[] { "pwd" };

                var key = new SymmetricSecurityKey(Convert.FromBase64String(keyAlg));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("sub", "856933325856"),
                    new Claim("given_name", "Mohammed Ahmed Hussien"),
                    new Claim("iat", iat.ToString(), ClaimValueTypes.Integer), // time stamp
                    //new Claim("nonce", clientCodeChecker.Nonce)
                };
                foreach (var amr in amrs)
                    claims.Add(new Claim("amr", amr));// authentication method reference 

                id_token = new JwtSecurityToken("localhost", "all", claims, signingCredentials: credentials,
                    expires: DateTime.UtcNow.AddMinutes(
                       int.Parse("5")));
                id_token.SigningKey = key;

            }

            // Here I have to generate access token 
            var key_at = new SymmetricSecurityKey(Convert.FromBase64String(keyAlg));
            var credentials_at = new SigningCredentials(key_at, SecurityAlgorithms.HmacSha256);

            var claims_at = clientCodeChecker.Subject.Claims.ToList();
            claims_at.Add(new Claim("SystemId", "f2a46b64-ab2e-47b1-84fe-8cc91b241165"));
            claims_at.Add(new Claim("UserId", "e5ae5497-c6d6-4f16-8292-50fd6bf64f7c"));
            claims_at.Add(new Claim("UserGroupId", "05e652ff-c4b6-4966-8da4-08cb9d102081"));


            var access_token = new JwtSecurityToken("localhost", "all", claims_at, signingCredentials: credentials_at,
                expires: DateTime.UtcNow.AddMinutes(
                   int.Parse("5")));
            access_token.SigningKey = key_at;


            // here remoce the code from the Concurrent Dictionary
            _codeStoreService.RemoveClientDataByCode(request.Code);

            return new TokenResponse
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(access_token),
                id_token = id_token != null ? new JwtSecurityTokenHandler().WriteToken(id_token) : null,
                code = request.Code
            };
        }
    }
}
