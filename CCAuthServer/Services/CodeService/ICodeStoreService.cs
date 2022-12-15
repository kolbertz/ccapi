﻿using CCAuthServer.Models;

namespace CCAuthServer.Services.CodeService
{
    public interface ICodeStoreService
    {
        string GenerateAuthorizationCode(string clientId, IList<string> requestedScope);
        AuthorizationCode GetClientDataByCode(string key);
        AuthorizationCode RemoveClientDataByCode(string key);
        AuthorizationCode UpdatedClientDataByCode(string key, IList<string> requestdScopes,
                string userName, string password = null, string nonce = null);

    }
}
