using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCUserService.Data;
using CCUserService.DTOs;
using CCUserService.Interface;
using System.Dynamic;

namespace CCUserService.Repositories
{
    public class UserRepository : IUserRepository
    {
        public IApplicationDbConnection _dbContext { get; }

        public UserRepository(IApplicationDbConnection writeDbConnection)

        {
            _dbContext = writeDbConnection;
        }

        public void Init(string database)
        {
            _dbContext.Init(database);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
        public async Task<IEnumerable<User>> GetAllUsers(int? take, int? skip, UserClaim userClaim)
        {
            string query;
            string userQuery = string.Empty;
            string sysIdQuery = string.Empty;
            var paramObj = new ExpandoObject();

            if (userClaim.SystemId.HasValue)
            {
                sysIdQuery = " WHERE UserGroup.SystemSettingsId = @SysId";
                paramObj.TryAdd("SysId", userClaim.SystemId);
            }

            if (userClaim.UserGroupId != null) 
            {
                userQuery = " AND User.UserGroupId in @groupIds";
                paramObj.TryAdd("groupIds", userClaim.UserId.ToByteArray());
            }

            if (take.HasValue && skip.HasValue) 
            {
                //(Baustelle) auf Richtigkeit überprüfen
                query = $"SELECT[Id] ,[Language],[Culture],[IsBlocked] ,[FirstName],[LastName] ,[Comment] ,[IsApproved] ,[CreatedDate] ,[CreatedUser] ,[LastUpdatedDate] ,[LastUpdatedUser] ," +
                   $"[ProfileId] ,[UserGroupId] ,[UserImage] ,[LicenseId] ,[IsInactive] ,[DisplayName],[ValidFrom] ,[ValidThrough] ,[EMail],[EMailReceipt] ,[SignInCache] FROM User{sysIdQuery}{userQuery}  " +
                   $"ORDER BY UserGroupId   " +
                   $"OFFSET @offset ROWS FETCH NEXT @fetch ROWS ONLY " +
                   $"LEFT JOIN UserGroup on User.UserGroupId = UserGroup.Id";

                paramObj.TryAdd("offset", skip.Value);
                paramObj.TryAdd("fetch", take.Value);
            }
            else
            {
                query = $"SELECT[Id] ,[Language],[Culture],[IsBlocked] ,[FirstName],[LastName] ,[Comment] ,[IsApproved] ,[CreatedDate] ,[CreatedUser] ,[LastUpdatedDate] ,[LastUpdatedUser] ," +
                  $"[ProfileId] ,[UserGroupId] ,[UserImage] ,[LicenseId] ,[IsInactive] ,[DisplayName],[ValidFrom] ,[ValidThrough] ,[EMail],[EMailReceipt] ,[SignInCache] " +
                  $"FROM User  " +
                  $"JOIN UserGroup on User.UserGroupId = UserGroup.Id{sysIdQuery}{userQuery}" +
                  $"ORDER BY UserGroupId";                 
                  
            }


            //return _dbContext.QueryAsync<InternalUser, Guid, User>(query, (internalUser, sysId) =>
            //{
            //    return new User(internalUser, sysId);
            //}, splitOn: "[FirstName], SystemSettingsId");
            return null;

        }

        public async Task<User> GetUserById(Guid id, UserClaim userClaim)
        {
            var paramObj = new ExpandoObject();
            string sysIdQuery = string.Empty;

            if (userClaim.SystemId.HasValue)
            {
                sysIdQuery = " AND SystemSettingsId = @SysId";
                paramObj.TryAdd("SysId", userClaim.SystemId);
            }

            var query = $"SELECT[Id] ,[Language],[Culture],[IsBlocked] ,[FirstName],[LastName] ,[Comment] ,[IsApproved] ,[CreatedDate] ,[CreatedUser] ,[LastUpdatedDate] ,[LastUpdatedUser] ," +
                  $"[ProfileId] ,[UserGroupId] ,[UserImage] ,[LicenseId] ,[IsInactive] ,[DisplayName],[ValidFrom] ,[ValidThrough] ,[EMail],[EMailReceipt] ,[SignInCache] " +
                  $"FROM User WHERE Id = @UserId{sysIdQuery} ";

            InternalUser internalUser = await _dbContext.QueryFirstOrDefaultAsync<InternalUser>(query, param: new {UserId = id});

            if (internalUser != null) 
            {
                User user = new User(internalUser);
                internalUser.MergeUser(user);
                return user;
            }
            return null;
        }
    }
}
