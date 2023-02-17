using CCProductService.Data;
using CCProductService.Interface;
using Dapper;

namespace CCProductService.Repositories
{
    public class ClaimsRepository : IClaimsRepository
    {
        public IApplicationDbConnection _dbContext { get; }

        public ClaimsRepository(IApplicationDbConnection writeDbConnection)
        {
            _dbContext = writeDbConnection;
        }

        public async Task GetProductPoolIds(UserClaim userClaim)
        {
            string query = $"Select ProfileToProductPool.ProductPoolId From ProfileToProductPool where ProfileToProductPool.ProfileId = @ProfileId";
            userClaim.ProductPoolIds = await _dbContext.QueryAsync<Guid>(query, param: new {ProfileId = userClaim.ProfileId});
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

            userClaim.ProfileId = await _dbContext.ExecuteScalarAsync<Guid>(query, param: paramObj).ConfigureAwait(false);
        }
    }
}
