using CCApiLibrary.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using static Dapper.SqlMapper;

namespace CCAuthServer.Context
{
    public class UserRepository : IUserRepository
    {
        private UserDBContext _dbContext;
        IServiceProvider _serviceProvider;

        public UserRepository(UserDBContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }

        public Task<UserData> GetUserData(string userName, Guid sysId)
        {
            return _dbContext.UserData.Where(x => x.EMail == userName && x.UserTenant.Any(y => y.TenantId == sysId)).FirstAsync();
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

        public async Task<List<Guid>> GetUserClaims(UserData userData, string database)
        {
            string query = $"SELECT ProfileToUserGroup.UserGroupId FROM UserProfile join ProfileToUserGroup ON UserProfile.ProfileId = ProfileToUserGroup.ProfileId WHERE UserProfile.UserId = @userId; " +
                $"SELECT ProfileToProductPool.ProductPoolId FROM UserProfile join ProfileToProductPool ON UserProfile.ProfileId = ProfileToProductPool.ProfileId WHERE UserProfile.UserId = @userId; " +
                $"SELECT ProfileToCategoryPool.CategoryPoolId FROM UserProfile join ProfileToCategoryPool ON UserProfile.ProfileId = ProfileToCategoryPool.ProfileId WHERE UserProfile.UserId = @userId";

            using (IApplicationDbConnection dbConnection = _serviceProvider.GetService<IApplicationDbConnection>())
            {
                dbConnection.Init(database);
                    using (GridReader gridReader = await dbConnection.QueryMultipleAsync(query, param: new { userId = userData.Id }))
                    {
                        IEnumerable<Guid> userProfileGuids = await gridReader.ReadAsync<Guid>().ConfigureAwait(false);
                        if (userProfileGuids.Count() == 3)
                        {
                            // Success
                            return userProfileGuids.ToList();
                        }
                    }
            }
            return null;
        }
    }
}
