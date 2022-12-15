using CCAuthServer.OauthRequest;
using CCAuthServer.OauthResponse;

namespace CCAuthServer.Services
{
    public interface IAuthorizeResultService
    {
        AuthorizeResponse AuthorizeRequest(IHttpContextAccessor httpContextAccessor, AuthorizationRequest authorizationRequest);
        TokenResponse GenerateToken(IHttpContextAccessor httpContextAccessor);
    }
}
