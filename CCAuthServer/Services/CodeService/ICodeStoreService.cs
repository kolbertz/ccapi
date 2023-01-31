using CCAuthServer.Models;
using CCAuthServer.OauthRequest;

namespace CCAuthServer.Services.CodeService
{
    public interface ICodeStoreService
    {
        string GenerateAuthorizationCode(Client client, IList<string> requestedScope);
        AuthorizationCode GetClientDataByCode(string key);
        AuthorizationCode RemoveClientDataByCode(string key);
        AuthorizationCode UpdatedClientDataByCode(OpenIdConnectLoginRequest loginRequest);

    }
}
