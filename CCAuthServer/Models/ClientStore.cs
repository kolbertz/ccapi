namespace CCAuthServer.Models
{
    public class ClientStore
    {
        public IEnumerable<Client> Clients = new[]
        {
            new Client
            {
                ClientName = "platformnet .Net 6",
                ClientId = "CCLive",
                ClientSecret = "BYAmz*d4T3a!aucDdwD^K!8^Y2xTGg-Dr",
                AllowedScopes = new[]{ "openid", "profile"},
                GrantType = GrantTypes.Code,
                IsActive = true,
                ClientUri = "https://localhost:7207",
                RedirectUri = "https://localhost:7207/signin-oidc"
            }
        };
    }
}
