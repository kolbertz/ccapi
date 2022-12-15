namespace CCAuthServer.Models
{
    public class ClientStore
    {
        public IEnumerable<Client> Clients = new[]
        {
            new Client
            {
                ClientName = "platformnet .Net 6",
                ClientId = "platformnet6",
                ClientSecret = "123456789",
                AllowedScopes = new[]{ "openid", "profile"},
                GrantType = GrantTypes.Code,
                IsActive = true,
                ClientUri = "https://localhost:7207",
                RedirectUri = "https://localhost:7207/signin-oidc"
            }
        };
    }
}
