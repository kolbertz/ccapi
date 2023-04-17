namespace CCAuthServer.Context
{
    public interface IUserRepository
    {
        Task<List<Tenant>> GetUserSystems(string userName);

        Task<UserData> GetUserData(string userName, Guid sysId);

        Task<List<Guid>> GetUserClaims(UserData userData, string database);
    }
}
