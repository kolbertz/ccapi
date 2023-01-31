﻿namespace CCAuthServer.OauthRequest
{
    public class OpenIdConnectLoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RedirectUri { get; set; }
        public string Code { get; set; }
        public string Nonce { get; set; }
        public IList<string> RequestedScopes { get; set; }
        public Guid SystemSettingId { get; set; }
        public string TenantDatabase { get; set; }
    }
}
