using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CCAuthServer.Context
{
    public class UserRepository : IUserRepository
    {
        private UserDBContext _dbContext;

        public UserRepository(UserDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<UserData> GetUserData(string userName, string password)
        {
            return _dbContext.UserData.Where(x => x.EMail == userName).FirstAsync();
        }

        public async Task<List<Tenant>> GetUserSystems(string userName)
        {
            List<Guid> tenantIds = await _dbContext.UserData.Where(x => x.EMail == userName).SelectMany(x => x.UserTenant.Select(y => y.TenantId)).ToListAsync().ConfigureAwait(false);
            if (tenantIds != null && tenantIds.Count > 0)
            {
                var tenants = await _dbContext.Tenant.Where(sys => tenantIds.Contains(sys.TenantId)).ToListAsync();
                return tenants;
            }
            return null;
        }
    }
}
