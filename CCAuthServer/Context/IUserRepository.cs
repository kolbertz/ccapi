namespace CCAuthServer.Context
{
    public interface IUserRepository
    {
        Task<List<Tenant>> GetUserSystems(string userName);

        Task<UserData> GetUserData(string userName, string password);
    }
}
