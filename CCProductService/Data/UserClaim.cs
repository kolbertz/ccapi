using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace CCProductService.Data
{
    public class UserClaim
    {
        public Guid Tenant { get; set; }
        public Guid SystemId { get; set; }
        public Guid UserId { get; set; }
        public Guid UserGroupId { get; set; }
        public Guid ProfileId { get; set; }
        public IEnumerable<Guid> ProductPoolIds { get; set; }

        public UserClaim(IEnumerable<Claim> claims) {
            string tenant = claims.Where(x => x.Type == "Tenant").Select(x => x.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(tenant))
            {
                Tenant = new Guid(tenant);
            }
            string system = claims.Where(x => x.Type == "SystemId").Select(x => x.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(system))
            {
                SystemId = new Guid(system);
            }
            string user = claims.Where(x => x.Type == "UserId").Select(x => x.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(user))
            {
                UserId = new Guid(user);
            }
            string userGroup = claims.Where(x => x.Type == "UserGroupId").Select(x => x.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(userGroup))
            {
                UserGroupId = new Guid(userGroup);
            }
        }
    }
}
