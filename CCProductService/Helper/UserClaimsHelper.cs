using CCProductService.Data;
using System.Dynamic;
using System.Security.Claims;

namespace CCProductService.Helper
{
    public class UserClaimsHelper
    {
        public static UserClaim GetUserClaim(IEnumerable<Claim> claims)
        {
            IDictionary<string, object> properties = new ExpandoObject();
            foreach (var item in claims)
            {
                properties.Add(item.Type, item.Value);
            }
            return properties as UserClaim;
        }
    }
}

