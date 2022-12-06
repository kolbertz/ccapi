using CCProductService.Data;
using CCProductService.Interface;
using Dapper;

namespace CCProductService.Repositories
{
    public class ClaimsRepository : IClaimsRepository
    {
        public IApplicationDbContext _dbContext { get; }
        public IApplicationReadDbConnection _readDbContext { get; }
        public IApplicationWriteDbConnection _writeDbContext { get; }

        public ClaimsRepository(IApplicationDbContext dbContext, IApplicationReadDbConnection readDbConnection, IApplicationWriteDbConnection writeDbConnection)
        {
            _dbContext = dbContext;
            _readDbContext = readDbConnection;
            _writeDbContext = writeDbConnection;
        }

        public async Task GetProductPoolIds(UserClaim userClaim)
        {
            string query = $"Select ProfileToProductPool.ProductPoolId From ProfileToProductPool where ProfileToProductPool.ProfileId = @ProfileId";
            userClaim.ProductPoolIds = await _readDbContext.Connection.QueryAsync<Guid>(query, param: new {ProfileId = userClaim.ProfileId});
        }

        public async Task GetProfileId(UserClaim userClaim)
        {
            string query = $"Select ProfileToUserGroup.ProfileId from (Select ug.Id, ug.ParentGroupId from UserGroup ug where ug.Id = @UserGroupId and ug.SystemSettingsId = @SystemId) as t " +
                $"left join ProfileToUserGroup ptu on ptu.UserGroupId = t.Id left join ProfileToUserGroup on ProfileToUserGroup.UserGroupId = t.ParentGroupId";

            object paramObj = new
            {
                UserGroupId = userClaim.UserGroupId,
                SystemId = userClaim.SystemId
            };

            userClaim.ProfileId = await _readDbContext.Connection.ExecuteScalarAsync<Guid>(query, param: paramObj).ConfigureAwait(false);
        }
    }
}
