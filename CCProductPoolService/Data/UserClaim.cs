using System.Security.Claims;

namespace CCProductPoolService.Data
{
    public class UserClaim
    {
        public Guid TenantId { get; set; }
        public Guid SystemId { get; set; }
        public Guid UserId { get; set; }
        public Guid UserGroupId { get; set; }
        public Guid ProfileId { get; set; }
        public IEnumerable<Guid> ProductPoolIds { get; set; }
        public string TenantDatabase { get; set; }

        public UserClaim(IEnumerable<Claim> claims)
        {
            string tenant = claims.Where(x => x.Type == "Tenant").Select(x => x.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(tenant))
            {
                TenantId = new Guid(tenant);
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
            string tenantDatabase = claims.Where(x => x.Type == "TenantDatabase").Select(x => x.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantDatabase))
            {
                TenantDatabase = tenantDatabase;
            }
            else
            {
                TenantDatabase = "TestDatabase";
            }
        }
    }
}
